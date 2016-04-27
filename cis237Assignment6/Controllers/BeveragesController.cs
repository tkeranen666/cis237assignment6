using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237Assignment6;

// Tim Keranen

namespace cis237Assignment6.Controllers
{
    [Authorize]
    public class BeveragesController : Controller
    {
        private BeverageEntities db = new BeverageEntities();

        // GET: Beverages
        public ActionResult Index()
        {
            DbSet<Beverage> BeveragesToFilter = db.Beverages;

            string filterName = "";
            string filterPack = "";
            string filterMinPrice = "";
            string filterMaxPrice = "";

            decimal minPrice = 0;
            decimal maxPrice = 90;

            // Retrieve value from session and assign it to a variable.
            if (Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["name"];
            }

            if (Session["pack"] != null && !String.IsNullOrWhiteSpace((string)Session["pack"]))
            {
                filterPack = (string)Session["pack"];
            }

            if (Session["minPrice"] != null && !String.IsNullOrWhiteSpace((string)Session["minPrice"]))
            {
                filterMinPrice = (string)Session["minPrice"];
                minPrice = Convert.ToDecimal(filterMinPrice);
            }

            if (Session["maxPrice"] != null && !String.IsNullOrWhiteSpace((string)Session["maxPrice"]))
            {
                filterMaxPrice = (string)Session["maxPrice"];
                maxPrice = Convert.ToDecimal(filterMaxPrice);
            }

            // Filter values using entered criteria.
            IEnumerable<Beverage> filtered = BeveragesToFilter.Where(beverage => beverage.price >= minPrice &&
                                                                                 beverage.price <= maxPrice &&
                                                                                 beverage.pack.Contains(filterPack) &&
                                                                                 beverage.name.Contains(filterName));

            // Create a list for filtered values.
            IEnumerable<Beverage> finalFiltered = filtered.ToList();

            // Send session values to viewbag.
            ViewBag.filterName = filterName;
            ViewBag.filterPack = filterPack;
            ViewBag.filterMinPrice = filterMinPrice;
            ViewBag.filterMaxPrice = filterMaxPrice;

            // Return the filtered list.
            return View(finalFiltered);
        }

        // GET: Beverages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: Beverages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Beverages.Add(beverage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: Beverages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
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

        // Restrict requests other than Post, and set 'Filter' as the action name.
        [HttpPost, ActionName("Filter")]
        public ActionResult Filter()
        {
            // Assign form data to string variables.
            string name = Request.Form.Get("name");
            string pack = Request.Form.Get("pack");
            string minPrice = Request.Form.Get("minPrice");
            string maxPrice = Request.Form.Get("maxPrice");

            // Store data in a session.
            Session["name"] = name;
            Session["pack"] = pack;
            Session["minPrice"] = minPrice;
            Session["maxPrice"] = maxPrice;

            // Redirect to index page for filtering.
            return RedirectToAction("Index");
        }
    }
}
