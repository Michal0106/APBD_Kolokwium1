using System.ComponentModel.DataAnnotations;

namespace ApbdKolokwium1.Models.Dto;

public class BookGenresDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Title { get; set; }
    public List<string> Genres { get; set; }
}
