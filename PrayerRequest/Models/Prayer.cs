using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PrayerRequest.Models
{
    public class Prayer : IEquatable<Prayer>
    {

        [Required]

        [EnumDataType(typeof(Repository.Catagorie))]

        [DisplayName("Catagory")]
        public Repository.Catagorie? Catagorie { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [DisplayName("Short Description")]
        public string ShortDescription { get; set; }
        [Required]
        [DisplayName("Expiration Date (Date prayer will be deleted)")]
        [DataType(DataType.DateTime)]
        public DateTime Expiration { get; set; }
        [DisplayName("Post Anonymously (defaults to NO)")]
        public bool PostAnonymous { get; set; }
        public string UserID { get; set; }
        [DisplayName("Long Description")]
        [DataType(DataType.MultilineText)]
        public string LongDescription { get; set; }
        [Required]
        [EnumDataType(typeof(Repository.States))]
        public Repository.States? State { get; set; }
        public int ID { get; set; }

        public bool Equals(Prayer other)
        {
            return this.ID == other.ID;
        }
    }
}
