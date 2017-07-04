using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Lieferant : EntityObject
    {
        [Required ]
        public String Name { get; set; }
        public String Straße { get; set; }
        public String Hausnummer { get; set; }
        public int Ort_Id { get; set; }
        //TODO: Ort einfach einfügen können?
        //[ForeignKey("Ort_Id")]
        //public Ort Ort { get; set; }
        public String Land { get; set; }
        public String FAX { get; set; }
        public String Telefon { get; set; }
        public String Email { get; set; }
        public String Kundennummer { get; set; }
        public String Kontaktperson { get; set; }
        public String Produkte { get; set; }  
        public String Sonstiges { get; set; }
    }
}
