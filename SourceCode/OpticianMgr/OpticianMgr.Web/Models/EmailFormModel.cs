using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace OpticianMgr.WebIdentity.Models
{
    public class EmailFormModel
    {
        [Required, Display(Name = "Your name")]
        public string FromName { get; set; }
        [Required, Display(Name = "Your email"), EmailAddress]
        public string FromEmail { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
