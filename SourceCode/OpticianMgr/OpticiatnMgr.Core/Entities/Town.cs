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
        [Required]
        public string TownName { get; set; }
        [Required]
        public string ZipCode { get; set; }
    }
}
