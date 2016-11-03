using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pMVC4UniversityMngApp.Models;
using System.Web.Helpers;

namespace pMVC4UniversityMngApp.Controllers
{
    public class AssignedCoursesController : Controller
    {
        private RootProjDBContext db = new RootProjDBContext();

        public ActionResult UnAuthorizedAccess()
        {
            ViewBag.Message = "You Are Not Authorized to View This";
            return View("~/Views/Shared/UnAuthorizedAccess.cshtml");
        }

        //
        // GET: /AssignedCourses/

        public ActionResult Index()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            ViewBag.DepartmentID = new SelectList(db.DepartmentDbSet, "DepartmentID", "DeptCode");
            var assignedcoursedbset = db.AssignedCourseDbSet.Include(a => a.Teacher).Include(a => a.Course).Include(a => a.Course.Semester).OrderBy(a => a.Course.CourseCode).Where(a =>  !a.IsOutDated);
            return View(assignedcoursedbset.ToList());
        }

        public PartialViewResult CourseFilteredList(int? departmentID)
        {
            List<AssignedCourse> assignedCourseModel;
            if (departmentID != null)
            {
                assignedCourseModel = db.AssignedCourseDbSet.Include(a => a.Teacher).Include(a => a.Course).Include(a => a.Course.Semester).OrderBy(a => a.Course.CourseCode).Where(a => (a.Course.DepartmentID == departmentID && !a.IsOutDated)).ToList();
            }
            else
            {
                assignedCourseModel = db.AssignedCourseDbSet.Include(a => a.Teacher).Include(a => a.Course).Include(a => a.Course.Semester).OrderBy(a => a.Course.CourseCode).Where(a => !a.IsOutDated).ToList();
            }
            return PartialView("~/Views/AssignedCourses/_CourseFilteredList.cshtml", assignedCourseModel);
        }

        class Result
        {
            public List<object> assignedCourses;
        };

        //
        // GET: /AssignedCourses/JsonData

        public JsonResult JsonData()
        {
            if (Session["Email"] == null)
            {
                string asd = null;
                return Json(asd, JsonRequestBehavior.DenyGet);
            }
            var assignedCourses =
                from assignedCourse in db.AssignedCourseDbSet.Include(a => a.Teacher).Include(a => a.Course).Include(a => a.Course.Semester).Include(a => a.Course.Department).Where(a => !a.IsOutDated).ToList()
                select new
                {
                    assignedCourse.AssignedCourseID,
                    assignedCourse.Course.CourseCode,
                    assignedCourse.Course.CourseName,
                    assignedCourse.Teacher.TeacherName,
                    assignedCourse.Course.Semester.SemesterName,
                    assignedCourse.Course.Department.DepartmentID,
                    assignedCourse.Course.Department.DeptCode,
                    assignedCourse.IsValid,
                    assignedCourse.IsAssigned
                };

            Result result = new Result();
            result.assignedCourses = new List<object>();
            result.assignedCourses.AddRange(assignedCourses);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AngularJs()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            return View();
        }

        //
        // GET: /AssignedCourses/Create

