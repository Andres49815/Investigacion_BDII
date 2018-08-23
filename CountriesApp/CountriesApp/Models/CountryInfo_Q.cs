using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CountriesApp.Models
{
    public class CountryInfo_Q
    {
        public int CountryID;
        public string CountryName;
        public int AvarageAge;
        public int Population;

        public CountryInfo_Q(int CountryID, string CountryName, int AvarageAge, int Population)
        {
            this.CountryID = CountryID;
            this.CountryName = CountryName;
            this.AvarageAge = AvarageAge;
            this.Population = Population;
        }
    }
}