namespace Engine.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(string attachmentPath);
    }
}