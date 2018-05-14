# Failure notification WebHooks

## Microsoft Teams

![BeatPulseUI](./doc/webhook_teams.png)

If you want to send failure notifications to Microsoft Teams the payload to be used is:

```json
{
  "@context": "http://schema.org/extensions",
  "@type": "MessageCard",
  "themeColor": "0072C6",
  "title": "[[LIVENESS]] has failed!",
  "text": "[[FAILURE]]. Click **Learn More** to go to BeatPulseUI!",
  "potentialAction": [
    {
      "@type": "OpenUri",
      "name": "Learn More",
      "targets": [
        { "os": "default", "uri": "http://localhost:52665/beatpulse-ui" }
      ]
    }
  ]
}
```

> BeatPulseUI replace automatically [[LIVENESS]] and [[FAILURE]] bookmarks.

You must scape the json before setting the **Payload** property in the configuration file:

```json
{
  "BeatPulse-UI": {
    "Liveness": [
      {
        "Name": "HTTP-Api-Basic",
        "Uri": "http://localhost:6457/health"
      }
    ],
    "Webhooks": [
      {
        "Name": "Teams",
        "Uri": "https://outlook.office.com/webhook/...",
        "Payload": "{\r\n  \"@context\": \"http://schema.org/extensions\",\r\n  \"@type\": \"MessageCard\",\r\n  \"themeColor\": \"0072C6\",\r\n  \"title\": \"[[LIVENESS]] has failed!\",\r\n  \"text\": \"[[FAILURE]] Click **Learn More** to go to BeatPulseUI Portal\",\r\n  \"potentialAction\": [\r\n    {\r\n      \"@type\": \"OpenUri\",\r\n      \"name\": \"Lear More\",\r\n      \"targets\": [\r\n        { \"os\": \"default\", \"uri\": \"http://localhost:52665/beatpulse-ui\" }\r\n      ]\r\n    }\r\n  ]\r\n}"
      }
    ],
    "EvaluationTimeOnSeconds": 10
  }
}
```

## Azure Functions

You can use [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/) to receive *BeatPulseUI* notifications and perform any action. 

Next samples show AF integration with Twilio to send SMS / SendGrid for BeatPulseUI failure notifications.

```c#
#r "Twilio.API"
#r "Newtonsoft.Json"

using System;
using System.Net;
using Twilio;
using Newtonsoft.Json.Linq;

public static async Task Run(HttpRequestMessage req, IAsyncCollector<SMSMessage> message, TraceWriter log)

{
    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic payload = JObject.Parse(jsonContent);

    log.Info($"Notifying a new failure notification to configured phone number");

    var sms = new SMSMessage();
    sms.Body = $"The liveness {payload.liveness} is failing with message {payload.message}";
    await message.AddAsync(sms);
}

```

```c# 
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
```

```json
{
  "BeatPulse-UI": {
    "Liveness": [
      {
        "Name": "HTTP-Api-Basic",
        "Uri": "http://localhost:6457/health"
      }
    ],
    "Webhooks": [
      {
        "Name": "AzureFunctions",
        "Uri": "https://beatpulsenotifier.azurewebsites.net/api/sample?code=...==",
        "Payload": "{\"liveness\": \"[[LIVENESS]]\",\"message\": \"[[FAILURE]]\"}"
      }
    ],
    "EvaluationTimeOnSeconds": 10
  }
}
```