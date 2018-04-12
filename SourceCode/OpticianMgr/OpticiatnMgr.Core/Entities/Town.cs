using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Town : EntityObject
    {
        [Required(ErrorMessage ="Bitte geben Sie einen Ortsnamen an!"), MaxLength(100, ErrorMessage ="Der Name des Ortes ist zu lange!")]
        public string TownName { get; set; }
        [Required(ErrorMessage = "Bitte geben Sie eine Postleitzahl an!"), MaxLength(15, ErrorMessage = "Die Postleitzahl ist zu lange!")]
        public string ZipCode { get; set; }
    }
}
