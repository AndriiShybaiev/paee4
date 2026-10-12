using System;
using System.Windows.Input;

namespace MonitorSistemaWPF
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(_ => execute(), canExecute == null ? (Predicate<object>)null : _ => canExecute())
        {
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class ComandoIniciar : ICommand
    {
        private readonly RecursosSistema _viewModel;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public ComandoIniciar(RecursosSistema viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        public bool CanExecute(object parameter)
        {
            return !_viewModel.EstaEjecutando;
        }

        public void Execute(object parameter)
        {
            _viewModel.Iniciar();
        }
    }

    public class ComandoParar : ICommand
    {
        private readonly RecursosSistema _viewModel;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public ComandoParar(RecursosSistema viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        public bool CanExecute(object parameter)
        {
            return _viewModel.EstaEjecutando;
        }

        public void Execute(object parameter)
        {
            _viewModel.Parar();
        }
    }

    public class ComandoReiniciarMaximos : ICommand
    {
        private readonly RecursosSistema _viewModel;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public ComandoReiniciarMaximos(RecursosSistema viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _viewModel.ReiniciarMaximos();
        }
    }
}
