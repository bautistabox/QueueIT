namespace QueueIT
{
    public interface IEmailSender
    {
        bool SendEmail(string to, string toName, string from, string fromName, string subject, string body,
            bool isBodyHTML);
    }
}