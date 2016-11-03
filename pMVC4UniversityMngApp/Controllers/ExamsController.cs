using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pMVC4UniversityMngApp.Models;
using RazorPDF;
using iTextSharp;
using iTextSharp.text;

namespace pMVC4UniversityMngApp.Controllers
{
    public class ExamsController : Controller
    {
        private RootProjDBContext db = new RootProjDBContext();

        public ActionResult UnAuthorizedAccess()
        {
            ViewBag.Message = "You Are Not Authorized to View This";
            return View("~/Views/Shared/UnAuthorizedAccess.cshtml");
        }

        //
        // GET: /Exams/

        public ActionResult Index()
        {
            ViewBag.StudentID = new SelectList(db.StudentDbSet.Where(s => s.IsActive), "StudentID", "RegNo");
            return View(new List<Exam>());
        }

        public PartialViewResult ResultFilteredList(int? studentID)
        {
            List<Exam> examModel = null;
            if (studentID != null)
            {
                examModel = db.ExamDbSet.Include(e => e.Course).OrderBy(e => e.Course.CourseCode).Where(e => (e.StudentID == studentID && e.IsValid)).ToList();
                if (examModel.Count < 1)
                {
                    Student aStudent = db.StudentDbSet.Find(studentID);
                    ViewBag.NotEnrolled = "Student : " + aStudent.StudentName
                        + " with Registration No. " + aStudent.RegNo
                        + " has not Enrolled any course in this semester";
                }
            }
            else
            {
                examModel = new List<Exam>();
            }
            return PartialView("~/Views/Exams/_ResultFilteredList.cshtml", examModel);
        }

        [HttpPost]
        public ActionResult ViewPDFResult(int studentID)
        {
            return new PdfResult(db.ExamDbSet.Where(e => (e.StudentID == studentID && e.IsValid)).ToList(),"PDFResultRPT");
        }

        class Result
        {
            public List<object> students;
        };

        //
        // GET: /AssignedCourses/JsonData

        public JsonResult JsonData()
        {

            var students =
                from student in db.StudentDbSet.Include(s => s.Department).Where(s => s.IsActive).ToList()
                select new
                {
                    student.StudentID,
                    student.StudentName,
                    student.RegNo,
                    student.Email,
                    student.Department.DeptName,
                    Exams = from exam in db.ExamDbSet.Include(e => e.Course).Include(e => e.Grade).Where(e => (e.StudentID == student.StudentID && e.IsValid)).ToList()
                            select new 
                            {
                                exam.ExamID,
                                exam.Course.CourseCode,
                                exam.Course.CourseName,
                                EnrollmentDate = exam.EnrollmentDate.ToLongDateString(),
                                exam.Grade.GradeLetter,
                                exam.IsGradeSubmitted
                            }
                };

            Result result = new Result();
            result.students = new List<object>();
            result.students.AddRange(students);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AngularJs()
        {
            return View();
        }

        //
        // GET: /Exams/Details/5

        public ActionResult Details(int id = 0)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            return RedirectToAction("UnAuthorizedAccess");
        }

        //
        // GET: /Exams/Create

        public ActionResult Create()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            ViewBag.StudentID = new SelectList(db.StudentDbSet.Where(s => s.IsActive), "StudentID", "RegNo");
            ViewBag.CourseID = new SelectList("", "CourseID", "CourseCode");
            return View();
        }

        //
        // POST: /Exams/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Exam exam)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            if (ModelState.IsValid)
            {
                Exam checkExam = db.ExamDbSet.FirstOrDefault(e => (e.StudentID == exam.StudentID && e.CourseID == exam.CourseID && e.IsValid));
                Course aCourse = db.CourseDbSet.Find(exam.CourseID);
                Student aStudent = db.StudentDbSet.Find(exam.StudentID);
                if (checkExam == null)
                {
                    exam.GradeID = 1;
                    exam.IsGradeSubmitted = false;
                    exam.IsValid = true;
                    db.ExamDbSet.Add(exam);
                    if (db.SaveChanges() > 0)
                    {
                        ViewBag.Message = "This Course : " + aCourse.CourseCode
                            + " has been successfully enrolled to student "
                            + aStudent.StudentName;
                    }
                }
                else
                {
                    ViewBag.Message = "This Course : " + aCourse.CourseCode
                            + " is already enrolled to student "
                            + aStudent.StudentName;
                }
            }

