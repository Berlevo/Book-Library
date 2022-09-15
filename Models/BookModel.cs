using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebProject.Models
{
    public class BookModel
    {
        public BookModel(){
            this.BookPhotos = new List<BookPhotoModel>();
        }
        [Key]
        public int BookID { get; set; }
        public int AuthorID { get; set; }
        public AuthorModel Author { get; set; }
        [Display (Name ="Book Name")]
        [Required (ErrorMessage = "Please Enter Book Name...")]
        public string BookName { get; set; }
        [Display (Name ="Page Number")]
        [Required (ErrorMessage ="Please Enter Your Page Number...")]
        public int PageNumber { get; set; }
        [Display (Name = "Price")]
        [Required (ErrorMessage ="Please Enter Price...")]
        public int Price { get; set; }
        [Display (Name = "Book Photos")]
        public List<BookPhotoModel> BookPhotos { get; set; }
        [Display (Name ="Publish Date")]
        public DateTime PublishDate { get; set; }
        [Display (Name ="Publisher")]
        public string Publisher { get; set; }
        [Display (Name ="Book Information")]
        [Required (ErrorMessage ="Please Enter Book Information")]
        public string BookInformation { get; set; }

        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime updatedTime { get; set; }
        public bool isDeleted { get; set; }

    }
}