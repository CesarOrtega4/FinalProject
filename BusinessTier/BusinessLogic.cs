using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;
using System.Data;
using MySql.Data.MySqlClient;
namespace FinalProject;

internal class BusinessLogic
{
    static async Task Main(string[] args)
    {
        bool _continue = true;
        User user;
        GuiTier appGUI = new GuiTier();
        DataTier database = new DataTier();

        // start GUI
        user = appGUI.Login();


        if (database.LoginCheck(user))
        {

            while (_continue)
            {
                int option = appGUI.Dashboard(user);
                switch (option)
                {
                    // check Email
                    case 1:
                        DataTable tableSendEmail = database.SendEmail(user);
                        if (tableSendEmail != null)
                        appGUI.DisplaySendEmail(tableSendEmail);
                        string serviceConnectionString = "endpoint=https://cesarowekk10coimmunicationservice.communication.azure.com/;accesskey=m8BC6xF/YdMXSgO/zl4tYx1aBrlvb7vVCPVJ81Oq9kzqy41k8vvlUXFgQTDWXMnAAbJI/OEe3YtASZfwBAa9sA==";
                        EmailClient emailClient = new EmailClient(serviceConnectionString);
                        var subject = "Hello CIDM4360";
                        var htmlContent = @"
                    <html>
                        <body>
                            <h1 style=color:red>Testing Email for Azure Email Service</h1>
                            <h4>This is a HTML content</h4>
                            <p>Happy Learning!!</p>
                        </body>
                    </html>";

                    var sender = "DoNotReply@ad98f970-ac91-4594-85b8-751a05feeed0.azurecomm.net";
                

                        Console.WriteLine("Please input recipient email address: ");
                        string? recipient = Console.ReadLine();

                        Console.WriteLine("Sending email with Async no Wait...");
                        EmailSendOperation emailSendOperation = await emailClient.SendAsync(
                        Azure.WaitUntil.Started,
                        sender,
                        recipient,
                        subject,
                        htmlContent);



                        /// Call UpdateStatus on the email send operation to poll for the status manually.
                        try
                        {
                            while (true)
                            {
                                await emailSendOperation.UpdateStatusAsync();
                                if (emailSendOperation.HasCompleted)
                                {
                                    break;
                                }
                                await Task.Delay(10);
                            }

                            if (emailSendOperation.HasValue)
                            {
                                Console.WriteLine($"Email queued for delivery. Status = {emailSendOperation.Value.Status}");
                            }
                            
                        }
                        catch (RequestFailedException ex)
                        {
                            Console.WriteLine($"Email send failed with Code = {ex.ErrorCode} and Message = {ex.Message}");
                            
                        }

                        /// Get the OperationId so that it can be used for tracking the message for troubleshooting
                        string operationId = emailSendOperation.Id;
                        Console.WriteLine($"Email operation id = {operationId}");
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
        }
    }
       
