using MAS_Shared.MASConstants;

namespace SidekickApp.SignalR
{
    public static class JWTService
    {
        public static Task StoreTokenAsync(string token, JwtTokenType type = JwtTokenType.ShortLived)
        {
            var key = TokenStorageKeys.ForType(type);
            return SecureStorage.SetAsync(key, token);
        }

        public static async Task<(string? Token, JwtTokenType Type)> RetrieveTokenWithSourceAsync()
        {
            foreach (var type in new[] { JwtTokenType.LongLived, JwtTokenType.ShortLived })
            {
                var token = await SecureStorage.GetAsync(TokenStorageKeys.ForType(type));
                if (!string.IsNullOrEmpty(token))
                    return (token, type);
            }

            return (null, JwtTokenType.Unknown);
        }

        public static async Task<string?> RetrieveTokenAsync()
        {
            var (token, _) = await RetrieveTokenWithSourceAsync();
            return token;
        }

        public static void RemoveToken()
        {
            SecureStorage.Remove(TokenStorageKeys.ForType(JwtTokenType.LongLived));
            SecureStorage.Remove(TokenStorageKeys.ForType(JwtTokenType.ShortLived));
        }
    }
}