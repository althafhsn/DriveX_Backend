using DriveX_Backend.Entities.Users.Models;

namespace DriveX_Backend.Helpers
{
    public class ConfirmationEmailBody
    {
        public static string SendCarConfirmationMail(string firstName, string lastName, string action)
        {
            return $@"
                     <!DOCTYPE html>
                                    <html>
                                    <head>
                                        <style>
                                            body {{
                                                font-family: Arial, sans-serif;
                                                line-height: 1.6;
                                                background-color: #f9f9f9;
                                                color: #333;
                                                margin: 0;
                                                padding: 0;
                                            }}
                                            .email-container {{
                                                max-width: 600px;
                                                margin: 20px auto;
                                                background-color: #ffffff;
                                                padding: 20px;
                                                border-radius: 8px;
                                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                                            }}
                                            .header {{
                                                text-align: center;
                                                border-bottom: 2px solid #4CAF50;
                                                padding-bottom: 10px;
                                            }}
                                            .header h1 {{
                                                color: #4CAF50;
                                                font-size: 24px;
                                            }}
                                            .content {{
                                                margin: 20px 0;
                                            }}
                                            .content p {{
                                                margin: 10px 0;
                                            }}
                                            .button {{
                                                display: inline-block;
                                                margin: 20px 0;
                                                padding: 10px 20px;
                                                color: #ffffff;
                                                background-color: #4CAF50;
                                                text-decoration: none;
                                                border-radius: 5px;
                                            }}
                                            .footer {{
                                                text-align: center;
                                                margin-top: 20px;
                                                color: #888;
                                                font-size: 12px;
                                            }}
                                        </style>
                                    </head>
                                    <body>
                                        <div class=""email-container"">
                                            <div class=""header"">
                                                <h1>Car Rental Approved!</h1>
                                            </div>
                                            <div class=""content"">
                                                <p>Dear {firstName} {lastName},</p>
                                                <p>We are pleased to inform you that your car rental request has been {action}.
                                               
                                                <p>Please ensure that you bring the required documents during the pick-up process. If you have any questions, feel free to contact us.</p>
                                                
                                            </div>
                                            <div class=""footer"">
                                                <p>Thank you for choosing Drive X.</p>
                                                <p>&copy; 2024 Drive X. All rights reserved.</p>
                                            </div>
                                        </div>
                                    </body>
                                    </html>
   
                    ";
        }

    }
}
