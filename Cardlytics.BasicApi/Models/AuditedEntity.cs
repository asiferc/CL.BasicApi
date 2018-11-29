using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cardlytics.BasicApi.Models
{
    /// <summary>
    /// Basic properties that represent auditing-related fields in a persisted entity.
    /// </summary>
    public class AuditedEntity : BaseEntity
    {
        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedDate { get; set; }
    }
}