using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TodoApi.Database;

namespace TodoApi.Models;

public class Todo : IEntityWithTimestamps
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public bool IsComplete { get; set; }

    [Column(TypeName = "timestamp")]
    [DefaultValue("CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
