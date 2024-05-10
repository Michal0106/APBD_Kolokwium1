using System.ComponentModel.DataAnnotations;

namespace ApbdKolokwium1.Models.Dto;

public class NewBookDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; }
    public List<int> Genres { get; set; }
}