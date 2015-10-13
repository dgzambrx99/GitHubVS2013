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

    public class EmbedCodesController : Controller
    {
        private WIPDbContext db = new WIPDbContext();

        // GET: EmbedCodes
        public async Task<ActionResult> Index()
        {
            return View(await db.EmbedCodes.ToListAsync());
        }

        // GET: EmbedCodes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmbedCodes embedCodes = await db.EmbedCodes.FindAsync(id);
            if (embedCodes == null)
            {
                return HttpNotFound();
            }
            return View(embedCodes);
        }

        // GET: EmbedCodes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmbedCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EmbedCodesId,Identifier,Token")] EmbedCodes embedCodes)
        {
            if (ModelState.IsValid)
            {
                db.EmbedCodes.Add(embedCodes);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(embedCodes);
        }

        // GET: EmbedCodes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmbedCodes embedCodes = await db.EmbedCodes.FindAsync(id);
            if (embedCodes == null)
            {
                return HttpNotFound();
            }
            return View(embedCodes);
        }

        // POST: EmbedCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EmbedCodesId,Identifier,Token")] EmbedCodes embedCodes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(embedCodes).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(embedCodes);
        }

        // GET: EmbedCodes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmbedCodes embedCodes = await db.EmbedCodes.FindAsync(id);
            if (embedCodes == null)
            {
                return HttpNotFound();
            }
            return View(embedCodes);
        }

        // POST: EmbedCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EmbedCodes embedCodes = await db.EmbedCodes.FindAsync(id);
            db.EmbedCodes.Remove(embedCodes);
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
