using System;
using System.Windows.Input;

namespace KalymnosBT.MVVMBase
{
    class RelayCommand<T> : ICommand
    {
        private static bool CanExecute(T parameter)
        {
            return true;
        }

        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute,
            Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute ?? CanExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(TranslateParameter(parameter));
        }

        public void Execute(object parameter)
        {
            _execute(TranslateParameter(parameter));
        }

        private T TranslateParameter(object parameter)
        {
            T value = default(T);
            if (parameter != null && typeof(T).IsEnum)
            {
                value = (T)Enum.Parse(typeof(T), (string)parameter);
            }
            else
            {
                value = (T)parameter;
            }
            return value;
        }
    }

    class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute,
            Func<bool> canExecute = null)
            : base(obj => execute(),
                canExecute == null ? null : new Func<object, bool>(obj => canExecute()))
        {

        }
    }
}
