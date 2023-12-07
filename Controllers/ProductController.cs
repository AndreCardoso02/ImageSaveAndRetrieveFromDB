using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ImageSaveAndRetrieveFromDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ImageSaveAndRetrieveFromDB.Controllers
{
    [Route("[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _environment;

        public ProductController(ILogger<ProductController> logger, ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _logger = logger;
            _context = context;
            _environment = environment;
        }

        [Route("/Products/")]
        [Route("~/Products/Index")]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [Route("/Product/Add")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("/Product/Add")]
        public IActionResult Create([Bind("Name,Price,ImageFile")] ProductCreateViewModel model)
        {
            string filename = "~/";

            if (model.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Image not uploaded");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                // Salvar na pasta Raiz
                string uploadedFolder = Path.Combine(_environment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filepath = Path.Combine(uploadedFolder, filename);
                model.ImageFile.CopyTo(new FileStream(filepath, FileMode.CreateNew));

                // Salvar na BD

                Product p = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    Image = ConvertFileToByte(model.ImageFile)
                };

                _context.Products.Add(p);
                _context.SaveChanges();
                ViewBag.Success = "Record Added";
                return View();
            }
            ModelState.AddModelError("ImageFile", "Error adding file");
            return View(model);
        }

        [Route("Product/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        private byte[] ConvertFileToByte(IFormFile imagem)
        {
            using (var ms = new MemoryStream())
            {
                imagem.OpenReadStream().CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}