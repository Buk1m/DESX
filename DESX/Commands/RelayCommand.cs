using System;
using System.Windows.Input;
using DESX.Annotations;

namespace ModemTalking.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _action;

        public RelayCommand([NotNull] Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}