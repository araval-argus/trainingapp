using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace ChatApp.Business.Helpers
{
    public static class FileManagement
    {
       
        public static string CreateUniqueFile(string path, IFormFile file)
        {

            string fileName = Guid.NewGuid().ToString();

            var extension = Path.GetExtension(file.FileName);

            using (FileStream fileStream = new FileStream(Path.Combine(path, fileName + extension), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return fileName + extension;
        }
    }
}
