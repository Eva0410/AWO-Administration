using GalaSoft.MvvmLight;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticianMgr.Wpf.ViewModel
{
    public class EyeGlassFramesViewModel : ViewModelBase
    {
        public IUnitOfWork Uow { get; set; }
        public ObservableCollection<EyeGlassFrame> EyeGlassFrames { get; set; }
        public EyeGlassFramesViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            this.EyeGlassFrames = new ObservableCollection<EyeGlassFrame>(this.Uow.EyeGlassFrameRepository.Get());//TODO
            this.RaisePropertyChanged(() => this.EyeGlassFrames);
        }
    }
}
