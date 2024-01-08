using System;

namespace ExtractAnywhere.Options
{
    public class ExtractorOptionChangedEventArgs<T> : EventArgs
    {
        public ExtractorOptionChangedEventArgs(T value)
        {
            Value = value;
        }

        public readonly T Value;
    }
}
