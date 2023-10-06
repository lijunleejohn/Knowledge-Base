using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace WebApiDemo.Models
{
    public enum GenderEnum { 
        [Description("Human has XY chromosome")]
        Male = 1, 

        [Description("Human has XX chromosome")]
        Female = 2 
    }
}