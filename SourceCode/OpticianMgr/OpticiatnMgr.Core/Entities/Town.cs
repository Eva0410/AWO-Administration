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
        //Keine Required-Attribute, für den Fall dass der Optiker nur schnell einen Lieferanten einfügen will und er später doch noch einen Ort einfügen will
        public string TownName { get; set; }
        public string ZipCode { get; set; }
    }
}
