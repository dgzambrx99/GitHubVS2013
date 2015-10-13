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

namespace ProductionWIP.Controllers
{
    [Authorize]
    public class OperatorsController : Controller
    {
        private WIPDbContext db = new WIPDbContext();

        // GET: Operators
        public async Task<ActionResult> Index()
        {
            return View(await db.Operators.ToListAsync());
        }

        // GET: Operators/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Operators operators = await db.Operators.FindAsync(id);
            if (operators == null)
            {
                return HttpNotFound();
            }
            return View(operators);
        }

        // GET: Operators/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Operators/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OperatorsId,Number,Name")] Operators operators)
        {
            if (ModelState.IsValid)
            {
                db.Operators.Add(operators);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(operators);
        }

        // GET: Operators/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Operators operators = await db.Operators.FindAsync(id);
            if (operators == null)
            {
                return HttpNotFound();
            }
            return View(operators);
        }

        // POST: Operators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OperatorsId,Number,Name")] Operators operators)
        {
            if (ModelState.IsValid)
            {
                db.Entry(operators).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(operators);
        }

        // GET: Operators/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Operators operators = await db.Operators.FindAsync(id);
            if (operators == null)
            {
                return HttpNotFound();
            }
            return View(operators);
        }

        // POST: Operators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Operators operators = await db.Operators.FindAsync(id);
            db.Operators.Remove(operators);
            await db.SaveChangesAsync();
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
