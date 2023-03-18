using lawChat.Client.Infrastructure.Base;
using System;

namespace lawChat.Client.Infrastructure
{
    internal class LambdaCommand : Command
    {
        private readonly Delegate _execute;
        private readonly Delegate? _canExecute;

        public LambdaCommand(Action<object> execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public LambdaCommand(Action<object> execute, Func<object, bool>? canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public LambdaCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public LambdaCommand(Action execute, Func<object, bool>? canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        protected override bool CanExecute(object p)
        {
            if (!base.CanExecute(p)) return false;
            return _canExecute switch
            {
                null => true,
                Func<bool> canExec => canExec(),
                Func<object, bool> canExec => canExec(p),
                _ => throw new InvalidOperationException(
                    $"Тип делегата {_canExecute.GetType()} не поддерживается командой.")
            };
        }
        protected override void Execute(object p)
        {
            switch (_execute)
            {
                default:
                    throw new InvalidOperationException(
                        $"Тип делегата {_canExecute?.GetType()} не поддерживается командой.");
                case null: throw new InvalidOperationException("Не указан делегат вызова для команды");

                case Action execute:
                    execute();
                    break;
                case Action<object> execute:
                    execute(p);
                    break;
            }
        }
    }
}
