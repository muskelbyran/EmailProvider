﻿using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailProvider.Interfaces;
using EmailProvider.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmailProvider.Services;

public class EmailService(EmailClient emailClient, ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailClient _emailClient = emailClient;
    private readonly ILogger<EmailService> _logger = logger;

    public EmailRequest UnpackEmailRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            var emailRequest = JsonConvert.DeserializeObject<EmailRequest>(message.Body.ToString());
            if (emailRequest != null)
                return emailRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.UnpackEmailRequest() :: {ex.Message}");
        }

        return null!;
    }

    public bool SendEmail(EmailRequest emailRequest)
    {
        try
        {
            var result = _emailClient.Send(
                Azure.WaitUntil.Completed,
                senderAddress: Environment.GetEnvironmentVariable("SenderAddress"),
                recipientAddress: emailRequest.To,
                subject: emailRequest.Subject,
                htmlContent: emailRequest.HtmlBody,
                plainTextContent: emailRequest.PlainText);

            if (result.HasCompleted)
                return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.SendEmail() :: {ex.Message}");
        }

        return false;
    }
}