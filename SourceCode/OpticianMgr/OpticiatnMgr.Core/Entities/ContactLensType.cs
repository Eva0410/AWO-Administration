using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class ContactLensType : EntityObject
    {
        [Required]
        public String ContactLensTypeDescription { get; set; }
    }
}
