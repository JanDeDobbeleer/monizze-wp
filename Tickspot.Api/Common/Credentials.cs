namespace Tickspot.Api.Common
{
    public class Credentials: ICredential
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public Credentials()
        {
            UserName = "jan.de.dobbeleer@mobilevikings.com";
            Password = "Sgom1981jj?";
        }
    }
}