            ViewBag.StudentID = new SelectList(db.StudentDbSet.Where(s => s.IsActive), "StudentID", "RegNo");
            ViewBag.CourseID = new SelectList("", "CourseID", "CourseCode");
            return View();
        }

        public ActionResult StudentInfo(int? studentID)
        {
            Student StudentModel = null;
            if (studentID != null)
            {
                StudentModel = db.StudentDbSet.FirstOrDefault(s => s.StudentID == studentID);
            }
            return PartialView("~/Views/Exams/_StudentInfo.cshtml", StudentModel);
        }

        public ActionResult ResultStudentInfo(int? studentID)
        {
            List<Exam> ExamModel = null;
            if (studentID != null)
            {
                ExamModel = db.ExamDbSet.Include(e => e.Student).Include(e => e.Student.Department).Where(e => e.StudentID == studentID).ToList();
                if (ExamModel.Count() < 1)
                {
                    Student aStudent = db.StudentDbSet.Include(s => s.Department).FirstOrDefault(s => s.StudentID == studentID);
                    ViewBag.StudentName = aStudent.StudentName;
                    ViewBag.Email = aStudent.Email;
                    ViewBag.DeptName = aStudent.Department.DeptName;
                }
            }
            return PartialView("~/Views/Exams/_ResultStudentInfo.cshtml", ExamModel);
        }

        public PartialViewResult LoadCourseByDeptStudent(int? studentID)
        {
            if (studentID != null)
            {
                Student student = db.StudentDbSet.FirstOrDefault(s => s.StudentID == studentID);
                ViewBag.CourseID = new SelectList(db.CourseDbSet.Where(c => (c.DepartmentID == student.DepartmentID && c.IsValid)), "CourseID", "CourseCode");
            }
            else
            {
                ViewBag.CourseID = new SelectList("", "CourseID", "CourseCode");
            }
            return PartialView("~/Views/Exams/_LoadCourseByDeptStudent.cshtml");
        }

        public PartialViewResult LoadCourseByEnrollment(int? studentID)
        {
            if (studentID != null)
            {
                List<Exam> examList = db.ExamDbSet.Where(e => e.StudentID == studentID && e.IsValid).ToList();
                List<Course> courseList = new List<Course>();

                foreach (Exam exam in examList)
                {
                    Course course = db.CourseDbSet.FirstOrDefault(c => c.CourseID == exam.CourseID && c.IsValid);
                    if (course != null)
                    {
                        courseList.Add(course);
                    }
                }
                ViewBag.CourseID = new SelectList(courseList, "CourseID", "CourseCode");
            }
            else
            {
                ViewBag.CourseID = new SelectList("", "CourseID", "CourseCode");
            }
            return PartialView("~/Views/Exams/_LoadCourseByEnrollment.cshtml");
        }

        //
        // GET: /Exams/ResultEntry/5

        public ActionResult ResultEntry()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            ViewBag.StudentID = new SelectList(db.StudentDbSet.Where(s => s.IsActive), "StudentID", "RegNo");
            ViewBag.CourseID = new SelectList("", "CourseID", "CourseCode");
            ViewBag.GradeID = new SelectList(db.GradeDbSet.Where(g => g.GradeID != 1), "GradeID", "GradeLetter");
            return View();
        }

        //
        // POST: /Exams/ResultEntry/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResultEntry(Exam exam)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            if (ModelState.IsValid)
            {
                Exam anExam = db.ExamDbSet.Include(e => e.Student).Include(e => e.Course).FirstOrDefault(e => (e.StudentID == exam.StudentID && e.CourseID == exam.CourseID && e.IsValid));
                if (anExam != null)
                {
                    if (!anExam.IsGradeSubmitted)
                    {
                        anExam.GradeID = exam.GradeID;
                        anExam.IsGradeSubmitted = true;
                        db.Entry(anExam).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            ViewBag.Message = "Result for this course "
                                + anExam.Course.CourseCode
                                + " has successfully published for this student "
                                + anExam.Student.StudentName;
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error : Result for this course "
                                + anExam.Course.CourseCode
                                + " is already published for this student "
                                + anExam.Student.StudentName;
                    }
                }
            }
            ViewBag.StudentID = new SelectList(db.StudentDbSet.Where(s => s.IsActive), "StudentID", "RegNo");
            ViewBag.CourseID = new SelectList("", "CourseID", "CourseCode");
            ViewBag.GradeID = new SelectList(db.GradeDbSet.Where(g => g.GradeID != 1), "GradeID", "GradeLetter");
            return View();
        }

        //
        // GET: /Exams/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            Exam exam = db.ExamDbSet.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        //
        // POST: /Exams/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            Exam exam = db.ExamDbSet.Find(id);
            exam.IsValid = false;
            db.Entry(exam).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}