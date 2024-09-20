using Library_API.Models;
using Library_API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenre _repo;
        private readonly ILogger<GenreController> _logger;

        public GenreController(IGenre repo, ILogger<GenreController> logger)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpPost]
        public ActionResult AddGenre(AddGenre request)
        {
            try
            {
                if(request == null)
                {
                    return BadRequest( new {Message = "Please provide Genre Name"});
                }

                var isAdded = _repo.AddGenre(request);

                if (!isAdded)
                {
                    return BadRequest( new { Message = "Could not add genre" });
                }

                return Ok(new { Message = "Genre added successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Genre Controller while trying to add a genre: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        public ActionResult GetAllGenre()
        {
            try
            {
                var genres = _repo.GetGenres();

                if(genres == null)
                {
                    return NotFound(new { Message = "Could not find any genres" });
                }

                return Ok(genres);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Genre Controller while trying to get all genrees: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get-genrebyid/{id}")]
        public ActionResult GetGenreById(int id)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var genre = _repo.GetGenreById(id);

                if(genre == null)
                {
                    return NotFound();
                }

                return Ok(genre);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Genre Controller while trying get a genre by id: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet("get-genrebyname/{name}")]
        public ActionResult GetGenreByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new { Message = "Provide Genre name" });
                }

                var genre = _repo.GetGenreByName(name);

                if (genre == null)
                {
                    return NotFound();
                }

                return Ok(genre);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Genre Controller while trying get a genre by name: {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateGenre(int id, AddGenre request)
        {
            try
            {
                if(id <= 0 || string.IsNullOrWhiteSpace(request.GenreName))
                {
                    return BadRequest(new { Message = "Provide Genre name or valid id" });
                }

                var genre = _repo.GetGenreById(id);

                if (genre == null)
                {
                    return NotFound();
                }

                var isUpdated = _repo.UpdateGenre(id, request);

                if (!isUpdated)
                {
                    return BadRequest(new {Message = "Could not update Genre"});
                }

                return Ok(new { Message = "Genre updated successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Genre Controller while trying update a genre : {ex}", ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteGenre(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Provide valid id" });
                }

                var genre = _repo.GetGenreById(id);

                if (genre == null)
                {
                    return NotFound();
                }

                var isDeleted = _repo.DeleteGenre(id);

                if (!isDeleted)
                {
                    return BadRequest(new { Message = "Could not delete Genre" });
                }

                return Ok(new { Message = "Genre deleted successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Genre Controller while trying get a genre by id: {ex}", ex.Message);
                return BadRequest();
            }
        }
    }
}
