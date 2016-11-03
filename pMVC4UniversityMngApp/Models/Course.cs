using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace pMVC4UniversityMngApp.Models
{
    [Table("Course")]
    public class Course
    {
        public int CourseID { set; get; }

        [Required(ErrorMessage = "Error : Course Code Can Not be Empty !!!")]
        [Remote("Check_CourseCode", "Courses",
            ErrorMessage = "Error : This Course Code Already Exists !!!")]
        [Display(Name = "Course Code :-")]
        public string CourseCode { set; get; }

        [Required(ErrorMessage = "Error : Course Name Can Not be Empty !!!")]
        [Remote("Check_CourseName", "Courses",
            ErrorMessage = "Error : This Course Name Already Exists !!!")]
        [Display(Name = "Name :-")]
        public string CourseName { set; get; }
        public double Credit { set; get; }
        public string Description { set; get; }
        public virtual Department Department { set; get; }
        public int DepartmentID { set; get; }
        public virtual Semester Semester { set; get; }
        public int SemesterID { set; get; }
        public virtual List<AssignedCourse> AssignedCourseList { set; get; }
        public virtual List<AllocatedRoom> AllocatedRoomList { set; get; }
        public virtual List<Exam> ExamList { set; get; }
        public bool IsValid { set; get; }
    }
}
