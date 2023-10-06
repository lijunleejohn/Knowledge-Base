using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Web.Mvc;
using WebApiDemo.Models;

namespace WebApiDemo.Controllers
{
    /// <summary>
    /// This is a demo of how to create a Restful Web Api, C(reate)R(ead)U(pdate)D(delete) operations
    /// </summary>
    public class SchoolController : ApiController
    {
        /// <summary>
        /// GET == Read : /api/school/
        /// </summary>
        /// <returns>All students</returns>
        public IEnumerable<WebApiDemo.UIModels.Student> Get()
        {
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
                return students;
            }
        }

        /// <summary>
        /// GET == Read : api/school/2
        /// </summary>
        /// <param name="id">Student Id</param>
        /// <returns></returns>
        public WebApiDemo.UIModels.Student Get(int id)
        {
            if (id == 0)
            {
                return null;
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
                return student;
            }
        }

        // POST api/values
        /// <summary>
        ///  POST == Create
        /// </summary>
        /// <param name="student"></param>
        public void Post([FromBody] WebApiDemo.UIModels.Student student)
        {
            using (SchoolContext context = new SchoolContext())
            {
                bool isExist = (from s in context.Students where s.StudentID == student.StudentID select s).Count<Student>() > 0;

                var record = context.Students.FirstOrDefault(s => s.StudentID == student.StudentID);
                record.StudentName = student.StudentName;
                record.DateOfBirth = student.DateOfBirth;
                record.Photo = student.Photo;
                record.GenderId = (int)student.GenderId;

                context.SaveChanges();
                return;
            }
        }

        /// <summary>
        /// PUT == Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="student"></param>
        public void Put(int id, [FromBody] WebApiDemo.UIModels.Student student)
        {
            using (SchoolContext context = new SchoolContext())
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
        }

        /// <summary>
        /// DELETE = Delete
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            using (SchoolContext context = new SchoolContext())
            {
                var record = context.Students.FirstOrDefault(s => s.StudentID == id);
                if (record != null)
                {
                    context.Students.Remove(record);
                    context.SaveChanges();
                }
            }
        }
    }
}
