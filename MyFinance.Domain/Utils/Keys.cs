namespace MyFinance.Domain.Utils
{
    public static class Keys
    {
        public const string ACCESS_TOKEN_SIGNATURE = "2:Jbils^mK8@[R2-";
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
