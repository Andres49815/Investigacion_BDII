using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CountriesApp.Models
{
    public class TotalPeople_Q
    {
        public int BirthYear { get; set; }
        public string CountryName { get; set; }
        public int PeopleBorned { get; set; }
        public bool IsTotal { get; set; }

        public TotalPeople_Q(int BirthYear = -1, string CountryName = "", int PeopleBorned = 0)
        {
            this.BirthYear = BirthYear;
            this.CountryName = CountryName;
            this.PeopleBorned = PeopleBorned;
            this.IsTotal = CountryName.Equals("");
        }
    }
}