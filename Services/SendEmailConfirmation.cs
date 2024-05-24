

using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SaveCourseProvider.Model;
using System.Configuration;

namespace SaveCourseProvider.Services;

public class SendEmailConfirmation(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;


    public async Task <bool> GenerateEmailRequestAsync(CourseModel courseModel)
    {
        try
        {
            var emailRequest = new EmailRequest()
            {
                To = courseModel.Email,
                Subject = $"Order {courseModel.Title}",
                HtmlBody = $@"
                            <!DOCTYPE html>
                        <html lang='en'>
                        <head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <title>Tack för ditt köp!</title>
                            <style>
                                body {{
                                    font-family: Arial, sans-serif;
                                    line-height: 1.6;
                                }}
                                .container {{
                                    max-width: 600px;
                                    margin: 0 auto;
                                    padding: 20px;
                                    border: 1px solid #ccc;
                                    border-radius: 10px;
                                    background-color: #f9f9f9;
                                }}
                                .header {{
                                    text-align: center;
                                    margin-bottom: 20px;
                                }}
                                .header h1 {{
                                    color: #333;
                                }}
                                .content {{
                                    margin-bottom: 20px;
                                }}
                                .footer {{
                                    text-align: center;
                                    color: #888;
                                }}
                                .button {{
                                    display: inline-block;
                                    padding: 10px 20px;
                                    margin-top: 20px;
                                    background-color: #28a745;
                                    color: white;
                                    text-decoration: none;
                                    border-radius: 5px;
                                }}
                                .button:hover {{
                                    background-color: #218838;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>Tack för ditt köp!</h1>
                                </div>
                                <div class='content'>
                                    <p>Hej {courseModel.Name},</p>
                                    <p>Tack för att du köpt kursen <strong>{courseModel.Title}</strong>. Vi är glada att ha dig som student och ser fram emot att hjälpa dig att nå dina mål.</p>

                                </div>
                                <div class='footer'>
                                    <p>Om du har några frågor, tveka inte att kontakta oss på <a href=""/"">support@silicon.com</a>.</p>
                                    <p>Vänliga hälsningar,<br>Silicon AB</p>
                                </div>
                            </div>
                        </body>
                        </html>
                      ",
                PlainText = $"Tack för att du köpt kursen <strong>{courseModel.Title}" +
                $"</strong>. Vi är glada att ha dig som student och ser fram emot att hjälpa dig att nå dina mål.Om du har några frågor, tveka inte att kontakta oss på support@silicon.com."
            };

            var payload = JsonConvert.SerializeObject(emailRequest);
            if (!string.IsNullOrEmpty(payload))
            {
                string ServiceBus = configuration["ServiceBusConnection"]!;

                ServiceBusClient client = new ServiceBusClient(ServiceBus);
                ServiceBusSender sender = client.CreateSender("email_request");

                ServiceBusMessage message = new ServiceBusMessage(payload);

                await sender.SendMessageAsync(message);

                return true;
            }


        }

        catch
        {

        }
        return false;
    }
}
