using System.ComponentModel.DataAnnotations;

namespace BuscoProfe.Api.Entities;

public class Sport
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}