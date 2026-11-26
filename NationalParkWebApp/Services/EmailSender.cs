
using System.Net;
using System.Net.Mail;

namespace NationalParkWebApp.Services
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAppointmentConfirmationAsync(
            string toEmail, string customerName)
        {
            var settings = _configuration.GetSection("EmailSettings");

            using var message = new MailMessage();
            message.From = new MailAddress(settings["FromEmail"]!);
            message.To.Add(toEmail);
            message.Subject = "National Park Appointment Confirmation";
            message.Body = $@"Hello {customerName},

Your appointment has been confirmed successfully.
Your payment has been received.

Thank you for booking with us!

Regards,
National Park Team";

            using var smtp = new SmtpClient(
                settings["PrimaryDomain"],
                int.Parse(settings["PrimaryPort"]!))
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    settings["UsernameEmail"],
                    settings["UsernamePassword"])
            };

            await smtp.SendMailAsync(message);
        }

        public async Task SendAppointmentCancellationAsync(
            string email,
            string name)
        {
            var settings = _configuration.GetSection("EmailSettings");

            using var message = new MailMessage();
            message.From = new MailAddress(settings["FromEmail"]!);
            message.To.Add(email);
            message.Subject = "National Park Appointment Cancellation";
            message.Body = $@"Hello {name},

Your National Park appointment has been cancelled successfully.

If you have already paid for this appointment, your refund will be processed according to the applicable refund policy.

Thank you for choosing us.

Regards,
National Park Team";

            using var smtp = new SmtpClient(
                settings["PrimaryDomain"],
                int.Parse(settings["PrimaryPort"]!))
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    settings["UsernameEmail"],
                    settings["UsernamePassword"])
            };

            await smtp.SendMailAsync(message);
        }
    }
}