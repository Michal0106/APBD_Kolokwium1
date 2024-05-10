using ApbdKolokwium1.Models.Dto;

namespace ApbdKolokwium1.Repositories;

public interface IBooksRepository
{
    Task<BookGenresDto> GetBookForId(int id);
    Task<BookGenresDto> AddNewBook(NewBookDto newBookDto);
    Task<bool> BookDoesntExist(int id);
    Task<bool> GenreDoesntExist(int id);
}