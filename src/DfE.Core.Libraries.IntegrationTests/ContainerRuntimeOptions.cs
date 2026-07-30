using DotNet.Testcontainers.Networks;

namespace DfE.Core.Libraries.IntegrationTests.Abstractions;

public sealed class ContainerRuntimeOptions
{
    public INetwork? Network { get; set;  }
}
