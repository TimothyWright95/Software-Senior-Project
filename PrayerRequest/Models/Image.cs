using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PrayerRequest.Models
{
    public class Image
    {
        public string ImageTitle { get; set; }
        [Required]
        public byte[] ImageData { get; set; }
    }
}
