using Microsoft.Extensions.Configuration;

namespace DfE.Core.Libraries.Testing;

public static class ConfigurationDefault
{
    public static IConfiguration Create() => CreateBuilder().Build();
    public static IConfigurationBuilder CreateBuilder() => new ConfigurationBuilder();
}
