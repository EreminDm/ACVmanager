using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using AcmMvcApp.Models;
using PagedList;

namespace AcmMvcApp.Controllers
{
    public class PackageController : Controller
    {
        private UsersContext db = new UsersContext();

        //
        // GET: /Package/

        public ActionResult Index()
        {

            return View(db.Packages.ToList());
        }

        //
        // GET: /Package/Details/5

        public ActionResult Details(int id = 0)
        {
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        //
        // GET: /Package/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Package/Create

        [HttpPost]
        public ActionResult Create(Package package)
        {
            if (ModelState.IsValid)
            {
                db.Packages.Add(package);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(package);
        }


        //
        // GET: /Package/Edit/5

        public ActionResult Edit(int id)
        {
            ViewBag.Title = "Edit";
            Package package = db.Packages.Find(id);

            PackageViewModel viewModel = new PackageViewModel();
            viewModel.package = package;
            viewModel._clients =  db.Clients.ToList();
            viewModel._contents = db.Contents.ToList();
            viewModel._displays = db.Displays.ToList();

            //Call Create View
            return View(viewModel);
        }


        [HttpPost]
        public JsonResult Edit(Package package)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    
                // удаляем все че есть в базе
                foreach (u_package_u_client pkg_cl in db.Pkg_client_links.Where(p => p.PackageId == package.Id))
                    db.Pkg_client_links.Remove(pkg_cl);
                foreach (u_package_u_display pkg_disp in db.Pkg_disp_links.Where(p => p.PackageId == package.Id))
                    db.Pkg_disp_links.Remove(pkg_disp);
                foreach (u_package_u_content pkg_cont in db.Pkg_cont_links.Where(p => p.PackageId == package.Id))
                    db.Pkg_cont_links.Remove(pkg_cont);

                // заливаем всем, что пришло в сущности
                try
                {
                    foreach (u_package_u_client pkg_cl in package.Clients)
                        db.Pkg_client_links.Add(pkg_cl);
                }
                catch (NullReferenceException e) { }
                try 
                {
                    foreach (u_package_u_display pkg_disp in package.Displays)
                        db.Pkg_disp_links.Add(pkg_disp);
                }
                catch (NullReferenceException e) { }
                try
                {
                    foreach (u_package_u_content pkg_cont in package.Contents)
                        db.Pkg_cont_links.Add(pkg_cont);
                }
                catch (NullReferenceException e) { }

                db.Entry(package).State = EntityState.Modified;

                db.SaveChanges();

                // заливка на ftp
                try
                {

                    //MyFtp ftp = new MyFtp(@"ftp://127.0.0.1/", "user_acm", "user_acm");
                    MyFtp ftp = new MyFtp(@"ftp://185.98.7.121/", "rtlsnet_kz", "p@ssw0rd");
                    ContentDisp content;
                    String remotePath;
                    String localPath;
                    String jsonConfig = "[";
                    String fileHash = "";
                    String tmpContentRoot = Server.MapPath("~/Temp_content/");

                    ftp.deleteDirectory("content/" + package.urlName);
                    ftp.createDirectory("content/" + package.urlName);

                    // льем контент на ftp, формируем актуальный config.json
                    foreach (u_package_u_content pkg_cont in package.Contents)
                    {
                        content = db.Contents.Find(pkg_cont.ContentId);
                        //"etc/test.txt", @"C:\Users\metastruct\Desktop\test.txt"

                        remotePath = "content/" + package.urlName + "/" + content.name + content.extension;
                        localPath = tmpContentRoot + content.name + content.extension;
                        ftp.upload(remotePath, localPath);

                        if (!jsonConfig.Equals("["))
                        {
                            jsonConfig += ",";
                        }

                        fileHash = getMD5(localPath);

                        jsonConfig += "{\"filename\":\"" + content.name + content.extension + "\"," +
                                      "\"hash\":\"" + fileHash + "\"," +
                                      "\"duration\":" + content.duration + "," +
                                      "\"clientType\":" + "\"01\"" + "}";
                    }

                    jsonConfig += "]";

                    // если файлов нет - пуляем голый json
                    if (jsonConfig.Equals("[]"))
                    {
                        jsonConfig = "[{\"filename\":\"\",\"hash\":\"\",\"duration\":0,\"clientType\":\"\"}]";
                    }

                    //проверяем есть ли локальная директория пакета
                    if (!Directory.Exists(tmpContentRoot + package.urlName)) 
                    {
                        Directory.CreateDirectory(tmpContentRoot + package.urlName);
                    }

                    // льем конфиг в локаль
                    System.IO.File.WriteAllText(tmpContentRoot + package.urlName + "/" + "config.json",
                                                jsonConfig);

                    // льем с локали на ftp
                    ftp.upload("content/" + package.urlName + "/config.json", 
                               tmpContentRoot + package.urlName + "/" + "config.json");

                }
                catch (NullReferenceException e) { }
                 
                return Json(new { Success = 1, PackageId = package.Id, ex = "" });
                //}

            }
            catch (Exception ex)
            {
                // If Sucess== 0 then Unable to perform Save/Update Operation and send Exception to View as JSON
                return Json(new { Success = 0, ex = ex.Message.ToString() });
            }

            return Json(new { Success = 0, ex = new Exception("Unable to save").Message.ToString() });
        }

        // получаем md5 из локального файла
        private String getMD5(String filepath)
        {

            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filepath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", string.Empty);
                }
            }

        }

        //
        // GET: /Package/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        //
        // POST: /Package/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Package package = db.Packages.Find(id);
            package.isActive = false;
            db.Entry(package).State = EntityState.Modified;
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