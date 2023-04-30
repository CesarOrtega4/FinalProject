using System.Data;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
namespace FinalProject;
class BusinessLogic
{
   
    static async Task Main(string[] args)
    {
        bool _continue = true;
        User user;
        GuiTier appGUI = new GuiTier();
        DataTier database = new DataTier();

        // start GUI
        user = appGUI.Login();

       
        if (database.LoginCheck(user)){

            while(_continue){
                int option  = appGUI.Dashboard(user);
                switch(option)
                {
                    //Send email
                    case 1:
                        DataTable tableSendEmail = database.SendEmail();
                        if(tableSendEmail != null)
                            appGUI.DisplaySendEmail(tableSendEmail);
                            string serviceConnectionString =  "endpoint=https://cesarowekk10coimmunicationservice.communication.azure.com/;accesskey=m8BC6xF/YdMXSgO/zl4tYx1aBrlvb7vVCPVJ81Oq9kzqy41k8vvlUXFgQTDWXMnAAbJI/OEe3YtASZfwBAa9sA==";
                            EmailClient emailClient = new EmailClient(serviceConnectionString);
                            var subject = "Package Status";
                            var emailContent = new EmailContent(subject);
                            emailContent.Html= @"
                                        <html>
                                            <body>
                                                <h1 style=color:red>Your package is ready to be picked up</h1>
                                                <h4>Retrieve your package within the next five days, otherwise it will be returned to the post office.</h4>
                                            </body>
                                        </html>";


                            var sender = "DoNotReply@ad98f970-ac91-4594-85b8-751a05feeed0.azurecomm.net";

                            Console.WriteLine("Please input an email address: ");
                            string? inputEmail = Console.ReadLine();
                            var emailRecipients = new EmailRecipients(new List<EmailAddress> {
                                new EmailAddress(inputEmail) { DisplayName = "Testing" },
                            });

                            var emailMessage = new EmailMessage(sender, emailContent, emailRecipients);

                            try
                            {
                                SendEmailResult sendEmailResult = emailClient.Send(emailMessage);

                                string messageId = sendEmailResult.MessageId;
                                if (!string.IsNullOrEmpty(messageId))
                                {
                                    Console.WriteLine($"Email sent, MessageId = {messageId}");
                                }
                                else
                                {
                                    Console.WriteLine($"Failed to send email.");
                                    return;
                                }

                                // wait max 2 minutes to check the send status for mail.
                                var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(2));
                                do
                                {
                                    SendStatusResult sendStatus = emailClient.GetSendStatus(messageId);
                                    Console.WriteLine($"Send mail status for MessageId : <{messageId}>, Status: [{sendStatus.Status}]");

                                    if (sendStatus.Status != SendStatus.Queued)
                                    {
                                        break;
                                    }
                                    await Task.Delay(TimeSpan.FromSeconds(10));
                                
                                } while (!cancellationToken.IsCancellationRequested);

                                if (cancellationToken.IsCancellationRequested)
                                {
                                    Console.WriteLine($"Email timed out");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in sending email, {ex}");
                            }
                        break;
                    //Records history
                    case 2:
                        DataTable tableRecords = database.ShowRecords();
                        if(tableRecords != null)
                            appGUI.DisplayRecords(tableRecords);
                        break;
                    // Log Out
                    case 3:
                        _continue = false;
                        Console.WriteLine("You have logged out successfully");
                        break;
                    // default: wrong input
                    default:
                        Console.WriteLine("Invalid Input");
                        break;
                }

            }
        }
        else{
            Console.WriteLine("Login Failed");
        }        
    }    
}