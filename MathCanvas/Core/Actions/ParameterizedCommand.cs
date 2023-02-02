using System;
using System.Windows.Input;

namespace MathCanvas.Core.Actions;

public class ParameterizedCommand<T> : ICommand
{
    /// <summary>
    ///     Function to be executed to be executed.
    /// </summary>
    private readonly Action<T> _func;

    public ParameterizedCommand(Action<T> func) => _func = func;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => _func?.Invoke((T) parameter!);

    public event EventHandler? CanExecuteChanged;
}