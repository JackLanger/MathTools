using System;
using System.Windows.Input;

namespace MathCanvas.Core.Actions;

public class RelayCommand : ICommand
{
    /// <summary>
    ///     Action to be executed.
    /// </summary>
    private readonly Action _action;

    public RelayCommand(Action action) => _action = action;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => _action?.Invoke();

    public event EventHandler? CanExecuteChanged;
}