using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class HomeController : Controller
    {
        IRepository<Product> prodcontext; // instantiate ProductRepository 
        IRepository<ProductCategory> productCategories;


        public HomeController(IRepository<Product> prodcontextI, IRepository<ProductCategory> productCategoriesI)  
        {

            prodcontext = prodcontextI;
            productCategories = productCategoriesI;
        }
        public ActionResult Details (string Id)
        {
            Product product = prodcontext.Find(Id);
            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }
        }

        public ActionResult Index(string Category=null)
        {
            List<Product> products;
            List<ProductCategory> categories = productCategories.Collection().ToList();
            
            if (Category == null)
            {
                products =  prodcontext.Collection().ToList();
            } else
            {
                products = prodcontext.Collection().Where(p => p.Category == Category).ToList();

            }

            
            ProductListViewModel model = new ProductListViewModel();
            model.Products = products;
            model.ProductCatergories = categories;

           
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}