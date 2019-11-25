namespace GrowRoomEnvironment.Contracts.Email
{
    public interface ISmtpConfig
    {
        string EmailAddress { get; set; }
        string Host { get; set; }
        string Name { get; set; }
        string Password { get; set; }
        int Port { get; set; }
        string Username { get; set; }
        bool UseSSL { get; set; }
    }
}