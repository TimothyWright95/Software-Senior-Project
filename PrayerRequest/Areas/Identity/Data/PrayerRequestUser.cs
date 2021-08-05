using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PrayerRequest.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the PrayerRequestUser class
    public class PrayerRequestUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}
