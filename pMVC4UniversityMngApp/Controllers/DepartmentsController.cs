using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pMVC4UniversityMngApp.Models;

namespace pMVC4UniversityMngApp.Controllers
{
    public class DepartmentsController : Controller
    {
        private RootProjDBContext db = new RootProjDBContext();

        public ActionResult UnAuthorizedAccess()
        {
            ViewBag.Message = "You Are Not Authorized to View This";
            return View("~/Views/Shared/UnAuthorizedAccess.cshtml");
        }

        //
        // GET: /Departments/

        public ActionResult Index()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            return View(db.DepartmentDbSet.ToList());
        }

        //
        // GET: /Departments/Details/5

        public ActionResult Details(int id = 0)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            Department department = db.DepartmentDbSet.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // GET: /Departments/Create

        public ActionResult Create()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            return View();
        }

        //
        // POST: /Departments/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            if (ModelState.IsValid)
            {
                db.DepartmentDbSet.Add(department);
                if (db.SaveChanges() > 0) {
                    ViewBag.Message = "Department :- " + department.DeptCode
                        + " has been saved successfully .";
                }
            }

            return View(department);
        }

        public JsonResult Check_DeptCode(string deptCode)
        {
            var result = db.DepartmentDbSet.Count(d => d.DeptCode == deptCode) == 0;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Check_DeptName(string deptName)
        {
            var result = db.DepartmentDbSet.Count(d => d.DeptName == deptName) == 0;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Departments/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            Department department = db.DepartmentDbSet.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // POST: /Departments/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Department department)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            if (ModelState.IsValid)
            {
                Department chekDept1 = db.DepartmentDbSet.FirstOrDefault(d => (d.DeptCode == department.DeptCode && d.DepartmentID != department.DepartmentID));
                if (chekDept1 != null) 
                {
                    ViewBag.Message = "Department Code : " + chekDept1.DeptCode + " Already Exists !!!";
                    return View(department);
                }
                Department chekDept2 = db.DepartmentDbSet.FirstOrDefault(d => (d.DeptName == department.DeptName && d.DepartmentID != department.DepartmentID));
                if (chekDept2 != null) 
                {
                    ViewBag.Message = "Department Name : " + chekDept2.DeptName + " Already Exists !!!";
                    return View(department);
                }
                db.Entry(department).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    ViewBag.Message = "Department :- " + department.DeptCode
                        + " has been updated successfully .";
                }
            }
            return View(department);
        }

        //
        // GET: /Departments/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            Department department = db.DepartmentDbSet.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // POST: /Departments/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("UnAuthorizedAccess");
            }
            Department department = db.DepartmentDbSet.Find(id);
            List<Teacher> TeacherList = db.TeacherDbSet.Where(t => t.DepartmentID == id).ToList();
            foreach (var teacher in TeacherList)
            {
                db.TeacherDbSet.Remove(teacher);
            }
            db.SaveChanges();
            List<Student> StudentList = db.StudentDbSet.Where(t => t.DepartmentID == id).ToList();
            foreach (var Student in StudentList)
            {
                db.StudentDbSet.Remove(Student);
            }
            db.SaveChanges();
            db.DepartmentDbSet.Remove(department);
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