using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Glasses : EntityObject
    {
        [Display(Name = "Bild")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie einen Namen ein."), Display(Name = "Name")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie Ihren gewünschten Preis ein."), Display(Name = "Preis"), Range(0, 1000, ErrorMessage = "Der Stundensatz muss zwischen 0 und 1000 sein!")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie eine Marke ein."), Display(Name = "Marke")]
        public String Marke { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie eine Kategorie ein."), Display(Name = "Kategorie")]
        public String Category { get; set; }
        public bool Wish { get; set; }
        public int Kaufinteresse { get; set; }
    }
}
