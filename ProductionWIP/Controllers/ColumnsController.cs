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

    public class ColumnsController : Controller
    {
        private WIPDbContext db = new WIPDbContext();

        // GET: Columns
        public async Task<ActionResult> Index()
        {
            return View(await db.ColumnNames.ToListAsync());
        }

        // GET: Columns/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ColumnNames columnNames = await db.ColumnNames.FindAsync(id);
            if (columnNames == null)
            {
                return HttpNotFound();
            }
            return View(columnNames);
        }

        // GET: Columns/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Columns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ColumnNamesId,Order,Label")] ColumnNames columnNames)
        {
            if (ModelState.IsValid)
            {
                db.ColumnNames.Add(columnNames);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(columnNames);
        }

        // GET: Columns/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ColumnNames columnNames = await db.ColumnNames.FindAsync(id);
            if (columnNames == null)
            {
                return HttpNotFound();
            }
            return View(columnNames);
        }

        // POST: Columns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ColumnNamesId,Order,Label")] ColumnNames columnNames)
        {
            if (ModelState.IsValid)
            {
                db.Entry(columnNames).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(columnNames);
        }

        // GET: Columns/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ColumnNames columnNames = await db.ColumnNames.FindAsync(id);
            if (columnNames == null)
            {
                return HttpNotFound();
            }
            return View(columnNames);
        }

        // POST: Columns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ColumnNames columnNames = await db.ColumnNames.FindAsync(id);
            db.ColumnNames.Remove(columnNames);
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
