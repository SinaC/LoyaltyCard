using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasyMVVM
{
    public class AsyncRelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Func<Task> _action;
        private readonly Func<bool> _canExecute;

        public AsyncRelayCommand(Func<Task> action)
            : this(action, null)
        {
        }

        public AsyncRelayCommand(Func<Task> action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync();
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            return CanExecute();
        }

        public bool CanExecute()
        {
            return _canExecute();
        }

        public async Task ExecuteAsync()
        {
            await _action();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class AsyncRelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Func<T, Task> _action;
        private readonly Predicate<T> _canExecute;

        public AsyncRelayCommand(Func<T, Task> action)
            : this(action, null)
        {
        }

        public AsyncRelayCommand(Func<T, Task> action, Predicate<T> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            return CanExecute((T)parameter);
        }

        public bool CanExecute(T parameter)
        {
            return _canExecute(parameter);
        }

        public async Task ExecuteAsync(T parameter)
        {
            await _action(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
