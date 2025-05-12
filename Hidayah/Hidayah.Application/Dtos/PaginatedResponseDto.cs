using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Dtos
{
    public class PaginatedResponseDto<T>
    {
        public List<T> Items { get; set; }
        public int TotalRecords { get; set; }

        public PaginatedResponseDto(List<T> items, int totalRecords)
        {
            Items = items;
            TotalRecords = totalRecords;
        }
    }
}
