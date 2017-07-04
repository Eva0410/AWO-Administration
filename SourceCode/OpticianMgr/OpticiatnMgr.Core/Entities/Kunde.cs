using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Kunde : EntityObject
    {
        public String Titel { get; set; }
        public String Vorname { get; set; }
        [Required]
        public String Nachname { get; set; }
        public String Straße { get; set; }
        public String Hausnummer { get; set; }
        public int Ort_Id { get; set; }
        [ForeignKey("Ort_Id")]
        public Ort Ort { get; set; }
        public String Land { get; set; }
        public String Telefon1 { get; set; }
        public String Telefon2 { get; set; }
        public String Email { get; set; }
        public String Krankenkassa { get; set; }
        public String Job { get; set; }
        public String Hobbies { get; set; }
        public String Sonstiges1 { get; set; }
        public String Sonstiges2 { get; set; }
        public String Versicherungsnummer { get; set; }
        public DateTime Geburtsdatum { get; set; }

    }
}
