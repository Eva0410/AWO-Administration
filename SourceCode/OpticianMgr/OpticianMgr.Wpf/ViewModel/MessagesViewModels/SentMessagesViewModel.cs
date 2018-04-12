using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using OpticianMgr.Persistence;
using OpticiatnMgr.Core.Contracts;
using OpticiatnMgr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows;
using System.Resources;
using System.Collections;
using System.Windows.Data;
using System.ComponentModel;
using OpticianMgr.Wpf.WindowServices;

namespace OpticianMgr.Wpf.ViewModel
{
    public class SentMessagesViewModel : ViewModelBase, IRequestClose
    {
        public IUnitOfWork Uow { get; set; }
        public object Selected { get; set; }
        public List<CustomMessage> Messages { get; set; }

        public SentMessagesViewModel(IUnitOfWork _uow)
        {
            this.Uow = _uow;
            InitAllMessgaes();
        }

        public event EventHandler<EventArgs> CloseRequested;

        public void InitAllMessgaes()
        {
            this.Messages =  this.Uow.MessageRepository.Get(orderBy: ord => ord.OrderByDescending(m => m.Date), includeProperties: "Recipients").ToList();
            this.RaisePropertyChanged(() => this.Messages);
        }
        public void InitMessages(List<CustomMessage> messages)
        {
            this.Messages = messages;
            this.RaisePropertyChanged(() => this.Messages);
        }
        
    }
}
