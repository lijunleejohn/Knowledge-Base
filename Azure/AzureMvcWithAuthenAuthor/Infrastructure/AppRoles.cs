using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureMvcWithAuthenAuthor.Infrastructure
{
    /// <summary>
    /// Contains a list of all the Azure AD app roles this app depends on and works with.
    /// </summary>
    public static class AppRole
    {
        /// <summary>
        /// Readers can read only.
        /// </summary>
        public const string Readers = "Readers";

        /// <summary>
        /// Writers can read and write.
        /// </summary>
        public const string Writers = "Writers";


        /// <summary>
        /// Admins can see the application info
        /// </summary>
        public const string Admin = "Admins";
    }

    /// <summary>
    /// Wrapper class the contain all the authorization policies available in this application.
    /// </summary>
    public static class AuthorizationPolicies
    {
        public const string ReadersRole = "ReadersRole";
        public const string WritersRole = "WritersRole";
        public const string AdminsRole = "AdminsRole"; 
    }
}
