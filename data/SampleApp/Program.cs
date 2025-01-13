using System;
using System.Net;
using System.Net.Mail;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string email = "example@example.com";
            string password = "123456"; // Hard-coded secret

            SendEmail(email, password);
        }

        static void SendEmail(string email, string password)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.example.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = true,
                };

                smtpClient.Send(email, "recipient@example.com", "Test Email", "This is a test email.");
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
