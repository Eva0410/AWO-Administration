using OpticianMgr.Wpf.WindowServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticianMgr.Wpf
{
    public interface IWindowService
    {
        void ShowAddSupplierWindow<TViewModel>(TViewModel dataContext) where TViewModel : IRequestClose;
        
    }
}
