﻿using TournamentsXPanded.Settings.Interfaces;
using System;

namespace TournamentsXPanded.Settings
{
    public class ComplexAction<T> : IAction
    {
        public Ref Context { get; private set; }

        public object Value { get; private set; }

        public Action<T> DoFunction { get; private set; }

        public Action<T> UndoFunction { get; private set; }

        public ComplexAction(T value, Action<T> doFunction, Action<T> undoFunction)
        {
            Value = value;
            DoFunction = doFunction;
            UndoFunction = undoFunction;
        }

        public void DoAction()
        {
            DoFunction?.Invoke((T)Value);
        }

        public void UndoAction()
        {
            UndoFunction?.Invoke((T)Value);
        }
    }
}
