using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using LibraryApp.Entities;
using LibraryApp.Models;
using LibraryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers {

    [Route("api/authors/{authorId}/[controller]")]
    public class BooksController : Controller {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IMapper _mapper;

        public BooksController(ILibraryRepository libraryRepository, IMapper mapper) {
            _libraryRepository = libraryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetBooksForAuthor(Guid authorId) {
            if (!_libraryRepository.AuthorExists(authorId)) {
                return NotFound();
            }

            var booksForAuthorFromRepo = _libraryRepository.GetBooksForAuthor(authorId);
            var booksForAuthor = _mapper.Map<IEnumerable<BookDto>>(booksForAuthorFromRepo);

            return Ok(booksForAuthor);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid id, Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            
            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
                return NotFound();

            var bookForAuthor = _mapper.Map<BookDto>(bookForAuthorFromRepo);

            return Ok(bookForAuthor);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody]BookForCreationDto book) {
            if (book == null)
                return BadRequest();

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookEntity = _mapper.Map<Book>(book);
            _libraryRepository.AddBookForAuthor(authorId, bookEntity);

            if (!_libraryRepository.Save()) {
                throw new Exception($"Creating book for author {authorId} failed on save.");
            }

            var bookToReturn = _mapper.Map<BookDto>(bookEntity);
            
            return CreatedAtRoute("GetBookForAuthor",
                new { id = bookToReturn.Id, authorId },
                bookToReturn);
        }
    }
}