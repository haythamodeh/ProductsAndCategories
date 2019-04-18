using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductsAndCategories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ProductsAndCategories.Controllers
{
    public class HomeController : Controller
    {
        private ProductsAndCategoriesContext dbContext;

        public HomeController(ProductsAndCategoriesContext context)
        {
            dbContext = context;
        }


        [HttpGet("products")]
        public IActionResult Index()
        {
            List<Product> AllProducts = dbContext.Products.ToList();
            ViewBag.AllProducts = AllProducts;
            return View();
        }
        [HttpPost("products/new")]
        public IActionResult AddProduct(Product newProduct)
        {
            if(ModelState.IsValid)
            {
                dbContext.Products.Add(newProduct);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            List<Product> AllProducts = dbContext.Products.ToList();
            ViewBag.AllProducts = AllProducts;
            return View("Index");
        }

        [HttpGet("categories")]
        public IActionResult Category()
        {
            List<Category> AllCategories = dbContext.Categories.ToList();
            ViewBag.AllCategories = AllCategories;
            return View();
        }
        [HttpPost("categories/new")]
        public IActionResult AddCategory(Category newCategory)
        {
            if(ModelState.IsValid)
            {
                dbContext.Categories.Add(newCategory);
                dbContext.SaveChanges();
                return RedirectToAction("Category");
            }
            List<Category> AllCategories = dbContext.Categories.ToList();
            ViewBag.AllCategories = AllCategories;
            return View("Category");
        }

        [HttpGet("view/product/{ProductID}")]
        public IActionResult ViewProduct(int ProductID)
        {
            HttpContext.Session.SetInt32("ProductID", ProductID);
            Product retreivedProduct = dbContext.Products.FirstOrDefault(p => p.ProductID == ProductID);
            ViewBag.Product = retreivedProduct;
            var AllCategoriestOfProduct = dbContext.Products
                .Include(c => c.Categories)
                .ThenInclude(x => x.Category)
                .FirstOrDefault(p => p.ProductID == ProductID);

            List<Category> AllCategories = dbContext.Categories
                .Include(c => c.Products)
                .Where(c => c.Products.All(p => p.ProductID != ProductID))
                .ToList();
            ViewBag.AllCategoriesThatAreAvailable = AllCategories;
            ViewBag.AllCategoriestOfProduct = AllCategoriestOfProduct;
            
            return View("ViewProduct");
        }

        [HttpPost("view/product/addCategory")]
        public IActionResult AddCategoryToProduct(Category addCategoryToProduct)
        {
            System.Console.WriteLine($"***************************{ModelState.IsValid}***************************");
            System.Console.WriteLine($"***************************{addCategoryToProduct.Name}********************");
            System.Console.WriteLine($"***************************{addCategoryToProduct.CategoryID}***************");
            if(ModelState.IsValid)
            {
                Association newAssociation = new Association();
                newAssociation.ProductID = (int)HttpContext.Session.GetInt32("ProductID");
                newAssociation.CategoryID = Int32.Parse(addCategoryToProduct.Name);
                dbContext.Add(newAssociation);
                dbContext.SaveChanges();
                return RedirectToAction("ViewProduct", new {ProductID = HttpContext.Session.GetInt32("ProductID")});
            }
            TempData["errorC"] = "Theres no more categories to add";
            return RedirectToAction("ViewProduct", new {ProductID = HttpContext.Session.GetInt32("ProductID")});
        }


        [HttpGet("view/category/{CategoryID}")]
        public IActionResult ViewCategory(int CategoryID)
        {
            HttpContext.Session.SetInt32("CategoryID", CategoryID);
            Category retreivedCategory = dbContext.Categories.FirstOrDefault(c => c.CategoryID == CategoryID);
            ViewBag.Category = retreivedCategory;
            var AllProductsOfCategory = dbContext.Categories
                .Include(p => p.Products)
                .ThenInclude(x => x.Product)
                .FirstOrDefault(c => c.CategoryID == CategoryID);

            List<Product> AllProducts = dbContext.Products
                .Include(c => c.Categories)
                .Where(c => c.Categories.All(x => x.CategoryID != CategoryID))
                .ToList();
            ViewBag.AllProductsThatAreAvailable = AllProducts;
            ViewBag.AllProductsOfCategories = AllProductsOfCategory;
            
            return View("ViewCategory");
        }

        [HttpPost("view/category/addProduct")]
        public IActionResult AddProductToCategory(Association newAssociation)
        {
            // addProductToCategory.ProductID = addProductToCategory.Product.ProductID;
            // System.Console.WriteLine($"*************************** Model state: {ModelState.IsValid}***************************");
            // System.Console.WriteLine($"*************************** Product Name: {addProductToCategory.ProductID}********************");
            // System.Console.WriteLine($"*************************** Product ID: {addProductToCategory.Product.ProductID}***************");
            // System.Console.WriteLine($"*************************** Product ID: {addProductToCategory.ProductID}***************");
            if(ModelState.IsValid)
            {
                newAssociation.CategoryID = (int)HttpContext.Session.GetInt32("CategoryID");
                dbContext.Add(newAssociation);
                dbContext.SaveChanges();
                return RedirectToAction("ViewCategory", new {CategoryID = HttpContext.Session.GetInt32("CategoryID")});
            }
            // TempData["errorP"] = "Theres no more products to add";
            return RedirectToAction("ViewCategory", new {CategoryID = HttpContext.Session.GetInt32("CategoryID")});
        }

    }
}
