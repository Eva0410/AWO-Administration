using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class EyeGlassFrame : EntityObject
    {
        public String SupplierComment { get; set; }
        public String Brand { get; set; }
        public String ModelDescription { get; set; }
        public String Color { get; set; }
        public String Size { get; set; }
        public Decimal? PurchasePrice { get; set; }
        public Decimal? SalePrice { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? PurchaseDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? SaleDate { get; set; }
        [Required]
        public String State { get; set; }
        public int? Supplier_Id { get; set; }
        [ForeignKey("Supplier_Id")]
        public Supplier Supplier { get; set; }

        public List<Order> Orders { get; set; }
    }
}
