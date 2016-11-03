using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace pMVC4UniversityMngApp.Models
{
    [Table("Department")]
    public class Department
    {
        public int DepartmentID { set; get; }

        [Required(ErrorMessage = "Error : Department Code Can Not be Empty !!!")]
        [Remote("Check_DeptCode", "Departments", 
            ErrorMessage = "Error : This Department Code Already Exists !!!")]
        [Display(Name = "Department Code :-")]
        public string DeptCode { set; get; }

        [Required(ErrorMessage = "Error : Department Name Can Not be Empty !!!")]
        [Remote("Check_DeptName", "Departments",
            ErrorMessage = "Error : This Department Name Already Exists !!!")]
        [Display(Name = "Department Name :-")]
        public string DeptName { set; get; }

        public virtual List<Teacher> TeacherList { set; get; }
        public virtual List<Course> CourseList { set; get; }
        public virtual List<Student> StudentList { set; get; }
    }
}