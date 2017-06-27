using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class Auftrag
    {
        public int Kunden_Id { get; set; }
        [ForeignKey("Kunden_Id")]
        public Kunde Kunde { get; set; }
        public DateTime Auftragsdatum { get; set; }
        public DateTime Zahlungsdatum { get; set; }
        public int Glastyp_Id { get; set; }
        [ForeignKey("Glastyp_Id")]
        public Glastyp Glastyp { get; set; }
        public String ProdukttypSonstiges { get; set; }
        public int Arzt_Id { get; set; }
        [ForeignKey("Arzt_Id")]
        public Arzt Arzt { get; set; }
        public String Sonstiges { get; set; }
        [Required]
        public String Bearbeitungsstatus { get; set; }
        public String Zahlungsstatus { get; set; }
        public double GlaspreisLinks { get; set; }
        public double GlaspreisRechts { get; set; }
        public double Nettopreis { get; set; }
        public double Bruttopreis { get; set; }
        public double Sonstigespreis { get; set; }
        public double Krankenkassengeld { get; set; }
        public double Selbstbehalt { get; set; }
        public double Rabatt { get; set; }
        public Byte[] Rechnung { get; set; }
        public decimal Glas_F_R_sph { get; set; }
        public decimal Glas_F_R_cyl { get; set; }
        public int Glas_F_R_Achse { get; set; }
        public decimal Glas_F_R_Prisma { get; set; }
        public String Glas_F_R_PD_NTH { get; set; }
        public String Glas_FWS { get; set; }
        public decimal Glas_F_L_sph { get; set; }
        public decimal Glas_F_L_cyl { get; set; }
        public int Glas_F_L_Achse { get; set; }
        public decimal Glas_F_L_Prisma { get; set; }
        public String Glas_F_L_PD_NTH { get; set; }
        public String Glas_Ink { get; set; }
        public decimal Glas_N_R_sph { get; set; }
        public decimal Glas_N_R_cyl { get; set; }
        public int Glas_N_R_Achse { get; set; }
        public decimal Glas_N_R_Prisma { get; set; }

        public String Glas_N_R_PD_NTH { get; set; }
        public String Glas_HSA { get; set; }
        public decimal Glas_N_L_sph { get; set; }
        public decimal Glas_N_L_cyl { get; set; }
        public int Glas_N_L_Achse { get; set; }
        public decimal Glas_N_L_Prisma { get; set; }
        public String Glas_N_L_PD_NTH { get; set; }
        public String Glassonstiges { get; set; }
        public String Auftragstyp { get; set; }
        public int Kontaktlinsentyp_Id { get; set; }
        [ForeignKey("Kontaktlinsentyp_Id")]
        public Kontaktlinsentyp Kontaktlinsentyp { get; set; }
        public String SonstigesKontaktlinsen1 { get; set; }
        public String SonstigesKontaktlinsen2 { get; set; }
        public int LagerndeBrillenfassung_Id { get; set; }
        [ForeignKey("LagerndeBrillenfassung_Id")]
        public LagerndeBrillenfassung Brillenfassung { get; set; }
    }
}
