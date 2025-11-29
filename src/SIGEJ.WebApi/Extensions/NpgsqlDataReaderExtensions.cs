using System.Data;
using System.Data.Common;
using Npgsql;

namespace SIGEJ.WebApi.Extensions;

public static class NpgsqlDataReaderExtensions
{
    public static async Task<T?> GetNullableFieldValueAsync<T>(this NpgsqlDataReader reader, int ordinal,
        CancellationToken cancellationToken = default)
    {
        return await reader.IsDBNullAsync(ordinal, cancellationToken)
            ? default
            : await reader.GetFieldValueAsync<T>(ordinal, cancellationToken);
    }
}