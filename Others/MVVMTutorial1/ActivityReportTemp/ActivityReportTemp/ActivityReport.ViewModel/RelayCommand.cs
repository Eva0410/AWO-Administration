using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ActivityReport.ViewModel
{
    public class RelayCommand : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExectue;

        public RelayCommand(Action<object> execute) : this (execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if(execute == null)
            {
                throw new ArgumentNullException("Execute darf nicht null sein!");
            }
            this._execute = execute;
            this._canExectue = canExecute;
             
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value;  }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExectue == null)
            {
                return true;
            }
            else
                return _canExectue(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        public static ICommand CreateCommand(ref ICommand cmd, Action<object> execute, Predicate<object> canExcecute = null)
        {
            if (cmd == null)
                cmd = new RelayCommand(execute, canExcecute);
            return cmd;
        }
    }
}
