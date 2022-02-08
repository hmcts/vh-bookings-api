using System;
using System.Windows.Input;

namespace BookingsApi.DAL.Commands
{
    public class AnonymiseHearingsWithUsernameCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;
    }
}