using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PrayerRequest.Models
{
    public class Group
    {
        [Required]
        [DisplayName("Require Password")]
        public bool GroupOpen { get; set; }

        [PasswordPropertyText(true)]
        [RegularExpression(@"^.*(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*\(\)_\-+=]).*$", ErrorMessage = "Password must include 1 digit, 1 uppercase leter and 1 special character")]
        [StringLength(40,MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [Required]
        public string GroupPassword { get; set; }
        [PasswordPropertyText(true)]
        [Compare("GroupPassword", ErrorMessage = "Passwords dont match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [DisplayName("Group Name")]
        public string GroupName { get; set; }
        [Required]
        [DisplayName("Group Description")]
        [DataType(DataType.MultilineText)]
        public string GroupDescription { get; set; }

        [DisplayName("Group Location")]
        [DataType(DataType.MultilineText)]
        public string GroupLocation { get; set; }

        [DisplayName("Contact Information")]
        [DataType(DataType.MultilineText)]
        public string GroupContact { get; set; }
        public string OwnerID { get; set; }
        [Required]
        [EnumDataType(typeof(Repository.States))]
        public int State { get; set; }
        public int ID { get; set; }
    }
}
