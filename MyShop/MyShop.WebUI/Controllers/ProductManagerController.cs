using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using Microsoft.Owin.Security.Provider;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.DataAccess.InMemory;

namespace MyShop.WebUI.Controllers
{
    public class ProductManagerController : Controller
    {
        IRepository<Product> prodcontext; // instantiate ProductRepository 
       IRepository<ProductCategory> productCategories;
        

        public ProductManagerController(IRepository<Product> prodcontextI, IRepository<ProductCategory> productCategoriesI)
        {
            this.prodcontext = prodcontextI;
            this.productCategories = productCategoriesI;
        }
        // start consuming the methods defined in the 
        // GET: ProductManager
        public ActionResult Index()
        {
            List<Product> products = prodcontext.Collection().ToList();
            return View(products);
        }

        public ActionResult Create()
        {
            ProductManagerViewModel viewModel = new ProductManagerViewModel();
            viewModel.Product = new Product();
            viewModel.ProductCatergories = productCategories.Collection();  
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(Product product, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(product);

            }
            else
            {
                if (file != null)
                {

                    product.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//ProductImages//") + product.Image);

                }
                
                prodcontext.Insert(product);
                prodcontext.Commit();
                return RedirectToAction("Index");
            }

        }
        public ActionResult Edit(string Id)
        {
            Product product = prodcontext.Find(Id);
            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                ProductManagerViewModel viewModel = new ProductManagerViewModel();
                viewModel.Product = product;
                viewModel.ProductCatergories = productCategories.Collection();
                return View(viewModel);
            }
        }
        [HttpPost]
        public ActionResult Edit(Product product, string id, HttpPostedFileBase file)
        {
            Product productToEdit = prodcontext.Find(id);
            if (productToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(product);
                }
                else
                {
                    if (file != null)
                    {
                      productToEdit.Image = product.Id + Path.GetExtension(file.FileName);
                     file.SaveAs(Server.MapPath("//Content//ProductImages//") + productToEdit.Image);
                        

                    }
                    productToEdit.Category = product.Category;
                    productToEdit.Description = product.Description;
                    productToEdit.Price = product.Price;
                    
                    productToEdit.Name = product.Name;

                    prodcontext.Commit();


                    return RedirectToAction("Index");
                }

            }

        }
        public ActionResult Delete(string Id)
        {
            Product productoDelete = prodcontext.Find(Id);
            if (productoDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productoDelete);
            }

        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            Product productoDelete = prodcontext.Find(Id);
            if (productoDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                    prodcontext.Delete(Id);
                    prodcontext.Commit();
                    return RedirectToAction("Index");
            }

        }

    }
}