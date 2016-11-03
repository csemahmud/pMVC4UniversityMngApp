using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace pMVC4UniversityMngApp.Models
{
    [Table("Teacher")]
    public class Teacher
    {
        public int TeacherID { set; get; }

        [Required(ErrorMessage = "Error : Teacher Name Can Not be Empty !!!")]
        [Display(Name = "Teacher Name :-")]
        public string TeacherName { set; get; }
        public string Address { set; get; }

        [Required(ErrorMessage = "Error : Email Can Not be Empty !!!")]
        [Remote("Check_Email", "Teachers",
            ErrorMessage = "Error : This Email ID Already Exists !!!")]
        public string Email { set; get; }
        public string ContactNo { set; get; }

        [Display(Name = "Credits To Take :-")]
        public double CreditsToBeTaken { set; get; }

        [Display(Name = "Credits Have Taken :-")]
        public double CreditsHaveTaken { set; get; }
        public virtual Department Department { set; get; }
        public int DepartmentID { set; get; }
        public virtual Designation Designation { set; get; }
        public int DesignationID { set; get; }
        public virtual List<AssignedCourse> AssignedCourseList { set; get; }
        public bool IsActive { set; get; }
    }
}
