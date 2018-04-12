using Microsoft.AspNetCore.Mvc.Rendering;
using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OpticianMgr.WebIdentity.Models
{
    public class GlassesModel
    {
        public List<Glasses> GlassModel { get; set; }
        public SelectList SortList { get; set; }
        public string SelectedSortProperty { get; set; }

        public void FillGlasses(IUnitOfWork uow)
        {
            //defaul sort property
            this.SelectedSortProperty = SelectedSortProperty == null ? "Name" : this.SelectedSortProperty;
            //fill sort selectlist
            var sortprops = new List<String>() { "Name", "Preis - aufsteigend", "Preis - absteigend" };
            this.SortList = new SelectList(sortprops, this.SelectedSortProperty);

            if(this.SelectedSortProperty == "Preis - aufsteigend")
            {
                this.GlassModel = uow.GlassesRepository.Get(orderBy: ord => ord.OrderBy(s => s.Price)).ToList();
            }
            else if (this.SelectedSortProperty == "Preis - absteigend")
            {
                this.GlassModel = uow.GlassesRepository.Get(orderBy: ord => ord.OrderByDescending(s => s.Price)).ToList();
            }
            else
                this.GlassModel = uow.GlassesRepository.Get(orderBy: ord => ord.OrderBy(s => s.Name)).ToList();
        }

    }
}
