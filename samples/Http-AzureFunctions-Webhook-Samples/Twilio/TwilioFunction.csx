
#r "Twilio.API"

using System;
using System.Net;
using Twilio;

public static async Task Run(HttpRequestMessage req, IAsyncCollector<SMSMessage> message, TraceWriter log)

{
    string messageContent = await req.Content.ReadAsStringAsync();

    log.Info($"Notifying: {messageContent} to configured phone number");

    var sms = new SMSMessage();
    sms.Body = messageContent;
    await message.AddAsync(sms);
}
