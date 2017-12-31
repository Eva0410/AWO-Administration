using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class CustomRecipient : EntityObject
    {
        public int Customer_Id { get; set; }
        [ForeignKey("Customer_Id")]
        public Customer Customer { get; set; }
        public string Address { get; set; }
    }
}
