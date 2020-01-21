using System.Collections.Generic;

namespace FerProjekt.Models
{
    public class ImageRequestModel
    {
        public string ImageBlob { get; set; }
        public string ImageLink { get; set; }
        public IEnumerable<TaggedFace> TaggedFaces { get; set; }
    }

    public class TaggedFace
    {
        public int? X1 { get; set; }
        public int? X2 { get; set; }
        public int? Y1 { get; set; }
        public int? Y2 { get; set; }

        public string Name { get; set; }
    }
}