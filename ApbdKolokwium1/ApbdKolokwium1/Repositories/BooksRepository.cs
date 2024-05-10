using System.Runtime.InteropServices.ObjectiveC;
using ApbdKolokwium1.Models.Dto;
using Microsoft.Data.SqlClient;

namespace ApbdKolokwium1.Repositories;

public class BooksRepository : IBooksRepository
{
    private IConfiguration _configuration;
    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<BookGenresDto> GetBookGenresForId(int id)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("Default"));
        await con.OpenAsync();
        
        using var com = con.CreateCommand();
        com.CommandText = @"select books.PK, title, name
                            from books join books_genres on books.PK = books_genres.FK_book
                            join genres on books_genres.FK_genre = genres.PK
                            where books.PK = @1";
        com.Parameters.AddWithValue("@1", id);

        BookGenresDto bookAuthorsDto = null;
        
        using var reader = await com.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (bookAuthorsDto is null)
            {
                bookAuthorsDto = new BookGenresDto()
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Genres = new List<string>()
                };
            }

            bookAuthorsDto.Genres.Add(reader.GetString(2));
        }
        if (bookAuthorsDto is null) throw new Exception("book not found");

        return bookAuthorsDto;
    }

    public async Task<BookGenresDto> AddNewBook(NewBookDto newBookDto)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("Default"));
        await con.OpenAsync();
    
        var tran = await con.BeginTransactionAsync();
        
        using var com = con.CreateCommand();
        com.Transaction = (SqlTransaction)tran;
        
        int bookId = -1;
        var genresList = new List<string>();
        try
        {
            com.CommandText = @"insert into books (title)
                                values (@1);
                                select SCOPE_IDENTITY()";
            com.Parameters.AddWithValue("@1", newBookDto.Title);
            bookId = Convert.ToInt32(await com.ExecuteScalarAsync());

            foreach (var genre in newBookDto.Genres)
            {
                com.Parameters.Clear();
                com.CommandText = @"insert into books_genres (FK_book, FK_genre)
                                    values (@1,@2)";
                com.Parameters.AddWithValue("@1", bookId);
                com.Parameters.AddWithValue("@2", genre);

                await com.ExecuteNonQueryAsync();

                com.Parameters.Clear();
                com.CommandText = @"select name from genres where PK = @1";
                com.Parameters.AddWithValue("@1", genre);

                genresList.Add((string)await com.ExecuteScalarAsync());
            }

            await tran.CommitAsync();
            
            return new BookGenresDto()
            {
                Id = bookId,
                Title = newBookDto.Title,
                Genres = genresList
            };
        }
        catch (Exception)
        {
            await tran.RollbackAsync();
        }
        
        return new BookGenresDto()
        {
            Id = bookId,
            Title = newBookDto.Title,
            Genres = genresList
        };
    }

    public async Task<bool> BookDoesntExist(int id)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("Default"));
        await con.OpenAsync();
        
        using var com = con.CreateCommand();
        com.CommandText = @"select 1 from books where PK = @1";
        com.Parameters.AddWithValue("@1", id);

        var result = await com.ExecuteScalarAsync();
        return result is null;
    }

    public async Task<bool> GenreDoesntExist(int id)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("Default"));
        await con.OpenAsync();
        
        using var com = con.CreateCommand();
        com.CommandText = @"select 1 from genres where PK = @1";
        com.Parameters.AddWithValue("@1", id);

        var result = await com.ExecuteScalarAsync();
        return result is null;
    }
}