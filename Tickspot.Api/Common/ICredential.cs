namespace Tickspot.Api.Common
{
    public interface ICredential
    {
        string UserName { get; set; }
        string Password { get; set; }
    }
}
