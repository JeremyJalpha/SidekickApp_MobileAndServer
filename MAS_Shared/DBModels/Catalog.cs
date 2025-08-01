using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAS_Shared.DBModels
{
    public class Catalog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CatalogID { get; set; }

        [ForeignKey("Business")]
        public long BusinessID { get; set; }

        [Required]
        [StringLength(450)]
        public string? CreatorUserID { get; set; }

        [Required]
        [StringLength(70)]
        public string? Name { get; set; }

        [StringLength(128)]
        public string? Description { get; set; }

        public DateTime LastUpdated { get; set; }

        // Navigation Property
        public Business? Business { get; set; }
    }
}