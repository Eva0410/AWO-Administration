using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Customer : EntityObject
    {
        [MaxLength(45, ErrorMessage ="Der Titel ist zu lange!")]
        public String Title { get; set; }
        [MaxLength(45, ErrorMessage ="Der Vorname ist zu lange!")]
        public String FirstName { get; set; }
        [Required(ErrorMessage ="Der Nachname ist erforderlich!")]
        [MaxLength(45, ErrorMessage ="Der Nachname ist zu lange!")]
        public String LastName { get; set; }
        [MaxLength(60, ErrorMessage ="Der Straßenname ist zu lange!")]
        public String Street { get; set; }
        [MaxLength(30, ErrorMessage ="Die Hausnummer ist zu lange!")]
        public String HouseNumber { get; set; }
        public int? Town_Id { get; set; }
        [ForeignKey("Town_Id")]
        public Town Town { get; set; }
        
        public int? Country_Id { get; set; }
        [ForeignKey("Country_Id")]
        public Country Country { get; set; }
        [MaxLength(17, ErrorMessage ="Die Telefonnummer ist zu lange!")]
        public String Telephone1 { get; set; }
        [MaxLength(17, ErrorMessage = "Die Telefonnummer ist zu lange!")]
        public String Telephone2 { get; set; }
        [RegularExpression("^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$", ErrorMessage ="Die E-Mail-Adresse ist nicht gültig!"), MaxLength(100, ErrorMessage = "Der E-Mail-Adresse des Kunden ist zu lange!")]
        public String Email { get; set; }
        [MaxLength(60, ErrorMessage ="Der Versicherungsname ist zu lange!")]
        public String Insurance { get; set; }
        [MaxLength(100, ErrorMessage ="Der Jobname ist zu lange!")]
        public String Job { get; set; }
        [MaxLength(300, ErrorMessage ="Maximale Zeichenanzahl: 300")]
        public String Hobbies { get; set; }
        [MaxLength(300, ErrorMessage = "Maximale Zeichenanzahl: 300")]
        public String Others1 { get; set; }
        [MaxLength(300, ErrorMessage = "Maximale Zeichenanzahl: 300")]
        public String Others2 { get; set; }
        [MaxLength(15, ErrorMessage ="Die Versicherungsnummer ist zu lange!")]
        public String InsurancePolicyNumber { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? DateOfBirth { get; set; }
        public bool NewsLetter { get; set; }

    }
}
