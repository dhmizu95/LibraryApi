using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
            return new JsonResult(authors);
        }
    }
}
