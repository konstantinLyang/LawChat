using System;
using System.Windows.Input;

namespace lawChat.Client.Infrastructure.Base
{
    internal abstract class Command : ICommand
    {
        event EventHandler? ICommand.CanExecuteChanged // генерируется когда метод CanExecute возвращает другой тип
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        bool ICommand.CanExecute(object? parameter) => CanExecute(parameter);
        void ICommand.Execute(object? parameter)
        {
            if (((ICommand)this).CanExecute(parameter)) Execute(parameter);
        }
        protected virtual bool CanExecute(object parameter) => true; // активность визуального элемента
        protected abstract void Execute(object parameter); // логика команды
    }
}
