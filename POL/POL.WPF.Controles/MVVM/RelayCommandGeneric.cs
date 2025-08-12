using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace POL.WPF.Controles.MVVM
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> _CanExecute;
        private readonly Action<T> _Execute;

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _Execute = execute;
            _CanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_CanExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (_CanExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return _CanExecute == null || _CanExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            _Execute((T) parameter);
        }


        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "The this keyword is used in the Silverlight version")]
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
