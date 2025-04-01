namespace Platform.Application.InfrastructureInterfaces;

public interface IEmailGateway
{
    Task SendEmailAsync(string subject, string body, string sendTo, CancellationToken cancellationToken);
}
