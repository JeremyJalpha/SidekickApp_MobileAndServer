using MAS_Shared.Data;
using MAS_Shared.DBModels;
using MAS_Shared.MASConstants;

namespace CommandBot.Models
{
    public class BusinessContextFactory
    {
        private readonly ILogger<BusinessContextFactory> _logger;

        public BusinessContextFactory(ILogger<BusinessContextFactory> logger)
        {
            _logger = logger;
        }

        public static string GetCommandMenu()
        {
            return "Main Menu:\n\n" +
                   "#menu - Show this menu\n" +
                   "#shop - Show shop details\n" +
                   "#userinfo - Show user information\n" +
                   "#driverlogin - Driver login command\n\n" +
                   "#update email:<email> - Update user email\n" +
                   "#update social:<social> - Update user social media\n" +
                   "#update consent:<consent> - Update user POPI consent\n" +
                   "#update order:<order> - Update an order\n\n" +
                   "#new sighting:<sighting> - Create a new sighting";
        }

        public BusinessContext CreateBusinessContext(AppDbContext dbContext, string baseurl, string cellNumber, ChatChannelType chatChannelType)
        {
            if (!int.TryParse(cellNumber, out int parsedCellNumber))
            {
                throw new ArgumentException("Cellnumber is not a valid integer.", nameof(cellNumber));
            }

            // Start with a default context
            var businessContext = new BusinessContext(parsedCellNumber, baseurl, GetCommandMenu, chatChannelType);

            // Try to fetch the business
            var business = dbContext.Businesses.FirstOrDefault(b => b.Cellnumber.ToString() == cellNumber);
            if (business == null)
            {
                _logger.LogWarning($"No business found for Cellnumber {cellNumber}. Creating default business context.");
                return businessContext;
            }

            // Fetch the catalog for the business, if available
            var catalog = dbContext.Catalogs.FirstOrDefault(c => c.BusinessID == business.BusinessID);
            List<CatalogItem> catalogItems = new List<CatalogItem>();
            if (catalog == null)
            {
                _logger.LogWarning($"No catalog found for business with Cellnumber {cellNumber}.");
            }
            else
            {
                catalogItems = dbContext.CatalogItems
                                   .Where(ci => ci.CatalogID == catalog.CatalogID)
                                   .ToList();
            }

            // Fetch the pricelist preamble from the business record
            string preamble = business.PricelistPreamble ?? string.Empty;
            if (string.IsNullOrWhiteSpace(preamble))
            {
                _logger.LogWarning($"Pricelist Preamble is not set for business with Cellnumber {cellNumber}. Using default.");
                preamble = "Welcome to our shop! Here are our prices:";
            }

            // Create a fully populated BusinessContext with the gathered data.
            businessContext = new BusinessContext(parsedCellNumber, baseurl, GetCommandMenu, chatChannelType)
            {
                Business = business,
                // Set Catalog to an empty string or to a specific catalog property if you have one.
                Catalog = string.Empty,
                PrclstPreamble = preamble,
                CatalogItems = catalogItems
            };

            return businessContext;
        }
    }
}