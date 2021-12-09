namespace EmailSnapshotTesting.Services;

public class Message
{
    public string Subject { get; set; }
    public string Address { get; set; }
    public string HtmlBody { get; set; }
}

public interface IMailPostman
{
    Task SendEmail(Message message);
}

public class FakePostman : IMailPostman
{
    public Task SendEmail(Message message)
    {
        LastMessage = message;
        return Task.CompletedTask;
    }

    public Message LastMessage { get; set; }
}

public class RealPostman : IMailPostman
{
    public Task SendEmail(Message message)
    {
        // TODO: Implement this using SMTP or an Email Gateway
        throw new NotImplementedException();
    }
}
