using EmailSnapshotTesting.EmailTemplates;

namespace EmailSnapshotTesting.Services;

public interface IMailerService
{
    Task SendEmail<T>(string subject, string address, T model);
    Task SendWelcomeEmail(string address, Welcome welcome);
}

public class MailerService : IMailerService
{
    private readonly IEmailRenderer _renderer;
    private readonly IMailPostman _postman;

    public MailerService(
        IEmailRenderer renderer,
        IMailPostman postman)
    {
        _renderer = renderer;
        _postman = postman;
    }

    public async Task SendWelcomeEmail(string address, Welcome welcome)
    {
        await SendEmail($"Welcome {welcome.FullName}!", address, welcome);
    }

    public async Task SendEmail<T>(string subject, string address, T model)
    {
        var html = await _renderer.Render(model);

        await _postman.SendEmail(new Message
        {
            Subject = subject,
            Address = address,
            HtmlBody = html
        });
    }
}
