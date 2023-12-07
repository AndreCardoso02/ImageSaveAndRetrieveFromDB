using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSaveAndRetrieveFromDB.Models
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}