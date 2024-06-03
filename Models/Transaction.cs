namespace MyFinance.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? Date { get; set; }

        public int? Id_User { get; set; }

        public int? Id_Category { get; set; }

        public virtual Category Categories { get; set; }

        public virtual User Users { get; set; }
    }
}
