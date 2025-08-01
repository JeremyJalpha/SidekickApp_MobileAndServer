using Microsoft.AspNetCore.Identity;

namespace MAS_Shared.Data
{
    // Extending IdentityUser with additional fields
    public class ApplicationUser : IdentityUser
    {
        public string? SocialMedia { get; set; }
        public bool? POPIConsent { get; set; }
        public DateTime? DtTmJoined { get; set; }
        public bool IsVerified { get; set; } = false;

        [Obsolete("Use CellNumber instead. PhoneNumber is not reliable for messaging.")]
        public new string? PhoneNumber { get; set; }
        public required string CellNumber { get; set; }

        public string GetUserInfoAsAString()
        {
            string dateTimeJoined = DtTmJoined.HasValue ? DtTmJoined.Value.ToString("yyyy-MM-dd HH:mm:ss")
                : "Not specified";

            string info = string.Format(@"Date Time Joined: {0}
Your Email: {1}
Social: {2}
IsVerified: {3}
Consent: {4}
(needed to store & process your personal data)",
                dateTimeJoined, Email, SocialMedia, IsVerified, POPIConsent);

            return info;
        }
    }
}