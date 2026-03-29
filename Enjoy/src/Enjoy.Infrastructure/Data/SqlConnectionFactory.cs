using System.Data;
using Enjoy.Application.Abstractions.Data;
using Enjoy.Persistence.Constants;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Enjoy.Infrastructure.Data;

internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString(ConfigurationKeys.ConnectionEnjoy)
            ?? throw new Enjoy.Domain.Shared.Exceptions.MissingConnectionStringException(ConfigurationKeys.ConnectionEnjoy);
    }

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
