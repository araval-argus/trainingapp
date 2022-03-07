using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models.Assets
{
    public class AvatarModel
    {

        public int Id { get; set; }

        public int ProfileId { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public long FileSize { get; set; }

        //public string FileType { get; set; }

        public string FilePath { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}
