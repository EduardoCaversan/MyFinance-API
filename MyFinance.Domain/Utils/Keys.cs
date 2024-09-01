namespace MyFinance.Domain.Utils
{
    public static class Keys
    {
        public const string ACCESS_TOKEN_SIGNATURE = "E4G03v1q8nT9zWbS0cM4yP7dV6U1A2lLkFqRjH3rZ5pE8vXo9wA6YzN0uO2bQ7r";
        public static string Issuer { get; private set; }
        public static string ApiUrl { get; private set; }

        public static void SetIssuer(string issuer)
        {
            Issuer = issuer ?? "";
        }

        public static void SetApiUrl(string apiUrl)
        {
            ApiUrl = apiUrl ?? "";
        }
    }
}
