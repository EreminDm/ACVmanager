using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using AcmMvcApp.Models;

namespace AcmMvcApp.Controllers
{
    public class ContentDispController : Controller
    {
        private UsersContext db = new UsersContext();

        //
        // GET: /Content/

        public ActionResult Index()
        {
            return View(db.Contents.ToList());
        }

        //
        // GET: /Content/Details/5

        public ActionResult Details(int id = 0)
        {
            ContentDisp content = db.Contents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            return View(content);
        }

        //
        // GET: /Content/Create

        public ActionResult Create()
        {

            ContentViewModel cvm = new ContentViewModel();
            
            return View(cvm);
        }

        //
        // POST: /Content/Create

        [HttpPost]
        public ActionResult Create(ContentViewModel contentVm)
        {
            if (ModelState.IsValid)
            {

                string fileName = "";

                // локальная заливка файла в Temp_content
                if (contentVm.file != null && contentVm.file.ContentLength > 0)
                {
                    fileName = Path.GetFileName(contentVm.file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Temp_content/"), fileName);
                    contentVm.file.SaveAs(path);
                }

                db.Contents.Add(contentVm.content);
                db.SaveChanges();


                return RedirectToAction("Index");
            }

            return View(contentVm.content);
        }

        //
        // GET: /Content/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ContentDisp content = db.Contents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            ContentViewModel cvm = new ContentViewModel();

            cvm.content = content;

            return View(cvm);
        }

        //
        // POST: /Content/Edit/5

        [HttpPost]
        public ActionResult Edit(ContentViewModel contentVm)
        {
            if (ModelState.IsValid)
            {

                string fileName = "";

                // локальная заливка файла в Temp_content
                if (contentVm.file != null && contentVm.file.ContentLength > 0)
                {
                    fileName = Path.GetFileName(contentVm.file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Temp_content/"), fileName);
                    contentVm.file.SaveAs(path);
                }

                if (contentVm.file != null && contentVm.file.ContentLength > 0)
                {
                    int indexOfDot = fileName.IndexOf('.');
                    contentVm.content.name = fileName.Substring(0, indexOfDot);
                    contentVm.content.extension = fileName.Substring(indexOfDot, fileName.Length - indexOfDot);
                }

                db.Entry(contentVm.content).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contentVm.content);
        }

        //
        // GET: /Content/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ContentDisp content = db.Contents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            return View(content);
        }

        //
        // POST: /Content/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ContentDisp content = db.Contents.Find(id);
            db.Contents.Remove(content);
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