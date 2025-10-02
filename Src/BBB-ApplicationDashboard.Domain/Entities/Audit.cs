using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BBB_ApplicationDashboard.Domain.Entities
{
    [Table("activity_events")]
    public class ActivityEvent
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string User { get; set; } = default!;

        [Required, MaxLength(100)]
        public string Action { get; set; } = default!;

        public DateTimeOffset Timestamp { get; set; }

        [Required, MaxLength(100)]
        public string Entity { get; set; } = default!;

        [MaxLength(200)]
        public string? EntityIdentifier { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(50)]
        public string? UserVersion { get; set; }

        [Column(TypeName = "jsonb")]
        public JsonDocument Metadata { get; set; } = JsonDocument.Parse("{}");
    }
}
