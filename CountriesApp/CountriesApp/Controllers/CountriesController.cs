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
        private static int PopulationIndex = 0;
        private CountriesEntities db = new CountriesEntities();
        private static string connectionInfo = "data source=bd2progra1countries.database.windows.net;initial " +
        "catalog=Countries;persist security info=True;user id=admin1;password=osAf34@fdl4029;MultipleActiveResultSets=True;App=EntityFramework";

        #region Constructors
        /**
         * Controller for the main view, also used when want to change the country
         * In:
         *  1- countryIndex: Actual index of the country that is been seen
         *  2- Sum: Sumatory to the actual index in order to change the Country
         * Out: View with data of the countries
         */
        public ActionResult AllCountries(int? countryIndex = 1, int sum = 0)
        {
            // Transactions
            SQLTransactionManager.Instance();

            // Country Information
            countryIndex += sum;
            List<object> countryInformation = SelectCountry((int)countryIndex);
            Country country = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            country.Person = db.People.Find(country.presidentID);//SelectPresident((int)country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(country.id, 0);
            country.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(db.Countries.ToList(), "id", "name");
            return View(country);
        }
        public ActionResult Query_1()
        {
            List<CountryInfo_Q> info = CountryInfo();
            return View(info);
        }
        public ActionResult Query_2(int? Year = 1960, int? sum = 0)
        {
            Year += sum;
            if (Year < 1960)
            {
                Year = 2018;
            }
            if (Year > 2018)
            {
                Year = 1960;
            }
            List<TotalPeople_Q> result = TotalPeople((int)Year);
            ViewBag.Year = Year;
            return View(result);
        }
        #endregion

        #region Connection Procedures
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

        // Stored Procedures
        private List<CountryInfo_Q> CountryInfo()
        {
            int countryID, AvarageAge, Population;
            string CountryName;
            List<CountryInfo_Q> query = new List<CountryInfo_Q>();

            countryID = AvarageAge = Population = 0;
            CountryName = "";

            SqlConnection connection = new SqlConnection(connectionInfo);
            connection.Open();
            using (SqlCommand command = new SqlCommand("CountryInfo", connection))
            {
                command.CommandTimeout = 0;
                command.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            countryID = (int)reader["CountryID"];
                            CountryName = (string)reader["CountryName"];
                            AvarageAge = (int)reader["AverageAge"];
                            Population = (int)(decimal) reader["Population"];
                            query.Add(new CountryInfo_Q(countryID, CountryName, AvarageAge, Population));
                        }
                    }
                }
            }
            connection.Close();
            return query;
        }
        private List<TotalPeople_Q> TotalPeople(int Year)
        {
            int BirthYear, PeopleBorned;
            string CountryName;
            List<TotalPeople_Q> query = new List<TotalPeople_Q>();

            SqlConnection connection = new SqlConnection(connectionInfo);
            connection.Open();

            using (SqlCommand command = new SqlCommand("dbo.TotalPeople", connection))
            {
                command.CommandTimeout = 0;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@year", Year);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                BirthYear = (int)reader["BirthYear"];
                            }
                            catch (InvalidCastException)
                            {
                                BirthYear = -1;
                            }
                            try
                            {
                                CountryName = (string)reader["CountryName"];
                            }
                            catch (InvalidCastException)
                            {
                                CountryName = "";
                            }
                            PeopleBorned = (int)reader["PeopleBorned"];
                            query.Add(new TotalPeople_Q(BirthYear, CountryName, PeopleBorned));
                        }
                    }
                }
            }
            connection.Close();
            return query;
        }
        #endregion
        #region Transaction
        [HttpPost] public ActionResult AddPerson([Bind(Include = "id,idNumber,firstName,lastName,birthdate,birthCountry")] Person person)
        {
            person.residenceCountry = person.birthCountry;
            person.email = person.firstName + person.lastName + "@gmail.com";
            SQLTransactionManager.UploadPerson(person);
            // Country Information
            List<object> countryInformation = SelectCountry(1);
            Country country = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            country.Person = db.People.Find(country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(country.id, 0);
            country.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            return View("AllCountries", country);
        }
        [HttpPost] public ActionResult SendCommit()
        {
            SQLTransactionManager.SendCommit();
            // Country Information
            List<object> countryInformation = SelectCountry(1);
            Country country = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            country.Person = db.People.Find(country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(country.id, 0);
            country.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            SQLTransactionManager.EndTransaction(isRollback: false);
            return View("AllCountries", country);
        }
        [HttpPost] public ActionResult Rollback()
        {
            // Country Information
            List<object> countryInformation = SelectCountry(1);
            Country country = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            country.Person = db.People.Find(country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(1, 0);
            country.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            SQLTransactionManager.EndTransaction(isRollback: true);
            return View("AllCountries", country);
        }
        #endregion
        #region Media
        [HttpPost] public ActionResult AddFlag(Country country, HttpPostedFileBase flag1, int countryIndex)
        {
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                byte[] flag = new byte[file.ContentLength];
                file.InputStream.Read(flag, 0, (int)file.ContentLength);
                Country countryModel = db.Countries.Find(countryIndex);
                countryModel.flag = flag;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Country Information
            List<object> countryInformation = SelectCountry(1);
            Country c = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            c.Person = db.People.Find(country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(1, 0);
            c.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            return View("AllCountries", c);
        }
        [HttpPost] public ActionResult AddPhoto(HttpPostedFileBase flag1, int personId)
        {
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                byte[] photo = new byte[file.ContentLength];
                file.InputStream.Read(photo, 0, (int)file.ContentLength);
                Person person = db.People.Find(personId);
                person.photo = photo;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Country Information
            List<object> countryInformation = SelectCountry(1);
            Country c = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            c.Person = db.People.Find(c.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(c.id, 0);
            c.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            return View("AllCountries", c);
        }
        [HttpPost] public ActionResult AddInterview(HttpPostedFileBase flag1, int personId)
        {
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                Person personModel = db.People.Find(personId);
                byte[] interview = new byte[file.ContentLength];
                file.InputStream.Read(interview, 0, (int)file.ContentLength);
                personModel.interview = interview;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Country Information
            List<object> countryInformation = SelectCountry(1);
            Country c = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            c.Person = db.People.Find(c.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(c.id, 0);
            c.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            return View("AllCountries", c);
        }
        [HttpPost] public ActionResult AddAnthem(Country country, HttpPostedFileBase anthem1, int countryID)
        {
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                byte[] anthem = new byte[file.ContentLength];
                file.InputStream.Read(anthem, 0, (int)file.ContentLength);
                Country modelCountry = db.Countries.Find(countryID);
                modelCountry.anthem = anthem;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Country Information
            List<object> countryInformation = SelectCountry(1);
            Country c = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            c.Person = db.People.Find(country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(c.id, 0);
            c.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];
            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");

            return View("AllCountries", c);
        }
        #endregion
        #region Pagination
        [HttpPost] public ActionResult Data(int CountryIndex, short sum)
        {
            Country country = (Country)SelectCountry(CountryIndex)[0];

            PopulationIndex += sum;

            List<object> peopleInformation = SelectPeople(country.id, PopulationIndex);
            country.SelectedPeople = (List<Person>)peopleInformation[0];
            PopulationIndex = (int)peopleInformation[1];

            return View("PeopleList", country);
        }
        [HttpPost] public ActionResult FirstData(int CountryIndex)
        {
            Country country = (Country)SelectCountry(CountryIndex)[0];
            
            List<object> peopleInformation = SelectPeople(country.id, 0);
            country.SelectedPeople = (List<Person>)peopleInformation[0];
            PopulationIndex = (int)peopleInformation[1];

            return View("PeopleList", country);
        }
        [HttpPost]
        public ActionResult LastData(int CountryIndex)
        {
            Country country = (Country)SelectCountry(CountryIndex)[0];

            List<object> peopleInformation = SelectPeople(country.id, -1);
            country.SelectedPeople = (List<Person>)peopleInformation[0];
            PopulationIndex = (int)peopleInformation[1];

            return View("PeopleList", country);
        }
        #endregion
        #region Options
        public ActionResult Edit(int countryID)
        {
            Country country = db.Countries.Find(countryID);
            Country.GlobalPresidentID = country.presidentID;
            Country.GlobalFlag = country.flag;
            Country.GlobalAnthem = country.anthem;
            Country.GlobalPopulation = country.population;
            return View(country);
        }
        public ActionResult Delete(int countryID)
        {
            Country cntry = db.Countries.Find(countryID);
            cntry.presidentID = null;
            db.SaveChanges();
            db.Countries.Remove(cntry);
            db.SaveChanges();

            // Country Information
            List<object> countryInformation = SelectCountry(1);
            Country country = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            country.Person = db.People.Find(country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(country.id, 0);
            country.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            return View("AllCountries", country);
        }
        public ActionResult SetPresident(int countryID, int president)
        {
            Country country = db.Countries.Find(countryID);
            country.presidentID = president;
            db.SaveChanges();

            // Country Information
            List<object> countryInformation = SelectCountry(1);
            Country c = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            c.Person = db.People.Find(country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(country.id, 0);
            c.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            return View("AllCountries", c);
        }
        public ActionResult Edit_Person(int id)
        {
            Person person = db.People.Find(id);
            ViewBag.birthCountry = new SelectList(db.Countries, "id", "name", person.birthCountry);
            ViewBag.residenceCountry = new SelectList(db.Countries, "id", "name", person.residenceCountry);
            Person.GlobalBirthdate = person.birthdate;
            Person.GlobalPhoto = person.photo;
            Person.GlobalInterview = person.interview;
            return View(person);
        }
        public ActionResult DeletePerson(int id)
        {
            Person person = db.People.Find(id);
            Country country = db.Countries.Find(person.birthCountry);
            if (country.presidentID == id)
            {
                country.presidentID = null;
            }
            db.People.Remove(person);
            db.SaveChanges();

            // Country Information
            List<object> countryInformation = SelectCountry(1);
            country = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            country.Person = db.People.Find(country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(country.id, 0);
            country.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            return View("AllCountries", country);
        }
        [HttpPost] public ActionResult Confirm_Edit([Bind(Include = "id,idNumber,firstName,lastName,birthCountry,residenceCountry,birthdate,email")] Person person)
        {
            // Birthdate
            if (person.birthdate.Year == 1)
            {
                person.birthdate = Person.GlobalBirthdate;
            }
            person.photo = Person.GlobalPhoto;
            person.interview = Person.GlobalInterview;
            db.Entry(person).State = EntityState.Modified;
            db.SaveChanges();

            // Country Information
            List<object> countryInformation = SelectCountry(1);
            Country country = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            country.Person = db.People.Find(country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(country.id, 0);
            country.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            return View("AllCountries", country);
        }
        [HttpPost] public ActionResult Confirm_Edit_Country([Bind(Include = "id,name,area,population")] Country country)
        {
            country.presidentID = Country.GlobalPresidentID;
            country.population = Country.GlobalPopulation;
            country.flag = Country.GlobalFlag;
            country.anthem = Country.GlobalAnthem;
            db.Entry(country).State = EntityState.Modified;
            db.SaveChanges();

            // Country Information
            List<object> countryInformation = SelectCountry(1);
            country = (Country)countryInformation[0];
            ViewBag.CountryIndex = (int)countryInformation[1];

            // President Information
            country.Person = db.People.Find(country.presidentID);

            // Resident Information
            List<object> peopleInformation = SelectPeople(country.id, 0);
            country.People1 = (List<Person>)peopleInformation[0];
            ViewBag.PeopleIndex = (int)peopleInformation[1];

            ViewBag.birthCountry = new SelectList(SelectAllCountries(), "id", "name");
            return View("AllCountries", country);
        }
        #endregion
    }
}