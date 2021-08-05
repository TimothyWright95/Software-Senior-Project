using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PrayerRequest.Models
{
    public class Bulitin
    {
        public int GroupID { get; set; }
        public DateTime DatePosted { get; set; }
        [Required]
        [DisplayName("Message")]
        public string Bulitinmessage { get; set; }
        [Required]
        public string EventTitle { get; set; }
        public int ID { get; set; }
       
    }
}
