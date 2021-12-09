using EmailSnapshotTesting.EmailTemplates;
using EmailSnapshotTesting.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using System.IO;
using System.Threading.Tasks;

using Xunit;

namespace IntegrationTests;

public class EmailTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly IEmailRenderer _renderer;
    private readonly string _folderPath;

    public EmailTests(WebApplicationFactory<Program> factory)
    {
        var environment = factory.Services.GetRequiredService<IWebHostEnvironment>();
        _folderPath = Path.Combine(environment.ContentRootPath, "../IntegrationTests/Snapshots");

        var scope = factory.Services.CreateScope();
        _renderer = scope.ServiceProvider.GetRequiredService<IEmailRenderer>();
    }

    [Fact]
    public async Task CanSendWelcomeEmail()
    {
        var postman = new FakePostman();

        var mailService = new MailerService(_renderer, postman);

        await mailService.SendWelcomeEmail("person@example.com", new Welcome
        {
            FullName = "Example Person"
        });

        Assert.Equal("person@example.com", postman.LastMessage.Address);
        Assert.Equal("Welcome Example Person!", postman.LastMessage.Subject);

        await SaveToFile("Welcome.actual.html", postman.LastMessage.HtmlBody);
        var expectedBody = await File.ReadAllTextAsync(Path.Combine(_folderPath, "Welcome.expected.html"));

        Assert.Equal(Sanitize(postman.LastMessage.HtmlBody), Sanitize(expectedBody));
    }

    private string Sanitize(string text)
    {
        return text
            .Replace("\r\n", "\n")
            .Replace('\r', '\n');
    }

    private async Task SaveToFile(string name, string content)
    {
        var fullPath = Path.Combine(_folderPath, name);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        await File.WriteAllTextAsync(fullPath, content);
    }
}
