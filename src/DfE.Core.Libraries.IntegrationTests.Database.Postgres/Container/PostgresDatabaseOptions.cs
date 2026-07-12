using System;
using System.Collections.Generic;
using System.Text;

namespace DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container;

public sealed class PostgresDatabaseOptions
{
#nullable disable
    public string Database { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
#nullable enable
}
