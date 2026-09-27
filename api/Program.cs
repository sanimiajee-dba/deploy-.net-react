using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

// Connection string comes from environment (set in docker-compose.yml).
// Points at Oracle 19c running on the HOST machine (192.168.0.231), not inside a container.
var connectionString = builder.Configuration["ORACLE_CONNECTION_STRING"]
    ?? "User Id=demo_user;Password=demo_pass;Data Source=192.168.0.231:1521/ORCLPDB1;";

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors();

// Simple health check - doesn't touch the DB
app.MapGet("/api/health", () => Results.Ok(new { status = "API is running", time = DateTime.UtcNow }));

// Tests the actual Oracle 19c connection
app.MapGet("/api/db-check", () =>
{
    try
    {
        using var conn = new OracleConnection(connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT banner FROM v$version WHERE ROWNUM = 1";
        var result = cmd.ExecuteScalar()?.ToString() ?? "unknown";

        return Results.Ok(new { connected = true, oracleVersion = result });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Oracle connection failed: {ex.Message}");
    }
});

// Example: fetch a demo list from a table you create for this demo
app.MapGet("/api/items", () =>
{
    try
    {
        using var conn = new OracleConnection(connectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, name FROM demo_items ORDER BY id";

        var items = new List<object>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new { id = reader.GetInt32(0), name = reader.GetString(1) });
        }

        return Results.Ok(items);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Query failed: {ex.Message}");
    }
});

app.Run("http://0.0.0.0:8080");
