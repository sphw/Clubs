using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Clubs.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public IList<TripUser> TripUsers { get; set; }
        public IList<string> Qualifications { get; set; }
    }

}