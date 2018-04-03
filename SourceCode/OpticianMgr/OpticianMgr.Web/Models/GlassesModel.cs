using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpticianMgr.WebIdentity.Models
{
    public class GlassesModel
    {
        public List<Glasses> GlassModel { get; set; }

        public void FillGlasses(IUnitOfWork uow)
        {
            this.GlassModel = uow.GlassesRepository.Get(orderBy: ord => ord.OrderBy(s => s.Name)).ToList();
        }

    }
}
