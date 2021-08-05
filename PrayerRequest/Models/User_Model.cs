using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace PrayerRequest.Models
{
    public class User_Model
    {
        [DisplayName("Username")]
        public string UserName { get; set; }
        [DisplayName("User ID")]
        public string UserID { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Display Name")]
        public string DisplayName {get;set;}
        [DisplayName("Phone Number")]
        public string phonenumber { get; set; }

    }
}
