using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAS_Shared.DBModels
{
    public class CatalogItem
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Catalog")]
        public long CatalogID { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("Item")]
        public long ItemID { get; set; }

        // Navigation properties
        public Catalog? Catalog { get; set; }
        public string? Item { get; set; }
    }
}