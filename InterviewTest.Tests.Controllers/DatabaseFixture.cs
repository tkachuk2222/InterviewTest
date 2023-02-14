using InterviewTest.DataLayer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InterviewTest.Tests.Controllers;

public class DatabaseFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public DbContextOptions<InterviewDbContext> Options { get; set; }

    public DatabaseFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        Options = new DbContextOptionsBuilder<InterviewDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new InterviewDbContext(Options);

        context.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();
}