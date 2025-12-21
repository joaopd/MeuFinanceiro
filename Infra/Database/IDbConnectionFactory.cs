using System.Data;

namespace Infra.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}