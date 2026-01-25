using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using MuVi.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuVi.ViewModels
{
    public class MovieViewModel : BaseViewModel
    {
        private MovieBLL movieBLL = new MovieBLL();

        // Thuộc tính để DataGrid Binding vào
        public ObservableCollection<MovieDTO> MovieList { get; set; }

        public MovieViewModel()
        {
            MovieList = new ObservableCollection<MovieDTO>();
            LoadData();
        }

        public void LoadData()
        {
            // Lấy dữ liệu từ tầng BLL
            var data = movieBLL.GetListMovies();

            MovieList.Clear();
            foreach (var item in data)
            {
                MovieList.Add(item);
            }
        }
    }
}
