using AutoMapper;
using BlazorPeliculasServer.DTOs;
using BlazorPeliculasServer.Entities;

namespace BlazorPeliculasServer.Helpers {
    public class AutoMapperProfile: Profile {
        public AutoMapperProfile() {
            CreateMap<Actor, Actor>()
                .ForMember(x => x.Photo, option => option.Ignore());

            CreateMap<Movie, Movie>()
                .ForMember(x => x.Poster, option => option.Ignore());

            CreateMap<VoteMovieDTO, VoteMovie>();
        }
    }
}
