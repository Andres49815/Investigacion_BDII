﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CountriesApp.Models;

namespace CountriesApp.Controllers
{
    public class CountriesController : Controller
    {
        private CountriesEntities db = new CountriesEntities();

        #region Autogenerated Code
        // GET: Countries
        public ActionResult Index()
        {
            var countries = db.Countries.Include(c => c.Person);
            return View(countries.ToList());
        }

        // GET: Countries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = db.Countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // GET: Countries/Create
        public ActionResult Create()
        {
            ViewBag.presidentID = new SelectList(db.People, "id", "firstName");
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,area,population,flag,anthem,presidentID")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Countries.Add(country);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.presidentID = new SelectList(db.People, "id", "firstName", country.presidentID);
            return View(country);
        }

        // GET: Countries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = db.Countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            ViewBag.presidentID = new SelectList(db.People, "id", "firstName", country.presidentID);
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,area,population,flag,anthem,presidentID")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.presidentID = new SelectList(db.People, "id", "firstName", country.presidentID);
            return View(country);
        }

        // GET: Countries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = db.Countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Country country = db.Countries.Find(id);
            db.Countries.Remove(country);
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
        #endregion

        public ActionResult AllCountries()
        {
            ViewBag.ActualIndex = 0;
            ViewBag.PopulationIndex = 1;

            Country country = db.Countries.Include(c => c.Person).First();

            return View(country);
        }

        public ActionResult ChangeCountry(short actual, short i)
        {
            List<Country> countries = db.Countries.Include(c => c.Person).ToList();
            Country country;

            actual += i;
            actual = actual < 0 ? (short)(countries.Count - 1) : (short)(actual == countries.Count ? 0 : actual);

            ViewBag.ActualIndex = actual;
            ViewBag.PopulationIndex = 1;

            country = countries[actual];

            return View("AllCountries", country);
        }

        [HttpPost] public ActionResult TravelPopulation(short actualCountry, short actualPopulation, short sum)
        {
            List<Country> countries = db.Countries.Include(c => c.Person).ToList();
            Country country;
            int actualPopulationIndex;

            ViewBag.ActualIndex = actualCountry;
            country = countries[actualCountry];

            actualPopulationIndex = actualPopulation + sum;

            if (actualPopulationIndex == 0)
                actualPopulationIndex = 1;

            if (country.People1.Count() < (actualPopulationIndex - 1) * 10)
                actualPopulationIndex--;
            ViewBag.PopulationIndex = actualPopulationIndex;
            return View("AllCountries", country);
        }
        
        [HttpPost] public ActionResult ReadData(FormCollection data, int countryID)
        {
            Person person = new Person();
            Country country = db.Countries.Find(countryID);

            try
            {
                person.firstName = data["newPersonName"];
                person.lastName = data["newPersonLastName"];
                person.birthdate = Convert.ToDateTime(data["newPersonBirthdate"]);
                person.email = data["newPersonEmail"];
                person.birthCountry = countryID;
                person.residenceCountry = countryID;
                ViewBag.ActualIndex = countryID;
                country.AddTemporal(person);
            }
            catch (FormatException)
            {

            }
            ViewBag.PopulationIndex = 1;
            return View("AllCountries", country);
        }
        [HttpPost] public ActionResult DeleteTemporalPeople()
        {
            ViewBag.ActualIndex = 0;
            Country.TemporalPeople = new List<Person>();
            return View("AllCountries", db.Countries.First());
        }
    }
}