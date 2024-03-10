namespace PoolComVnWebAPI.Common
{
    public interface IEmailSender
    {
        Task SendMailAsync(string email, string username);
    }
}
