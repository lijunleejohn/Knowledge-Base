using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System;
using System.Linq;
using System.Net.Http;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureMvcWithAuthenAuthor.Infrastructure;
using AzureMvcWithAuthenAuthor.Models;
using AzureMvcWithAuthenAuthor.Services;
using Graph = Microsoft.Graph;
using Constants = AzureMvcWithAuthenAuthor.Infrastructure.Constants;
using Microsoft.AspNetCore.Http;

namespace AzureMvcWithAuthenAuthor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly WebOptions webOptions;

        public HomeController(ITokenAcquisition tokenAcquisition, IOptions<WebOptions> webOptionValue)
        {
            this.tokenAcquisition = tokenAcquisition;
            this.webOptions = webOptionValue.Value;
        }

        [AllowAnonymous]
        public IActionResult Introduction()
        {
            return View();
        }

        public IActionResult Index()
        {
            ViewData["User"] = HttpContext.User;
            return View();
        }

        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead })]
        public async Task<IActionResult> Profile()
        {
            ViewData["DisplayName"] = this.HttpContext.Session.GetString("DisplayName");
            ViewData["photo"] = this.HttpContext.Session.GetString("Photo");
            // Initialize the GraphServiceClient.
            Graph::GraphServiceClient graphClient = GetGraphServiceClient(new[] { Constants.ScopeUserRead });

            var me = await graphClient.Me.Request().GetAsync();
            ViewData["Me"] = me;

            try
            {
                // Get user photo
                var photoStream = await graphClient.Me.Photo.Content.Request().GetAsync();
                byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                ViewData["Photo"] = Convert.ToBase64String(photoByte);
            }
            catch (System.Exception)
            {
                ViewData["Photo"] = null;
            }

            return View();
        }

        /// <summary>
        /// Fetches and displays all the users in this directory. This method requires the signed-in user to be assigned to the 'UserReaders' approle.
        /// </summary>
        /// <returns></returns>
        [AuthorizeForScopes(Scopes = new[] { GraphScopes.UserReadBasicAll })]
        [Authorize(Policy = AuthorizationPolicies.AdminsRole)]
        public async Task<IActionResult> Admin()
        {
            ViewData["DisplayName"] = this.HttpContext.Session.GetString("DisplayName");
            ViewData["photo"] = this.HttpContext.Session.GetString("Photo");
            List<UserProfile> userProfiles = await GetAllUsers();

            ViewData["UserProfiles"] = userProfiles;
            return View();
        }

        private async Task<List<UserProfile>> GetAllUsers()
        {
            // Initialize the GraphServiceClient.
            Graph::GraphServiceClient graphClient = GetGraphServiceClient(new[] { GraphScopes.UserReadBasicAll });
            var users = await graphClient.Users.Request().GetAsync();
            List<UserProfile> userProfiles = new List<UserProfile>();

            foreach (Graph.User u in users.CurrentPage)
            {
                string photo;
                try
                {
                    var photoStream = await graphClient.Users[u.Id].Photo.Content.Request().GetAsync();
                    byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                    photo = Convert.ToBase64String(photoByte);
                }
                catch
                {
                    photo = null;
                }

                userProfiles.Add(new UserProfile
                {
                    TokenID = u.Id,
                    DisplayName = u.DisplayName,
                    LoginID = u.UserPrincipalName,
                    PhotoBase64 = photo
                });
            }

            return userProfiles;
        }

        private Graph::GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return result;
            }, webOptions.GraphApiUrl);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}