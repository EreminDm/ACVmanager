using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AcmMvcApp.Models;

namespace AcmMvcApp.Controllers
{
    public class DisplayController : Controller
    {
        private UsersContext db = new UsersContext();

        //
        // GET: /Display/

        public ActionResult Index()
        {
            return View(db.Displays.ToList());
        }

        //
        // GET: /Display/Details/5

        public ActionResult Details(int id = 0)
        {
            Display display = db.Displays.Find(id);
            if (display == null)
            {
                return HttpNotFound();
            }
            return View(display);
        }

        //
        // GET: /Display/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Display/Create

        [HttpPost]
        public ActionResult Create(Display display)
        {
            if (ModelState.IsValid)
            {
                db.Displays.Add(display);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(display);
        }

        //
        // GET: /Display/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Display display = db.Displays.Find(id);
            if (display == null)
            {
                return HttpNotFound();
            }
            return View(display);
        }

        //
        // POST: /Display/Edit/5

        [HttpPost]
        public ActionResult Edit(Display display)
        {
            if (ModelState.IsValid)
            {
                db.Entry(display).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(display);
        }

        //
        // GET: /Display/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Display display = db.Displays.Find(id);
            if (display == null)
            {
                return HttpNotFound();
            }
            return View(display);
        }

        //
        // POST: /Display/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Display display = db.Displays.Find(id);
            db.Displays.Remove(display);
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