using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Supplier : EntityObject
    {
        [Required]
        public String Name { get; set; }
        public String Street { get; set; }
        public String HouseNumber { get; set; }
        public int? Town_Id { get; set; }
        //TODO: Ort einfach einfügen können?
        [ForeignKey("Town_Id")]
        public Town Town { get; set; }
        public String Country { get; set; }
        public String FAX { get; set; }
        public String Telephone { get; set; }
        public String Email { get; set; }
        public String CustomerId { get; set; }
        public String ContactPerson { get; set; }
        public String Products { get; set; }  
        public String Others { get; set; }
        public List<EyeGlassFrame> EyeGlassFrames { get; set; }
    }
}
