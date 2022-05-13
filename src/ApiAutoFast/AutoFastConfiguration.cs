namespace ApiAutoFast;

public interface IAutoFastConfiguration
{
    public void Configure();
}

public abstract class AutoFastConfiguration
{
    public abstract void Configure();
    protected static void RegisterComplexPropertyValueConverters(params ComplexPropertyValueConverter[] complexPropertyValueConverters)
    {
        foreach (var valueConverter in complexPropertyValueConverters)
        {
            ComplexPropertyValueConverterContainer.Add(valueConverter);
        }
    }
}
