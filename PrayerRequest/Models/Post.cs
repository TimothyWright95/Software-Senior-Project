using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PrayerRequest.Models
{
    public class Post
    {
        public string OwnerID { get; set; }
        [Required]
        [DisplayName("Message")]
        public string Postmessage { get; set; }
        public int GroupID { get; set; }
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string DisplayName { get; set; }
    }
}
