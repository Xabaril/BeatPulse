#r "SendGrid"
using System;
using SendGrid.Helpers.Mail;

public static async Task Run(HttpRequestMessage req, IAsyncCollector<Mail> message, TraceWriter log)
{
    const string targetEmail = "youemail@host.com";

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic payload = JObject.Parse(jsonContent);

    log.Info($"Notifying a new failure notification to configured phone number");

    var mail = new Mail
    {
        Subject = "Beatpulse Failure Notification"
    };

    var personalization = new Personalization();
    personalization.AddTo(new Email(targetEmail));

    Content content = new Content
    {
        Type = "text/plain",
        Value = $"The liveness {payload.liveness} is failing with message {payload.message}"
    };

    mail.AddContent(content);
    mail.AddPersonalization(personalization);

    await message.AddAsync(mail);
}