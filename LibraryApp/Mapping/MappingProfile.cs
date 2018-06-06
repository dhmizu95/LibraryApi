using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LibraryApp.Entities;
using LibraryApp.Helpers;
using LibraryApp.Models;

namespace LibraryApp.Mapping {
    public class MappingProfile : Profile {
        public MappingProfile() {

            // source to dto
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(
                    src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(
                    src => src.DateOfBirth.GetCurrentAge()));

            CreateMap<Book, BookDto>();

            CreateMap<Book, BookForUpdateDto>();


            // dto to source
            CreateMap<AuthorForCreationDto, Author>();

            CreateMap<BookForCreationDto, Book>();

            CreateMap<BookForUpdateDto, Book>();
        }
    }
}
