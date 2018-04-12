using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticiatnMgr.Core.Entities
{
    public class CustomMessage : EntityObject
    {
        public DateTime Date { get; set; }
        public String MessageText { get; set; }
        public MessageType MessageType { get; set; }
        public String Subject { get; set; }
        public List<CustomRecipient> Recipients { get; set; }
        public int Order_Id { get; set; }
    }
    public enum MessageType
    {
        SMS = 0,
        EMail = 1
    }
}
