using System;
using System.Collections.Generic;
using AutoMapper;
using LibraryApp.Models;
using LibraryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers {
    [Route("api/Authors")]
    public class AuthorsController : Controller {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IMapper _mapper;


        public AuthorsController(ILibraryRepository libraryRepository, IMapper mapper) {
            _libraryRepository = libraryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAuthors() {

            var authorsFromRepo = _libraryRepository.GetAuthors();
            var authors = _mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthor(Guid id) {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if (authorFromRepo == null)
                return NotFound();

            var author = _mapper.Map<AuthorDto>(authorFromRepo);
            return Ok(author);
        }
    }
}