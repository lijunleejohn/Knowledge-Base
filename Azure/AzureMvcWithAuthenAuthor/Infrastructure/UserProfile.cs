using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureMvcWithAuthenAuthor.Infrastructure
{
    public class UserProfile
    {
        public string TokenID { get; set; }
        public string DisplayName { get; set; }
        public string LoginID { get; set; }
        public string PhotoBase64 { get; set; }
    }
}
