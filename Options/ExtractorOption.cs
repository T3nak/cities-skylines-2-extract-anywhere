namespace ExtractAnywhere.Options
{
    public abstract class ExtractorOption<T> : IExtractorOption<T> where T : new()
    {
        private T _value = new();

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChanged();
            }
        }

        public event ExtractorOptionChangedEventHandler<T>? ValueChanged;

        protected void OnValueChanged(ExtractorOptionChangedEventArgs<T> e)
        {
            ValueChanged?.Invoke(this, e);
        }

        protected void OnValueChanged(T value)
        {
            OnValueChanged(new ExtractorOptionChangedEventArgs<T>(value));
        }

        protected void OnValueChanged()
        {
            OnValueChanged(Value);
        }

        public virtual void SetDefaults()
        {
            Value = new();
        }
    }
}
