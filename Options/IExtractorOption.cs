namespace ExtractAnywhere.Options
{
    public interface IExtractorOption<T>
    {
        T Value { get; set; }

        event ExtractorOptionChangedEventHandler<T>? ValueChanged;

        void SetDefaults();
    }
}
