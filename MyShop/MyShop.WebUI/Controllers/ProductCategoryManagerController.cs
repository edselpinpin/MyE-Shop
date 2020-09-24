using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.DataAccess.InMemory;
using MyShop.Core.Models;
using MyShop.Core.Contracts;

namespace MyShop.WebUI.Controllers
{
    public class ProductCategoryManagerController : Controller
    {
        IRepository<ProductCategory> prodcontext; // instantiate ProductRepository 

        public ProductCategoryManagerController(IRepository<ProductCategory> Iprodcontext)
        {
            prodcontext = Iprodcontext;
    }

        // GET: ProductManager
        public ActionResult Index()
        {
            List<ProductCategory> productCategory = prodcontext.Collection().ToList();
            return View(productCategory);
        }

        public ActionResult Create()
        {
            ProductCategory product = new ProductCategory();
            return View(product);
        }

        [HttpPost]
        public ActionResult Create(ProductCategory product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);

            }
            else
            {
                prodcontext.Insert(product);
                prodcontext.Commit();
                return RedirectToAction("Index");
            }

        }
        public ActionResult Edit(string Id)
        {
            ProductCategory product = prodcontext.Find(Id);
            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }
        }
        [HttpPost]
        public ActionResult Edit(ProductCategory product, string id)
        {
            ProductCategory productToEdit = prodcontext.Find(id);
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
                    productToEdit.Category = product.Category;
                    prodcontext.Commit();


                    return RedirectToAction("Index");
                }

            }

        }
        public ActionResult Delete(string Id)
        {
            ProductCategory productoDelete = prodcontext.Find(Id);
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
            ProductCategory productoDelete = prodcontext.Find(Id);
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