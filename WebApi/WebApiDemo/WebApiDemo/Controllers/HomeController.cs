using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApiDemo.Models;

namespace WebApiDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            using (SchoolContext context = new SchoolContext())
            {
                List<WebApiDemo.UIModels.Student> students =
                    (from s in context.Students
                     select new WebApiDemo.UIModels.Student
                     {
                         StudentID = s.StudentID,
                         StudentName = s.StudentName,
                         DateOfBirth = s.DateOfBirth,
                         GenderId = (GenderEnum)(s.GenderId)
                     }).ToList<WebApiDemo.UIModels.Student>();
                return View(students);
            }
        }

        /// <summary>
        /// Use optional parameter, if not provided, equals to 0
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            using (SchoolContext context = new SchoolContext())
            {
                WebApiDemo.UIModels.Student student =
                    (from s in context.Students
                     where s.StudentID == id
                     select new WebApiDemo.UIModels.Student
                     {
                         StudentID = s.StudentID,
                         StudentName = s.StudentName,
                         DateOfBirth = s.DateOfBirth,
                         GenderId = (GenderEnum)(s.GenderId)
                     }).FirstOrDefault<WebApiDemo.UIModels.Student>();
                return View(student);
            }
        }

        [HttpPost]
        public ActionResult Edit(WebApiDemo.UIModels.Student student)
        {
            // server validate
            if (!ModelState.IsValid)
            {
                return View();
            }

            using (SchoolContext context = new SchoolContext())
            {
                bool isExist = (from s in context.Students where s.StudentID == student.StudentID select s).Count<Student>() > 0;

                if (isExist)
                {
                    var record = context.Students.FirstOrDefault(s => s.StudentID == student.StudentID);
                    if (record != null)
                    {
                        record.StudentName = student.StudentName;
                        record.DateOfBirth = student.DateOfBirth;
                        record.Photo = student.Photo;
                        record.GenderId = (int)student.GenderId;

                        context.SaveChanges();
                    }
                }
                else
                {
                    Student newStudent = new Student
                    {
                        StudentName = student.StudentName,
                        DateOfBirth = student.DateOfBirth,
                        GenderId = (int)student.GenderId
                    };

                    context.Students.Add(newStudent);
                    context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
        }

        public ActionResult Details(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            using (SchoolContext context = new SchoolContext())
            {
                WebApiDemo.UIModels.Student student =
                    (from s in context.Students
                     where s.StudentID == id
                     select new WebApiDemo.UIModels.Student
                     {
                         StudentID = s.StudentID,
                         StudentName = s.StudentName,
                         DateOfBirth = s.DateOfBirth,
                         GenderId = (GenderEnum)(s.GenderId)
                     }).FirstOrDefault<WebApiDemo.UIModels.Student>();
                return View(student);
            }
        }

        public ActionResult Delete(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            using (SchoolContext context = new SchoolContext())
            {
                var record = context.Students.FirstOrDefault(s => s.StudentID == id);
                if (record != null)
                {
                    context.Students.Remove(record);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
