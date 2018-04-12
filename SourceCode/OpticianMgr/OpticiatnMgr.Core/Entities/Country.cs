using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Country : EntityObject
    {
        [Required(ErrorMessage ="Bitte geben Sie ein Land ein!"), MaxLength(100, ErrorMessage ="Der Name des Landes ist zu lange!")]
        public string CountryName { get; set; }
    }
}
