namespace DriveX_Backend.Utility;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

public class WhatsAppService
{
    private readonly string _accountSid = "US4e25bf6a79cd5030f9f49b933c4445b3"; 
    private readonly string _authToken = "2f636e89d1718b07945cedcee8075a33";  
    private readonly string _twilioWhatsAppNumber = "whatsapp:+16075363529"; 

    public WhatsAppService()
    {
        TwilioClient.Init(_accountSid, _authToken);
    }

    public void SendWhatsAppMessage(string receiverPhoneNumber, string action)
    {
        string messageBody = action.ToLower() switch
        {
            "approved" => "Your request was approved.",
            "rejected" => "Your request was rejected.",
            _ => "Your request status has been updated."
        };

        var message = MessageResource.Create(
            body: messageBody,
            from: new Twilio.Types.PhoneNumber(_twilioWhatsAppNumber),
            to: new Twilio.Types.PhoneNumber($"whatsapp:{receiverPhoneNumber}")
        );

        Console.WriteLine($"Message sent to {receiverPhoneNumber}. SID: {message.Sid}");
    }
}

