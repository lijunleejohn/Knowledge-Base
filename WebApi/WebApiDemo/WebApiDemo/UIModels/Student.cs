using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WebApiDemo.CustomValidator;

namespace WebApiDemo.UIModels
{
    public class Student
    {
        public int? StudentID { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string StudentName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        ///<summary>GenderId is an Enum</summary>
        [EnumRequired]
        public WebApiDemo.Models.GenderEnum GenderId { get; set; }

        public byte[] Photo { get; set; }
    }
}