using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApiDemo.CustomValidator
{
    public class EnumRequired : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            return base.IsValid(value) && (int)value > 0;
        }
    }
}