using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Supplier : EntityObject
    {
        [Required(ErrorMessage ="Bitte geben Sie einen Namen ein!"), MaxLength(100, ErrorMessage ="Der Name des Lieferanten ist zu lange!")]
        public String Name { get; set; }
        [MaxLength(100, ErrorMessage = "Der Straße des Lieferanten ist zu lange!")]
        public String Street { get; set; }
        [MaxLength(15, ErrorMessage = "Der Hausnummer des Lieferanten ist zu lange!")]
        public String HouseNumber { get; set; }
        public int? Town_Id { get; set; }
        
        [ForeignKey("Town_Id")]
        public Town Town { get; set; }
        public int? Country_Id { get; set; }
        [ForeignKey("Country_Id")]
        public Country Country { get; set; }
        [MaxLength(100, ErrorMessage = "Der FAX-Nummer des Lieferanten ist zu lange!")]
        public String FAX { get; set; }
        [MaxLength(17, ErrorMessage = "Der Telefonnummer des Lieferanten ist zu lange!")]
        public String Telephone { get; set; }
        [RegularExpression("^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$", ErrorMessage = "Die E-Mail-Adresse ist nicht gültig!"), MaxLength(100, ErrorMessage = "Die Email-Adresse des Lieferanten ist zu lange!")]
        public String Email { get; set; }
        [MaxLength(100, ErrorMessage = "Der Kundennummer ist zu lange!")]
        public String CustomerId { get; set; }
        [MaxLength(150, ErrorMessage = "Der Kontaktperson ist zu lange!")]
        public String ContactPerson { get; set; }
        [MaxLength(300, ErrorMessage = "Die Produkte des Lieferanten ist zu lange!")]
        public String Products { get; set; }
        [MaxLength(300, ErrorMessage = "Die sonstigen Bemerkungen sind zu lange!")]
        public String Others { get; set; }
        public List<EyeGlassFrame> EyeGlassFrames { get; set; }
    }
}
