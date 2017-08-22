using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Order : EntityObject
    {
        public int Customer_Id { get; set; }
        [ForeignKey("Customer_Id")]
        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public int? GlassType_Id { get; set; }
        [ForeignKey("GlassType_Id")]
        public Glasstype GlassType { get; set; }
        public String GlassTypeOthers { get; set; }
        public int? Doctor_Id { get; set; }
        [ForeignKey("Doctor_Id")]
        public Doctor Doctor { get; set; }
        public String Others { get; set; }
        [Required]
        public String ProcessingState { get; set; }
        public String PaymentState { get; set; }
        public decimal GlassPriceLeft { get; set; }
        public decimal GlassPriceRight { get; set; }
        public decimal NetPrice { get; set; }
        public decimal GrossPrice { get; set; }
        public decimal OthersPrice { get; set; }
        public decimal InsurancePrice { get; set; }
        public decimal PatientsContribution { get; set; }
        public decimal Discount { get; set; }
        public Byte[] Bill { get; set; }
        public decimal Glass_F_R_sph { get; set; }
        public decimal Glass_F_R_cyl { get; set; }
        public int Glass_F_R_Axis { get; set; }
        public decimal Glass_F_R_Prism { get; set; }
        public String Glass_F_R_PD_NTH { get; set; }
        public String Glass_FWS { get; set; }
        public decimal Glass_F_L_sph { get; set; }
        public decimal Glass_F_L_cyl { get; set; }
        public int Glass_F_L_Axis { get; set; }
        public decimal Glass_F_L_Prism { get; set; }
        public String Glass_F_L_PD_NTH { get; set; }
        public String Glass_Ink { get; set; }
        public decimal Glass_N_R_sph { get; set; }
        public decimal Glass_N_R_cyl { get; set; }
        public int Glass_N_R_Axis { get; set; }
        public decimal Glass_N_R_Prism { get; set; }

        public String Glass_N_R_PD_NTH { get; set; }
        public String Glass_HSA { get; set; }
        public decimal Glass_N_L_sph { get; set; }
        public decimal Glass_N_L_cyl { get; set; }
        public int Glass_N_L_Axis { get; set; }
        public decimal Glass_N_L_Prism { get; set; }
        public String Glas_N_L_PD_NTH { get; set; }
        public String GlassOthers { get; set; }
        //B = Brille, K = Kontaktlinse
        public char OrderType { get; set; }
        public int? ContactLensType_Id { get; set; }
        [ForeignKey("ContactLensType_Id")]
        public ContactLensType ContactLensType { get; set; }
        public String ContactLensOthers1 { get; set; }
        public String ContactLensOthers2 { get; set; }
        public int? EyeGlassFrame_Id { get; set; }
        [ForeignKey("EyeGlassFrame_Id")]
        public EyeGlassFrame EyeGlassFrame { get; set; }
    }
}
