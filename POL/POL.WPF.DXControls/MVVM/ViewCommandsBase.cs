using System;
using System.Collections.Generic;
using DevExpress.Xpf.Mvvm;

namespace POL.WPF.DXControls.MVVM
{




    public class ViewCommandsBase
    {
        private readonly List<DelegateCommand> commands = new List<DelegateCommand>();

        public void UpdateCanExecute()
        {
            foreach (var command in commands)
            {
                command.RaiseCanExecuteChanged();
            }
        }


        protected DelegateCommand CreateDelegateCommand(Action executeMethod)
        {
            return CreateDelegateCommand(executeMethod, null);
        }

        protected DelegateCommand CreateDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            var command = new DelegateCommand(executeMethod, canExecuteMethod);
            commands.Add(command);
            return command;
        }
    }
}
