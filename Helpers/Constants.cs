﻿
namespace AuthCore.Helpers
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Rol = "rol";
                public const string Id = "id";
            }

            public static class JwtClaims
            {
                public static string ApiAccess = "api_access";
            }
        }
    }
}