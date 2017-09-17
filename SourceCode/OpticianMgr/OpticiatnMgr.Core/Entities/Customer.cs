using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Customer : EntityObject
    {
        public String Title { get; set; }
        public String FirstName { get; set; }
        [Required]
        public String LastName { get; set; }
        public String Street { get; set; }
        public String HouseNumber { get; set; }
        public int? Town_Id { get; set; }
        [ForeignKey("Town_Id")]
        public Town Town { get; set; }
        
        public int? Country_Id { get; set; }
        [ForeignKey("Country_Id")]
        public Country Country { get; set; }
        public String Telephone1 { get; set; }
        public String Telephone2 { get; set; }
        public String Email { get; set; }
        public String Insurance { get; set; }
        public String Job { get; set; }
        public String Hobbies { get; set; }
        public String Others1 { get; set; }
        public String Others2 { get; set; }
        public String InsurancePolicyNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
}
