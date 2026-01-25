using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;

namespace MuVi.BLL
{
    public class MovieBLL
    {
        private readonly MovieDAL movieDAL = new MovieDAL();

        public List<MovieDTO> GetListMovies() => movieDAL.GetAll();

        public MovieDTO? GetMovieDetail(int id) => movieDAL.GetById(id);

    }
}