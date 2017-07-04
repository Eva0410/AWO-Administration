using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Ort : EntityObject
    {
        [Required]
        public string OrtName { get; set; }
        [Required]
        public string PLZ { get; set; }
    }
}
