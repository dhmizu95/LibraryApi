using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using LibraryApp.Helpers;
using LibraryApp.Models;
using LibraryApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers {
    [Route("api/Authors")]
    public class AuthorsController : Controller {
        private readonly ILibraryRepository _libraryRepository;


        public AuthorsController(ILibraryRepository libraryRepository) {
            _libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetAuthors() {
            var authorsFromRepo = _libraryRepository.GetAuthors();

            var authors = new List<AuthorDto>();

            foreach (var author in authorsFromRepo) {
                authors.Add(new AuthorDto() {
                    Id = author.Id,
                    Name = $"{author.FirstName} {author.LastName}",
                    Genre = author.Genre,
                    Age = author.DateOfBirth.GetCurrentAge()
                } );
            }
            return new JsonResult(authors);
        }
    }
}
