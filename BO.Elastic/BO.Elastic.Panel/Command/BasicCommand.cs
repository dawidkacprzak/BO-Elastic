using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace BO.Elastic.Panel.Command
{
    public class BasicCommand : ICommand
    {
        private readonly Action _execute;

        public BasicCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
