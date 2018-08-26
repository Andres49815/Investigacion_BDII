namespace CountriesApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Person()
        {
            this.Countries = new HashSet<Country>();
        }

        public Person(int id, int idNumber, string firstName, string lastName, int birthCountry, int residenceCountry, DateTime birthdate, string email, byte[] photo, byte[] interview)
        {
            this.id = id;
            this.idNumber = idNumber;
            this.firstName = firstName;
            this.lastName = lastName;
            this.birthCountry = birthCountry;
            this.residenceCountry = residenceCountry;
            this.birthdate = birthdate;
            this.email = email;
            this.photo = photo;
            this.interview = interview;

        }

        public int id { get; set; }
        public int idNumber { get; set; }
        [Display(Name = "Nombre")]public string firstName { get; set; }
        public string lastName { get; set; }
        public Nullable<int> birthCountry { get; set; }
        public Nullable<int> residenceCountry { get; set; }
        [DataType(DataType.Date)] public System.DateTime birthdate { get; set; }
        [Display(Name = "Nombre"), DataType(DataType.EmailAddress)] public string email { get; set; }
        public byte[] photo { get; set; }
        public byte[] interview { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Country> Countries { get; set; }
        public virtual Country Country { get; set; }
        public virtual Country Country1 { get; set; }

        public bool CanBePresident()
        {
            return DateTime.Today.Year - birthdate.Year > 31 && residenceCountry == birthCountry;
        }
        override public string ToString()
        {
            return "Nombre completo: " + firstName + "  " + lastName + "\n" +
                "Cedula: " + idNumber.ToString() + "\n" +
                "Nacimiento: " + birthdate.ToString() + "\n" +
                "Pais de nacimiento: " + birthCountry.ToString() + "\n" +
                "Pais de residencia: " + residenceCountry.ToString() + "\n" +
                "Email: " + email + "\n" + 
                "id: " + id.ToString();
        }
    }
}