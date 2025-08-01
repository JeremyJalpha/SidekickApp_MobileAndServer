﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAS_Shared.DBModels
{
    public class Business
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BusinessID { get; set; }

        [Required]
        public int Cellnumber { get; set; }

        [StringLength(50)]
        public string? PricelistPreamble { get; set; }

        [StringLength(25)]
        public string? RegisteredName { get; set; }

        public byte Industry { get; set; }

        [StringLength(25)]
        public string? TradingName { get; set; }

        public long? VATNumber { get; set; }

        [StringLength(70)]
        public string? StreetAddress { get; set; }

        public bool? PostalSameAsStreet { get; set; }

        [StringLength(70)]
        public string? PostalAddress { get; set; }

        [StringLength(40)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(40)]
        [Url]
        public string? Website { get; set; }

        [StringLength(50)]
        public string? Facebook { get; set; }

        [StringLength(50)]
        public string? Twitter { get; set; }

        [StringLength(128)]
        public string? TradingHours { get; set; }

        public long? PayGateID { get; set; }

        [StringLength(15)]
        public string? PayGatePassword { get; set; }

        public byte? Bank { get; set; }

        public int? BranchNumber { get; set; }

        public long? AccountNumber { get; set; }

        public byte? AccountType { get; set; }

        public decimal? GPSLocation { get; set; }

        public DateTime? LocationLastUpdated { get; set; }

        public byte? AverageRating { get; set; }
    }
}