using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using AzureMvcWithAuthenAuthor.Models;
using AzureMvcWithAuthenAuthor.Infrastructure;
using AzureMvcWithAuthenAuthor.Services;
using Graph = Microsoft.Graph;
using Constants = AzureMvcWithAuthenAuthor.Infrastructure.Constants;
using Microsoft.AspNetCore.Http;

namespace AzureMvcWithAuthenAuthor.Controllers
{
    [AuthorizeForScopes(Scopes = new[] { GraphScopes.UserReadBasicAll })]
    [Authorize(Policy = AuthorizationPolicies.ReadersRole)]
    public class ProductsController : Controller
    {
        private readonly IndustryDatabaseContext _context;
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly WebOptions webOptions;

        public ProductsController(ITokenAcquisition tokenAcquisition, IOptions<WebOptions> webOptionValue, IndustryDatabaseContext context)
        {
            this.tokenAcquisition = tokenAcquisition;
            this.webOptions = webOptionValue.Value;
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(this.HttpContext.Session.GetString("TokenID")) || this.HttpContext.Session.GetString("TokenID") != User.Claims.Where(r => r.Type == "tid").FirstOrDefault().Value)
            {
                UserProfile user = GetUserProfile().Result;
                HttpContext.Session.SetString("TokenID", user.TokenID);
                HttpContext.Session.SetString("DisplayName", user.DisplayName);
                HttpContext.Session.SetString("Photo", user.PhotoBase64);
            }

            ViewData["DisplayName"] = this.HttpContext.Session.GetString("DisplayName");
            ViewData["photo"] = this.HttpContext.Session.GetString("Photo");
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? ID)
        {
            ViewData["DisplayName"] = this.HttpContext.Session.GetString("DisplayName");
            ViewData["photo"] = this.HttpContext.Session.GetString("Photo");
            if (ID == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ID == ID);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["DisplayName"] = this.HttpContext.Session.GetString("DisplayName");
            ViewData["photo"] = this.HttpContext.Session.GetString("Photo");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Description,UnitPrice")] Product product)
        {
            ViewData["DisplayName"] = this.HttpContext.Session.GetString("DisplayName");
            ViewData["photo"] = this.HttpContext.Session.GetString("Photo");
            if (ModelState.IsValid)
            {
                product.UpdatedDate = DateTime.Now;
                product.UpdatedBy = this.User.Identity.Name;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? ID)
        {
            ViewData["DisplayName"] = this.HttpContext.Session.GetString("DisplayName");
            ViewData["photo"] = this.HttpContext.Session.GetString("Photo");
            if (ID == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(ID);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ID, [Bind("ID,Code,Description,UnitPrice")] Product product)
        {
            ViewData["DisplayName"] = this.HttpContext.Session.GetString("DisplayName");
            ViewData["photo"] = this.HttpContext.Session.GetString("Photo");
            if (ID != product.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.UpdatedDate = DateTime.Now;
                    product.UpdatedBy = this.User.Identity.Name;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? ID)
        {
            ViewData["DisplayName"] = this.HttpContext.Session.GetString("DisplayName");
            ViewData["photo"] = this.HttpContext.Session.GetString("Photo");
            if (ID == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ID == ID);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ID)
        {
            ViewData["DisplayName"] = this.HttpContext.Session.GetString("DisplayName");
            ViewData["photo"] = this.HttpContext.Session.GetString("Photo");
            var product = await _context.Products.FindAsync(ID);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int ID)
        {
            return _context.Products.Any(e => e.ID == ID);
        }

        /// <summary>
        /// Fetches and displays all the users in this directory. This method requires the signed-in user to be assigned to the 'UserReaders' approle.
        /// </summary>
        /// <returns></returns>
        private async Task<UserProfile> GetUserProfile()
        {
            UserProfile user = new UserProfile();
            // Initialize the GraphServiceClient.
            Graph::GraphServiceClient graphClient = GetGraphServiceClient(new[] { Constants.ScopeUserRead });

            var me = await graphClient.Me.Request().GetAsync();
            user.TokenID = me.Id;
            user.DisplayName = me.DisplayName;
            user.LoginID = me.UserPrincipalName;

            try
            {
                // Get user photo
                var photoStream = await graphClient.Me.Photo.Content.Request().GetAsync();
                byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                user.PhotoBase64 = Convert.ToBase64String(photoByte);
            }
            catch (System.Exception)
            {
                user.PhotoBase64 = null;
            }

            return user;
        }

        private Graph::GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return result;
            }, webOptions.GraphApiUrl);
        }
    }
}
