
//using Microsoft.Extensions.Configuration;
//using System.Net;
//using System.Net.Mail;
//using System.Security;
//using Twilio;
//using Twilio.Rest.Api.V2010.Account;
//using Twilio.Types;

//namespace NationalParkWebApp.Services
//{
//    public class TwilioSender
//    {
//        private readonly IConfiguration _configuration;

//        public TwilioSender(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task SendSmsAsync(string toPhone, string customerName)
//        {
//            var settings = _configuration.GetSection("TwilioSettings");

//            TwilioClient.Init(
//                settings["AccountSid"],
//                settings["AuthToken"]);

//            await MessageResource.CreateAsync(
//                to: new PhoneNumber(toPhone),
//                from: new PhoneNumber(settings["PhoneNumber"]),
//                body: $"Hello {customerName}, your National Park appointment has been confirmed successfully and your payment has been received. Thank you!"
//            );
//        }

//        public async Task MakeConfirmationCallAsync(string toPhone, string customerName)
//        {
//            var settings = _configuration.GetSection("TwilioSettings");

//            TwilioClient.Init(
//                settings["AccountSid"],
//                settings["AuthToken"]);

//            string safeName = SecurityElement.Escape(customerName) ?? "Customer";

//            string message =
//                $"<Response><Say>Hello {safeName}. Your National Park appointment has been confirmed successfully, and your payment has been received. Thank you for booking with us.</Say></Response>";

//            await CallResource.CreateAsync(
//                to: new PhoneNumber(toPhone),
//                from: new PhoneNumber(settings["PhoneNumber"]),
//                twiml: new Twilio.Types.Twiml(message)
//            );
//        }
//    }
//}
