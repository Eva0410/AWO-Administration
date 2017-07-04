using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class LagerndeBrillenfassung : EntityObject
    {
        public String Lieferantenbemerkung { get; set; }
        public String Marke { get; set; }
        public String Modellbezeichnung { get; set; }
        public String Farbe { get; set; }
        public String Größe { get; set; }
        public double Einkaufspreis { get; set; }
        public double Verkaufspreis { get; set; }
        public DateTime Einkaufsdatum { get; set; }
        public DateTime Verkaufsdatum { get; set; }
        [Required]
        public String Status { get; set; }
        public int Lieferant_Id { get; set; }
        [ForeignKey("Lieferant_Id")]
        public Lieferant Lieferant { get; set; }
    }
}
