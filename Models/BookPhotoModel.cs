using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebProject.Models
{
    public class BookPhotoModel
    {
        [Key]
        public int PhotoID { get; set; }
        public BookModel Book { get; set; }
        public string PhotoName { get; set; }
        public int BookID { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime updatedTime { get; set; }
        public bool isDeleted { get; set; }
    }
}