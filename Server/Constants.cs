using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Server
{
    public static class Constants
    {
        public const string Audience = "http://localhost:14744";
        public const string Issuer = Audience;

        public const string SecretValue = "Secret_Here_That_Should_Be_Really_Long_Or_It_Will_Throw_An_Exception";

    }
}
