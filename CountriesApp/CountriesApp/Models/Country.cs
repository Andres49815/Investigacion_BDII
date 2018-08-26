namespace CountriesApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Country
    {
        #region Constructors
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Country()
        {
            this.People = new HashSet<Person>();
            this.People1 = new HashSet<Person>();
            this.SelectedPeople = new HashSet<Person>();
        }
        public Country(int id, string name, decimal area, float population, byte[] flag, byte[] anthem, int presidentID)
        {
            this.id = id;
            this.name = name;
            this.area = area;
            this.population = (short)population;
            this.flag = flag;
            this.anthem = anthem;
            this.presidentID = presidentID;
        }
        #endregion

        [Display(Name = "Identificador")] public int id { get; set; }
        [Display(Name = "Nombre")] public string name { get; set; }
        [Display(Name = "Area")] public decimal area { get; set; }
        [Display(Name = "Poblacion")] public short population { get; set; }
        [Display(Name = "Bandera")] public byte[] flag { get; set; }
        [Display(Name = "Himno")] public byte[] anthem { get; set; }
        [Display(Name = "Presidente")] public Nullable<int> presidentID { get; set; }

        public virtual Person Person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Person> People { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Person> People1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Person> SelectedPeople { get; set; }

        public override string ToString()
        {
            return "Nombre: " + name + "\n" +
                "Arae: " + area.ToString();
        }
    }

}
