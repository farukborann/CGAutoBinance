﻿using System;
using System.Windows.Input;

namespace AutoBinance.MVVM
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object?> _action;

        public DelegateCommand(Action<object?> action)
        {
            _action = action;
        }

        public void Execute(object? parameter)
        {
            _action(parameter);
        }

        public bool CanExecute(object? parameter) => true;

        public event EventHandler? CanExecuteChanged;
    }
}
