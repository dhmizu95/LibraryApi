using System;
using System.Collections.Generic;
using AutoMapper;
using LibraryApp.Entities;
using LibraryApp.Models;
using LibraryApp.Services;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid id) {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if (authorFromRepo == null)
                return NotFound();

            var author = _mapper.Map<AuthorDto>(authorFromRepo);
            return Ok(author);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author) {
            if (author == null)
                return BadRequest();

            var authorEntity = _mapper.Map<Author>(author);

            _libraryRepository.AddAuthor(authorEntity);
            if (!_libraryRepository.Save())
                throw new Exception("Creating an author failed on save.");

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor",
                new {id = authorToReturn.Id},
                authorToReturn);
        }

        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id) {
            if (_libraryRepository.AuthorExists(id)) {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id) {
            
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if (authorFromRepo == null) {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(authorFromRepo);
            if (!_libraryRepository.Save()) {
                throw new Exception($"Deleting author {id} failed on save.");
            }

            return NoContent();
        }
    }
}