        public ActionResult Create()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            ViewBag.DepartmentID = new SelectList(db.DepartmentDbSet, "DepartmentID", "DeptCode");
            ViewBag.TeacherID = new SelectList("", "TeacherID", "TeacherName");
            ViewBag.CourseID = new SelectList("", "CourseID", "CourseCode");
            return View();
        }

        //
        // POST: /AssignedCourses/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AssignedCourse assignedcourse)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            if (ModelState.IsValid)
            {
                if (assignedcourse.TeacherID != 0 && assignedcourse.CourseID != 0)
                {
                    AssignedCourse anAssignedCourse = db.AssignedCourseDbSet.FirstOrDefault(a => (a.CourseID == assignedcourse.CourseID && !a.IsOutDated));
                    Teacher aTeacher = db.TeacherDbSet.Find(assignedcourse.TeacherID);
                    Course aCourse = db.CourseDbSet.Find(assignedcourse.CourseID);

                    if(anAssignedCourse != null && aTeacher != null && aCourse != null)
                    {
                        if (anAssignedCourse.IsValid)
                        {
                            if (!anAssignedCourse.IsAssigned)
                            {
                                anAssignedCourse.IsAssigned = true;
                                anAssignedCourse.TeacherID = assignedcourse.TeacherID;
                                db.Entry(anAssignedCourse).State = EntityState.Modified;
                                if (db.SaveChanges() > 0)
                                {
                                    aTeacher.CreditsHaveTaken += aCourse.Credit;
                                    db.Entry(aTeacher).State = EntityState.Modified;
                                    if (db.SaveChanges() > 0)
                                    {
                                        ViewBag.Message = "The Course :- " + aCourse.CourseCode
                                            + " has been assigned to Teacher :- "
                                            + aTeacher.TeacherName + " successfully .";
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Error : The Course :- "
                                        + aCourse.CourseCode
                                        + " has already been assigned";
                                Teacher asndTeacher = db.TeacherDbSet.FirstOrDefault(t => t.TeacherID == anAssignedCourse.TeacherID);
                                if (asndTeacher != null)
                                {
                                    ViewBag.Message += " to Teacher :- "
                                        + asndTeacher.TeacherName + " !!!";
                                }
                            }
                        }
                        else
                        {
                            anAssignedCourse.IsOutDated = true;
                            db.Entry(anAssignedCourse).State = EntityState.Modified;
                            db.SaveChanges();
                            assignedcourse.IsAssigned = true;
                            assignedcourse.IsValid = true;
                            assignedcourse.IsOutDated = false;
                            db.AssignedCourseDbSet.Add(assignedcourse);
                            if (db.SaveChanges() > 0)
                            {
                                aTeacher.CreditsHaveTaken += aCourse.Credit;
                                db.Entry(aTeacher).State = EntityState.Modified;
                                if (db.SaveChanges() > 0)
                                {
                                    ViewBag.Message = "The Course :- " + aCourse.CourseCode
                                        + " has been assigned to Teacher :- "
                                        + aTeacher.TeacherName + " successfully .";
                                }
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "All the fields are required";
                }
            }

            ViewBag.DepartmentID = new SelectList(db.DepartmentDbSet, "DepartmentID", "DeptCode");
            ViewBag.TeacherID = new SelectList("", "TeacherID", "TeacherName");
            ViewBag.CourseID = new SelectList("", "CourseID", "CourseCode");
            return View();
        }

        public PartialViewResult LoadTeacherDropDown(int? departmentID)
        {
            if (departmentID != null) 
            {
                ViewBag.TeacherID = new SelectList(db.TeacherDbSet.Where(t => (t.DepartmentID == departmentID && t.IsActive)), "TeacherID", "TeacherName");
            }
            else
            {
                ViewBag.TeacherID = new SelectList("", "TeacherID", "TeacherName");
            }
            return PartialView("~/Views/AssignedCourses/_LoadTeacherDropDown.cshtml");
        }

        public ActionResult TeacherInfo(int? teacherID) {
            Teacher teacherModel = null;
            if (teacherID != null)
            {
                teacherModel = db.TeacherDbSet.FirstOrDefault(t => t.TeacherID == teacherID);
            }
            return PartialView("~/Views/AssignedCourses/_TeacherInfo.cshtml", teacherModel);
        }

        public PartialViewResult LoadCourseDropDown(int? departmentID)
        {
            if (departmentID != null)
            {
                ViewBag.CourseID = new SelectList(db.CourseDbSet.Where(c => (c.DepartmentID == departmentID && c.IsValid)), "CourseID", "CourseCode");
            }
            else
            {
                ViewBag.CourseID = new SelectList("", "CourseID", "CourseCode");
            }
            return PartialView("~/Views/AssignedCourses/_LoadCourseDropDown.cshtml");
        }

        public ActionResult CourseInfo(int? courseID)
        {
            Course CourseModel = null;
            if (courseID != null)
            {
                CourseModel = db.CourseDbSet.FirstOrDefault(c => c.CourseID == courseID);
            }
            return PartialView("~/Views/AssignedCourses/_CourseInfo.cshtml", CourseModel);
        }

        //
        // GET: /AssignedCourses/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            AssignedCourse assignedcourse = db.AssignedCourseDbSet.Find(id);
            if (assignedcourse == null)
            {
                return HttpNotFound();
            }
            return View(assignedcourse);
        }

        //
        // POST: /AssignedCourses/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            AssignedCourse assignedcourse = db.AssignedCourseDbSet.Find(id);
            Teacher teacher = db.TeacherDbSet.Find(assignedcourse.TeacherID);
            Course course = db.CourseDbSet.Find(assignedcourse.CourseID);
            teacher.CreditsHaveTaken -= course.Credit;
            db.Entry(teacher).State = EntityState.Modified;
            db.SaveChanges();
            assignedcourse.IsAssigned = false;
            assignedcourse.IsValid = false;
            db.Entry(assignedcourse).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UnAssignAllCourses()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            ViewBag.Massage = "Are You Sure You Want to Unassign All Courses ?";
            return View();
        }

        public ActionResult UnAssignConfirmed()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            List<AssignedCourse> assignedCourseList = db.AssignedCourseDbSet.Include(a => a.Teacher).Where(a => (a.IsValid && a.IsAssigned && !a.IsOutDated)).ToList();
            bool IsSuccess = true;
            foreach (var assignedCourse in assignedCourseList)
            {
                assignedCourse.Teacher.CreditsHaveTaken = 0.0000;
                db.Entry(assignedCourse.Teacher).State = EntityState.Modified;
                db.SaveChanges();
                assignedCourse.IsValid = false;
                assignedCourse.IsAssigned = false;
                db.Entry(assignedCourse).State = EntityState.Modified;
                if (db.SaveChanges() <= 0)
                {
                    IsSuccess = false;
                }
            }
            if (IsSuccess)
            {
                ViewBag.Message = "All the courses have been unassigned successfully .";
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}