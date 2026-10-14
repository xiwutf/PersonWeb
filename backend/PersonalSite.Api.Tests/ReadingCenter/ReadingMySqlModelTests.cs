using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Data;
using PersonalSite.Api.Models;
using Xunit;

namespace PersonalSite.Api.Tests.ReadingCenter;

public class ReadingMySqlModelTests
{
    [Fact]
    public void MySqlProvider_CanBuildApplicationModelWithoutDatabaseConnection()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql("Server=localhost;Database=model_only;User=model_only;Password=not_used",
                ServerVersion.Parse("8.0.36-mysql")).Options;
        using var db = new AppDbContext(options);
        Assert.NotNull(db.Model.FindEntityType(typeof(ReadingSession)));
        Assert.NotNull(db.Model.FindEntityType(typeof(ReadingBook)));
        foreach (var type in new[] { typeof(ReadingBook), typeof(ReadingProgress), typeof(ReadingAnnotation), typeof(ReadingPreference), typeof(ReadingSession) })
        {
            var mapping = db.Model.FindEntityType(type)!.FindProperty("OwnerKey")!.GetRelationalTypeMapping();
            Assert.Equal(typeof(string), mapping.ClrType);
            Assert.Equal(typeof(string), mapping.Converter!.ModelClrType);
            Assert.Equal(typeof(Guid), mapping.Converter.ProviderClrType);
            var ownerKey = "11111111-1111-4111-8111-111111111111";
            Assert.Equal(Guid.Parse(ownerKey), mapping.Converter.ConvertToProvider(ownerKey));
            Assert.Equal(ownerKey, mapping.Converter.ConvertFromProvider(Guid.Parse(ownerKey)));
            Assert.Equal("char(36)", mapping.StoreType);
        }
        var query = db.ReadingSessions.Where(s => s.OwnerKey == "11111111-1111-4111-8111-111111111111").ToQueryString();
        Assert.Contains("owner_key", query);
    }
}
