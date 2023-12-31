using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TodoApi.Database;

namespace TodoApi.Models;

public class User : IEntityWithTimestamps
{
    public long Id { get; set; }

    [Required, MinLength(8)]
    public string? Password { get; set; }

    [EmailAddress, Required]
    public string? Email { get; set; }

    [Required]
    public string? Name { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
