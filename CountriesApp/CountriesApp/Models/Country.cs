//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CountriesApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Country
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Country()
        {
            this.People = new HashSet<Person>();
            this.People1 = new HashSet<Person>();
        }

        #region Atomic Parameters
        public int id { get; set; }
        [Display(Name = "Pais")] public string name { get; set; }
        [Display(Name = "Area")] public decimal area { get; set; }
        [Display(Name = "Pobalcion")] public short population { get; set; }
        public byte[] flag { get; set; }
        public byte[] anthem { get; set; }
        [Display(Name = "Presidente")] public Nullable<int> presidentID { get; set; }
        #endregion

        #region Ptrs Parameteres
        [Display(Name = "Presidente")] public virtual Person Person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Person> People { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Person> People1 { get; set; }
        #endregion

        public Country(int i, string n, short p, int pID)
        {
            id = i;
            name = n;
            population = p;
            presidentID = null;
            TemporalPeople = new List<Person>();
        }

        public static List<Person> TemporalPeople = new List<Person>();

        public List<Person> Temporal_People()
        {
            return TemporalPeople;
        }

        public void AddTemporal(Person person)
        {
            try
            {
                TemporalPeople.Add(person);
            }
            catch (System.NullReferenceException)
            {
                TemporalPeople = new List<Person>();
                TemporalPeople.Add(person);
            }
        }

    }
}
