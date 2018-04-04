using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OpticianMgr.WebIdentity.Models;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpticianMgr.Web.Models
{
    public class NewGlasses
    {
        public IFormFile Image { get; set; }
        [Required(ErrorMessage = "Bitte fügen Sie einen Bild ein."), FileExtensions(Extensions = "jpg", ErrorMessage = "Bild darf nur im jpg-Format hochgeladen werden!")]
        public string ImageFileName
        {
            get
            {
                if (Image != null)
                    return Image.FileName;
                else
                    return null;
            }
        }
        public string ImageAsString { get; set; }
        public Glasses Glasses{ get; set; }

        public SelectList Kategorie { get; set; }
        //not used
        public async Task Init()
        {
            this.Glasses = new Glasses();
            await FillList();
        }

        public async Task FillList()
        {
            var cat = new List<string> { "Herrenbrille", "Frauenbrille", "Kinderbrille" };
            this.Kategorie = new SelectList(cat);
        }
    }
}
