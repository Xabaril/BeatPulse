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
