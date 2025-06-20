namespace VCIConsumer.Api.Filter;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class SkipStandardFilterAttribute : Attribute { }
