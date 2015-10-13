using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProductionWIP.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;

namespace ProductionWIP.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private WIPDbContext db = new WIPDbContext();
        private ApplicationUserManager _userManager;

        public UsersController()
        {
        }

        public UsersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        //public UserManager<ApplicationUser> UserManager { get; private set; }

        // GET: /Usuarios/
        public async Task<ActionResult> Index()
        {
            ApplicationUser usr = await db.Users.Where(x=>x.UserName == User.Identity.Name).FirstOrDefaultAsync();


            return View(await db.Users.ToListAsync());
        }

        

        // GET: /Usuarios/Create
        public ActionResult Create()
        {
           

            RegisterViewModelByAdmin u = new RegisterViewModelByAdmin();            

            u.UserId = "null";

        
              
           
            return View("AddEdit",u);
        }

        // POST: /Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModelByAdmin model)
        {
           

            if (ModelState.IsValid)
            {

                
                   

                    var user = new ApplicationUser() { 
                        UserName = model.UserName, 
                   
            };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        TempData["Guardado"] = true;

                            UserManager.AddToRole(user.Id, "Administrador");
                      
                        //await SignInAsync(user, isPersistent: false);
                            return RedirectToAction("Edit", new { id = user.Id });

                            //return View("AddEdit", model);
                         
                    }
                    else
                    {
                        TempData["GuardadoError"] = true;

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                

                //Problems
                   
                    return View("AddEdit",model);
             
               
            }
            TempData["GuardadoError"] = true;

          
            return View("AddEdit", model);
        }

        // GET: /Usuarios/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser usuariodetalles = db.Users.Find(id);
            if (usuariodetalles == null)
            {
                return HttpNotFound();
            }

            RegisterViewModelByAdmin u = new RegisterViewModelByAdmin();
            u.UserName = usuariodetalles.UserName;
            u.UserId = usuariodetalles.Id;          
           


            return View("AddEdit", u);
        }

        // POST: /Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RegisterViewModelByAdmin model)
        {

            if (model.Password == null)
            {
                ModelState["Password"].Errors.Clear();
                ModelState["ConfirmPassword"].Errors.Clear();

            }
            if (ModelState.IsValid)
            {
             

                var user = UserManager.FindById(model.UserId);

                user.UserName = model.UserName;
        

                var result = await UserManager.UpdateAsync(user);

               
                if (result.Succeeded)
                {
                    //UserManager.RemoveFromRole(user.Id, "Role");
                  

                    //UserManager.AddToRole(user.Id, model.Role);



                    if (model.Password != null)
                    {
                        UserManager.RemovePassword(user.Id);
                        UserManager.AddPassword(user.Id, model.Password);

                    }
                    //await SignInAsync(user, isPersistent: false);
                    TempData["Guardado"] = true;

                    return RedirectToAction("Edit", new { id = model.UserId});
                }
                else
                {
                    TempData["GuardadoError"] = true;

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }


                // Hubo Problemas
                //ViewBag.vPacientes = db.Pacientes.OrderBy(r=>r.Nombre).ToList();

                return View("AddEdit", model);


            }
            //ViewBag.vPacientes = db.Pacientes.OrderBy(r => r.Nombre).ToList();
            TempData["GuardadoError"] = true;


            return View("AddEdit", model);
        }

       

        // POST: /Usuarios/Delete/5
        [ ActionName("Delete_")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
        
            await db.Database.ExecuteSqlCommandAsync("DELETE FROM AspNetUsers WHERE Id='" + id + "'");

            TempData["Eliminado"] = true;

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
