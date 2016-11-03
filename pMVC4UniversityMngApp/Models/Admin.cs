using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace pMVC4UniversityMngApp.Models
{
    [Table("Admin")]
    public class Admin
    {
        public int AdminID { set; get; }

        [Display(Name = "Name :-")]
        public string AdminName { set; get; }

        [Required(ErrorMessage = "Error : Email Can Not be Empty !!!")]
        public string Email { set; get; }

        [Required(ErrorMessage = "Error : Password Can Not be Empty !!!")]
        public string Password { set; get; }
        public bool IsActive { set; get; }
    }
}