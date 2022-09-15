using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LibraryWebProject.Models
{
    public class AuthorModel
    {
        public AuthorModel(){
            AuthorBooks = new List<BookModel>();
        }
        [Key]
        [Display (Name ="Author ID")]
        public int AuthorID { get; set; }
        [Display (Name ="Author Name")]
        // [Required (ErrorMessage ="Please Enter Your Name...")]
        public string AuthorName { get; set; }
        [Display (Name ="Author Surname")]
        // [Required (ErrorMessage ="Please Enter Your Surname...")]
        public string AuthorSurname { get; set; }
        [Display (Name ="Author Birth Date")]
        [Required (ErrorMessage = "Please Enter Your Birth Date...")]
        public DateTime AuthorBirthDate { get; set; }
        [Display (Name ="Author Death Date")]
        public DateTime AuthorDeathDate { get; set; }
        [Display (Name ="Author Books")]
        public List<BookModel> AuthorBooks { get; set; }
        [NotMapped]
        public IFormFile[] BookPhotoFiles { get; set; }

        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime updatedTime { get; set; }
        public bool isDeleted { get; set; }
    }
}