using TournamentsXPanded.Settings.Interfaces;
using System;
using TournamentsXPanded.Settings;

namespace TournamentsXPanded.Settings
{
    public class SetValueAction<T> : IAction where T : struct
    {
        public Ref Context { get; private set; }

        public object Value { get; private set; }

        private T original;

        public SetValueAction(Ref context, T value)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Context = context;
            Value = value;

            original = (T)Context.Value;
        }

        public void DoAction()
        {
            Context.Value = Value;
        }

        public void UndoAction()
        {
            Context.Value = original;
        }
    }
}
