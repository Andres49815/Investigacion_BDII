using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CountriesApp.Models;

namespace CountriesApp.Controllers
{
    public class CountriesController : Controller
    {
        private static int ActualIndex = 0;
        private static int PopulationIndex = 1;
        private CountriesEntities db = new CountriesEntities();
        private static string connectionInfo = "data source=ecRhin.ec.tec.ac.cr\\Estudiantes;initial catalog=Countries;persist security info=True;" +
            "user id=anobando;password=anobando;MultipleActiveResultSets=True;App=EntityFramework";

        #region ConnectionProcedures
        private static List<Country> SelectAllCountries()
        {
            SqlConnection connection = new SqlConnection(connectionInfo);
            connection.Open();
            List<Country> countries = new List<Country>();
            using (SqlCommand command = new SqlCommand("SELECT id AS id, name as name from dbo.Country", connection))
            {
                command.CommandTimeout = 0;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Country country = new Country();
                            country.id = (int)reader["id"];
                            country.name = (string)reader["name"];
                            countries.Add(country);
                        }
                    }
                }
            }
            connection.Close();
            return countries;
        }
        private static List<Object> SelectCountry(int possition)
        {
            int id = 0, population = 0, presidentId = 0, idx = 0;
            string name = "";
            decimal area = 0;
            byte[] flag = null, anthem = null;

            SqlConnection connection = new SqlConnection(connectionInfo);
            connection.Open();
            using (SqlCommand command = new SqlCommand("dbo.SelectCountry", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.AddWithValue("@index", possition);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            id = (int)reader["id"];
                            name = (string)reader["name"];
                            area = (decimal)reader["area"];
                            population = (int)(decimal)reader["population"];
                            flag = reader["flag"] != DBNull.Value ? (byte[])reader["flag"] : null;
                            anthem = reader["anthem"] != DBNull.Value ? (byte[])reader["anthem"] : null;
                            try
                            {
                                presidentId = (int)reader["presidentID"];
                            }
                            catch(InvalidCastException)
                            {
                                presidentId = -1;
                            }
                            idx = (int)reader["idx"];
                        }
                    }
                }
            }
            connection.Close();
            List<Object> obj = new List<object>();
            obj.Add(new Country(id, name, area, population, flag, anthem, presidentId));
            obj.Add(idx);
            return obj;
        }
        private static Person SelectPresident(int presidentID)
        {
            if (presidentID == -1)
            {
                return null;
            }
            SqlConnection connection = new SqlConnection(connectionInfo);
            connection.Open();
            string query = "SELECT firstName, lastName, birthdate\n" +
                "FROM dbo.Person P INNER JOIN dbo.Country C ON (P.id = C.presidentID)\n" +
                "WHERE C.presidentID = " + presidentID.ToString();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.CommandTimeout = 0;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Person president = new Person();
                            president.firstName = (string)reader["firstName"];
                            president.lastName = (string)reader["lastName"];
                            president.birthdate = (DateTime)reader["birthdate"];
                            connection.Close();
                            return president;
                        }
                    }
                }
            }
            return null;
        }
        private static List<object> SelectPeople(int country, int start)
        {
            int id = 0, idNumber = 0, birthCountry = 0, residenceCountry = 0, idx = 0;
            string firstName = "", lastName = "", email = "";
            DateTime birthdate = new DateTime();
            byte[] photo = null, interview = null;
            List<Person> people = new List<Person>();

            SqlConnection connection = new SqlConnection(connectionInfo);
            connection.Open();
            using (SqlCommand command = new SqlCommand("dbo.SelectPeople", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.AddWithValue("@country", country);
                command.Parameters.AddWithValue("@start", start);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            id = (int)reader["id"];
                            idNumber = (int)reader["idNumber"];
                            firstName = (string)reader["firstName"];
                            lastName = (string)reader["lastName"];
                            birthCountry = (int)reader["birthCountry"];
                            residenceCountry = (int)reader["residenceCountry"];
                            birthdate = (DateTime)reader["birthdate"];
                            email = (string)reader["email"];
                            photo = reader["photo"] != DBNull.Value ? (byte[])reader["photo"] : null;
                            interview = reader["interview"] != DBNull.Value ? (byte[])reader["interview"] : null;
                            idx = (int)reader["idx"];
                            people.Add(new Person(id, idNumber, firstName, lastName, birthCountry, residenceCountry, birthdate, email, photo, interview));
                        }
                    }
                }
            }
            connection.Close();
            List<object> obj = new List<object>();
            obj.Add(people);
            obj.Add(idx);
            return obj;
        }
        #endregion

        #region Autogenerated Code
        // GET: Countries
        public ActionResult Index(int? countryIndex = 1, int? sum = 0)
        {
            countryIndex += sum;

            List<Object> countryInformation = SelectCountry((int)countryIndex);
            Country country = (Country)countryInformation[0];
            country.Person = SelectPresident((int)country.presidentID);
            ViewBag.CountryIndex = (int)countryInformation[1];
            
            List<Object> peopleInformation = SelectPeople((int)countryIndex, 0);
            ViewBag.people = (List<Person>)peopleInformation[0];
            ViewBag.pageIndex = (int)peopleInformation[1];

            List<Country> countries = SelectAllCountries();
            ViewBag.CountriesList = new SelectList(countries, "id", "name");

            return View(country);
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
        [HttpPost, ActionName("Delete")] [ValidateAntiForgeryToken] public ActionResult DeleteConfirmed(int id)
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

        #region TravelMenues
        [HttpPost] public ActionResult TravelCountries(int actualIndex, int sum)
        {

            return View("Index");
        }
        #endregion


        public ActionResult AllCountries()
        {
            using (var conn = new SqlConnection(connectionInfo))
            {
                conn.Open();

                using (var sqlTxn = conn.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                {
                    try
                    {
                        var sqlCommand = new SqlCommand();
                        sqlCommand.Connection = conn;
                        sqlCommand.Transaction = sqlTxn;
                        sqlCommand.CommandText = "SELECT * FROM dbo.People WHERE id < 0";
                        sqlCommand.ExecuteNonQuery();

                        sqlTxn.Commit();
                    }
                    catch (Exception)
                    {
                        sqlTxn.Rollback();
                    }
                }
            }
            ViewBag.ActualIndex = 0;
            ViewBag.PopulationIndex = 1;

            ViewBag.birthCountry = new SelectList(db.Countries, "id", "name");

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

        public ActionResult TravelPopulation(short actualCountry, short actualPopulation, short sum)
        {
            List<Country> countries = db.Countries.Include(c => c.Person).ToList();
            Country country;
            int actualPopulationIndex;

            ViewBag.ActualIndex = actualCountry;
            country = countries[actualCountry];

            actualPopulationIndex = actualPopulation + sum;

            if (actualPopulationIndex == 0)
                actualPopulationIndex = country.People1.Count() / 10 + 1;

            if (country.People1.Count() < (actualPopulationIndex - 1) * 10)
                actualPopulationIndex = 1;
            ViewBag.PopulationIndex = actualPopulationIndex;
            return View("AllCountries", country);
        }

        [HttpPost] public string AddPerson([Bind(Include = "id,idNumber,firstName,lastName,birthdate,birthCountry")] Person person)
        {
            person.residenceCountry = person.birthCountry;
            return person.ToString();
        }

        [HttpPost] public ActionResult AddFlag(Country country, HttpPostedFileBase flag1, int countryIndex)
        {
            Country countryModel = db.Countries.ToList()[countryIndex];
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                countryModel.flag = new byte[file.ContentLength];
                file.InputStream.Read(countryModel.flag, 0, (int)file.ContentLength);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.ActualIndex = countryIndex;
            ViewBag.PopulationIndex = 1;
            return View("AllCountries", countryModel);
        }

        [HttpPost] public ActionResult AddAnthem(Country country, HttpPostedFileBase anthem1, int countryIndex)
        {
            Country countryModel = db.Countries.ToList()[countryIndex];
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                countryModel.anthem = new byte[file.ContentLength];
                file.InputStream.Read(countryModel.anthem, 0, (int)file.ContentLength);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.ActualIndex = countryIndex;
            ViewBag.PopulationIndex = 1;
            return View("AllCountries", countryModel);
        }

        [HttpPost] public ActionResult Data(short sum)
        {
            int actualCountry = ActualIndex;
            int actualPopulation = PopulationIndex;
            

            List<Country> countries = db.Countries.Include(c => c.Person).ToList();
            Country country;
            int actualPopulationIndex;

            ActualIndex = actualCountry;
            country = countries[actualCountry];

            actualPopulationIndex = actualPopulation + sum;

            if (actualPopulationIndex == 0)
                actualPopulationIndex = country.People1.Count() / 10 + 1;

            if (country.People1.Count() < (actualPopulationIndex - 1) * 10)
                actualPopulationIndex = 1;
            PopulationIndex = actualPopulationIndex;

            country.SelectedPeople = new List<Person>();
            int start = (PopulationIndex - 1) * 10;
            int end = (PopulationIndex) * 10;
            for (int i = start; i < end && i < country.People1.Count; i++)
            {
                country.SelectedPeople.Add(country.People1.ToList()[i]);
            }
            
            return View("PeopleList", country);
        }

        [HttpPost] public ActionResult TravelPopulation(int countryIndex, int startPopulation, int sumPopulation)
        {
            List<Object> countryInformation = SelectCountry(countryIndex);
            Country country = (Country)countryInformation[0];

            List<Object> peopleInformation = SelectPeople(1, 1);//((int)countryIndex, startPopulation + sumPopulation);
            ViewBag.people = (List<Person>)peopleInformation[0];
            country.SelectedPeople = (List<Person>)peopleInformation[0];
            ViewBag.pageIndex = (int)peopleInformation[1];

            return View("PeopleList", country);
        }
    }
}