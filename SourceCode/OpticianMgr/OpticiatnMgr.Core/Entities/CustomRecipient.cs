using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class CustomRecipient : EntityObject
    {
        public Customer Customer { get; set; }
        public string Address { get; set; }
    }
}
