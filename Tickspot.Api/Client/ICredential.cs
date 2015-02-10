namespace Tickspot.Api.Client
{
    public interface ICredential
    {
        string UserName { get; set; }
        string Password { get; set; }
    }
}
