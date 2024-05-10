using ApbdKolokwium1.Models.Dto;
using ApbdKolokwium1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApbdKolokwium1.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private IBooksRepository _booksRepository;

    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }

    [HttpGet("/{id}/genres")]
    public async Task<IActionResult> GetBookGenresById(int id)
    {
        if (await _booksRepository.BookDoesntExist(id))
        {
            return NotFound($"Book of given id = {id} doesnt exist id database");
        }
        var result = await _booksRepository.GetBookGenresForId(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddNewBook(NewBookDto newBookDto)
    {
        foreach (var genre in newBookDto.Genres)
        {
            if (await _booksRepository.GenreDoesntExist(genre))
            {
                return NotFound($"Genre with id = {genre} doesnt exist in database");
            }
        }
        var result = await _booksRepository.AddNewBook(newBookDto);
        return Ok(result);
    }
}