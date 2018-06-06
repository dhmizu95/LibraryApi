using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using LibraryApp.Entities;
using LibraryApp.Models;
using LibraryApp.Services;
using Microsoft.AspNetCore.JsonPatch;
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

        [HttpDelete("{id}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id) {
            if (!_libraryRepository.AuthorExists(authorId)) {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null) {
                return NotFound();
            }

            _libraryRepository.DeleteBook(bookForAuthorFromRepo);
            if(!_libraryRepository.Save())
                throw new Exception($"Deleteing book {id} for author {authorId} failed on save.");

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id,
            [FromBody] BookForCreationDto book) {

            if (book == null) {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId)) {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null) {
                var bookToAdd = _mapper.Map<Book>(book);
                bookToAdd.Id = id;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save()) {
                    throw new Exception($"Adding book {id} for author {authorId} failed on save.");
                }

                var bookToReturn = _mapper.Map<BookDto>(bookToAdd);

                return CreatedAtRoute("GetBookForAuthor",
                    new {authorId, id = bookToReturn.Id},
                    bookToReturn);
            }

            _mapper.Map(book, bookForAuthorFromRepo);

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (!_libraryRepository.Save()) {
                throw new Exception($"Updating book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id,
            [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc) {

            if (patchDoc == null) {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId)) {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null) {

                var bookDto = new BookForUpdateDto();
                patchDoc.ApplyTo(bookDto);

                var bookToAdd = _mapper.Map<Book>(bookDto);
                bookToAdd.Id = id;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save()) {
                    throw new Exception($"Adding book {id} for author {authorId} failed on save.");
                }

                var bookToReturn = _mapper.Map<BookDto>(bookToAdd);

                return CreatedAtRoute("GetBookForAuthor",
                    new { authorId, id = bookToReturn.Id },
                    bookToReturn); 
            }

            var bookToPatch = _mapper.Map<BookForUpdateDto>(bookForAuthorFromRepo);

            patchDoc.ApplyTo(bookToPatch);

            //add validation

            _mapper.Map(bookToPatch, bookForAuthorFromRepo);

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (!_libraryRepository.Save()) {
                throw new Exception($"Patching book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }
    }
}