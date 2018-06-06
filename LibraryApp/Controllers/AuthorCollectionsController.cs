using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LibraryApp.Entities;
using LibraryApp.Helpers;
using LibraryApp.Models;
using LibraryApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers {
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : Controller {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IMapper _mapper;

        public AuthorCollectionsController(ILibraryRepository libraryRepository, IMapper mapper) {
            _libraryRepository = libraryRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection(
            [FromBody] IEnumerable<AuthorForCreationDto> authorCollection) {

            if (authorCollection == null) {
                return BadRequest();
            }
            
            var authorEntities = _mapper.Map<IEnumerable<Author>>(authorCollection);

            foreach (var author in authorEntities) {
                _libraryRepository.AddAuthor(author);
            }

            if (!_libraryRepository.Save()) {
                throw new Exception($"Creating an author collection failed on save");
            }

            var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            var idsAsStrings = String.Join(",", authorCollectionToReturn.Select(a => a.Id));
            return CreatedAtRoute("GetAuthorCollection",
                new {ids = idsAsStrings},
                authorCollectionToReturn);
        }

        [HttpPost("{ids}", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids) {

            if (ids == null)
                return BadRequest();

            var authorIds = ids as Guid[] ?? ids.ToArray();
            var authorEntities = _libraryRepository.GetAuthors(authorIds);
            if (authorIds.Count() != authorEntities.Count()) {
                return NotFound();
            }

            var authorToReturn = _mapper.Map<IEnumerable<Author>>(authorEntities);

            return Ok(authorToReturn);
        }
    }
}