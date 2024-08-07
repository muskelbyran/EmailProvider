using Azure.Messaging.ServiceBus;
using EmailProvider.Models;

namespace EmailProvider.Interfaces;

public interface IEmailService
{
    bool SendEmail(EmailRequest emailRequest);
    EmailRequest UnpackEmailRequest(ServiceBusReceivedMessage message);
}