using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticianMgr.Wpf.WindowServices
{
    public interface IRequestClose
    {
        event EventHandler<EventArgs> CloseRequested;
    }
}
