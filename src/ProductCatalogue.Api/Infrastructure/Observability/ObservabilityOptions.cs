namespace ProductCatalogue.Api.Infrastructure.Observability;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public bool OpenTelemetryEnabled { get; set; }
}
