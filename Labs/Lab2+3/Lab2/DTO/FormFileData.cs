
using Microsoft.AspNetCore.Http;

namespace Lab2.DTO
{
    public class FormFileData
    {
        public IFormFile formFile { get; set; }
    }

    public class ListFormFileData
    {
        public List<IFormFile> formFiles { get; set; }
    }
}

