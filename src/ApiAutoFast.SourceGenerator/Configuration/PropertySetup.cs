namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct PropertySetup
{
    internal PropertySetup(string name, string baseSource, DomainValueMetadata domainValueMetadata)
    {
        Name = name;
        BaseSource = baseSource;
        DomainValueMetadata = domainValueMetadata;
    }

    internal readonly string Name { get; }
    internal readonly string BaseSource { get; }
    internal readonly DomainValueMetadata DomainValueMetadata { get; }
}
