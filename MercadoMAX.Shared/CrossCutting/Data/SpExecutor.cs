using System.Data;
using Dapper;
using MercadoMAX.Shared.Data;
using MercadoMAX.Shared.DTOs;

namespace MercadoMAX.Shared.CrossCutting.Data;

/// <summary>
/// Ejecutor de Stored Procedures sobre Dapper. Encapsula la "plomería" repetida
/// (abrir conexión + CommandType.StoredProcedure) que hoy se repite en cada repositorio.
/// NO reemplaza Dapper ni los SPs; solo centraliza la invocación.
/// Es la pieza que el ERP Template no provee (deja la capa de datos a cada proyecto).
/// </summary>
public interface ISpExecutor
{
    /// <summary>Ejecuta un SP que devuelve una sola fila (o null) mapeada a <typeparamref name="TResult"/>.</summary>
    Task<TResult?> QuerySingleSpAsync<TResult>(string storedProcedure, object? parameters = null);

    /// <summary>Ejecuta un SP que devuelve N filas mapeadas a <typeparamref name="TResult"/>.</summary>
    Task<List<TResult>> QueryListSpAsync<TResult>(string storedProcedure, object? parameters = null);

    /// <summary>Ejecuta un SP que devuelve el contrato estándar <see cref="SpResult"/> (Success/Message/Id).</summary>
    Task<SpResult> ExecSpResultAsync(string storedProcedure, object? parameters = null);
}

public sealed class SpExecutor : ISpExecutor
{
    private readonly DbConnectionFactory _db;

    public SpExecutor(DbConnectionFactory db) => _db = db;

    public async Task<TResult?> QuerySingleSpAsync<TResult>(string storedProcedure, object? parameters = null)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<TResult>(
            storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<TResult>> QueryListSpAsync<TResult>(string storedProcedure, object? parameters = null)
    {
        using var conn = _db.CreateConnection();
        var rows = await conn.QueryAsync<TResult>(
            storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task<SpResult> ExecSpResultAsync(string storedProcedure, object? parameters = null)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstAsync<SpResult>(
            storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }
}
