using Microsoft.AspNetCore.Identity;

namespace MAS_Shared.Data
{
    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public DateTime? DttmIssued { get; set; }
    }

}
