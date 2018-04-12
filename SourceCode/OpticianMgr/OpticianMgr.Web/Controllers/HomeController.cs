using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpticianMgr.WebIdentity.Models;
using System.Net;
using System.Net.Mail;
using OpticianMgr.WebIdentity.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OpticiatnMgr.Core.Contracts;
using System.IO;
using System.Web;
using OpticianMgr.Web.Models;
using Microsoft.AspNetCore.Http;
using OpticiatnMgr.Core.Entities;

namespace OpticianMgr.WebIdentity.Controllers
{
    public class HomeController : Controller
    {
        IUnitOfWork uow;
        UserManager<ApplicationUser> um;
        SignInManager<ApplicationUser> sim;
        public HomeController(IUnitOfWork _uow, UserManager<ApplicationUser> _um, SignInManager<ApplicationUser> _sm)
        {
            uow = _uow;
            um = _um;
            sim = _sm;
        }


        public IActionResult Index()
        {
            return View();
        }

        

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Wunschzettel()
        {
            GlassesModel model = new GlassesModel();
            model.FillGlasses(uow);
            return View(model);
        }

        public IActionResult Kaufinteresse()
        {
            GlassesModel model = new GlassesModel();
            model.FillGlasses(uow);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> KaufinteresseMit(int id)
        {
            var g = uow.GlassesRepository.Get(gl => gl.Id == id).FirstOrDefault();
            if (g == null)
                return NotFound();
            g.Wish = false;
            g.Kaufinteresse++;
            uow.GlassesRepository.Update(g);
            uow.Save();
            return RedirectToAction("Wunschzettel");
        }

        [HttpPost]
        public async Task<IActionResult> Wunschzettel(int id)
        {
            var g = uow.GlassesRepository.Get(gl => gl.Id == id).FirstOrDefault();
            if (g == null)
                return NotFound();
            g.Wish = true;
            uow.GlassesRepository.Update(g);
            uow.Save();
            return RedirectToAction("Wunschzettel");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteWish(int id)
        {
            var g = uow.GlassesRepository.Get(gl => gl.Id == id).FirstOrDefault();
            if (g == null)
                return NotFound();
            g.Wish = false;
            uow.GlassesRepository.Update(g);
            uow.Save();
            return RedirectToAction("Wunschzettel");
        }

        public IActionResult AdminBrillen()
        {
            GlassesModel model = new GlassesModel();
            model.FillGlasses(uow);
            return View(model);
        }
        [HttpPost]
        public IActionResult AdminBrillen(HttpPostedFileBase file)
        {
            var path = "";
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    if (Path.GetExtension(file.FileName).ToLower() == ".jpg"
                        || Path.GetExtension(file.FileName).ToLower() == ".png"
                            || Path.GetExtension(file.FileName).ToLower() == ".jpeg")
                    {
                        path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/wwwroot/images"), file.FileName);
                        file.SaveAs(path);
                        ViewBag.UploadSuccess = true;
                    }
                }
            }
            return View();
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdministrationArea()
        {

            AdministrationsAreaModel model = new AdministrationsAreaModel();
            await model.GetAdmins(um/*, this.uow*/);
            return View(model);
        }
        //TODO Info (Admin muss sich neu einloggen um Administrator-Rechte zu bekommen
        //TODO handle back button
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AdministrationArea(AdministrationsAreaModel model)
        {
            ApplicationUser user = null;
            if (String.IsNullOrEmpty(model.NewAdmin))
            {
                ModelState.AddModelError("NewAdmin", "Bitte geben Sie einen Namen ein!");
            }
            else
            {
                user = await um.FindByNameAsync(model.NewAdmin);
            }
            await model.GetAdmins(um/*, this.uow*/);
            if (ModelState.IsValid && user != null)
            {
                await um.AddToRoleAsync(user, "Admin");
            }
            else
            {
                ModelState.AddModelError("NewAdmin", "Zu diesem Namen konnte kein Account gefunden werden! Achtung: Der Benutzer muss sich vorher einmal eingeloggt haben, bevor er zum Admin werden kann!");
                return View(model);
            }

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdministrationAreaDelete(AdministrationsAreaModel model, string id)
        {
            var user = await um.FindByNameAsync(id);
            if (user != null)
            {
                await um.RemoveFromRoleAsync(user, "Admin");
            }
            await model.GetAdmins(um/*, this.uow*/);
            return RedirectToAction("AdministrationArea", model);
        }
        


        public IActionResult Herrenbrillen(string sort)
        {
            GlassesModel model = new GlassesModel();
            model.SelectedSortProperty = sort;
            model.FillGlasses(uow);
            return View(model);
        }
        [HttpPost]
        public IActionResult Herrenbrillen(GlassesModel model)
        {
            ModelState.Clear();
            model.FillGlasses(uow);
            return View(model);
        }
        public IActionResult Frauenbrillen()
        {
            GlassesModel model = new GlassesModel();
            model.FillGlasses(uow);
            return View(model);
        }
        [HttpPost]
        public IActionResult Frauenbrillen(GlassesModel model)
        {
            ModelState.Clear();
            model.FillGlasses(uow);
            return View(model);
        }
        public IActionResult Kinderbrillen()
        {
            GlassesModel model = new GlassesModel();
            model.FillGlasses(uow);
            return View(model);
        }
        [HttpPost]
        public IActionResult Kinderbrillen(GlassesModel model)
        {
            ModelState.Clear();
            model.FillGlasses(uow);
            return View(model);
        }
        public async Task<IActionResult> NewGlasses()
        {
            NewGlasses model = new NewGlasses();
            await model.Init();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> NewGlasses(NewGlasses model, int id)
        {

            if (id == 1)
            {
                if (ModelState.IsValid)
                {
                    //Save image
                    model.Glasses.Image = ConvertImage(model.Image, model.ImageAsString);


                    this.uow.GlassesRepository.Insert(model.Glasses);
                    this.uow.Save();

                    return RedirectToAction("AdminBrillen");

                }
                else
                {
                    await model.FillList();
                    //save image as string
                    model.ImageAsString = ConvertImage(model.Image, model.ImageAsString);
                    return View(model);
                }
            }
            else if (id == 2)
            {
                await model.FillList();
                model.ImageAsString = string.Empty;
                return View(model);
            }
            return NotFound();
        }


        public string ConvertImage(IFormFile Image, string ImageAsString)
        {
            if (Image != null && Image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    return Convert.ToBase64String(fileBytes);
                }
            }
            return ImageAsString;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGlasses(int id)
        {
            var g = uow.GlassesRepository.Get(gl => gl.Id == id).FirstOrDefault();
            if (g == null)
                return NotFound();
            uow.GlassesRepository.Delete(g);
            uow.Save();
            return RedirectToAction("AdminBrillen");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditGlasses(int id)
        {
            var g = uow.GlassesRepository.Get(gl => gl.Id == id).FirstOrDefault();

            return RedirectToAction("AdminBrillen");
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress("orascanin.99@gmail.com"));
                    message.From = new MailAddress("diplomarbeitdanijal@gmail.com");
                    message.Subject = "Anfrage!";
                    message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "diplomarbeitdanijal@gmail.com",
                            Password = ".di,wx,01,21"
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(message);
                        return RedirectToAction("Sent");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", "Ein Fehler ist aufgetreten! Tipp: Haben Sie Ihre Verbindung zum Internet überprüft?");
                }
            }
            return View(model);
        }

        public ActionResult Sent()
        {
            return View();
        }
    }
}
