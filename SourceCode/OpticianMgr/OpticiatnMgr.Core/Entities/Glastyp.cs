using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Glastyp : EntityObject
    {
        [Required]
        public string Bezeichnung { get; set; }
    }
}
