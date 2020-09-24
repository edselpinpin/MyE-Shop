using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace MyShop.Services
{ 
    public class BasketService : IBasketService
    {
        IRepository<Product> productContext;
        IRepository<Basket> basketContext;

        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> productContextI, IRepository<Basket> basketContextI)
        
        {
            this.basketContext = basketContextI;
            this.productContext = productContextI;
        }
        
        private Basket GetBasket(HttpContextBase httpContext, bool createIfull)
        { 
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();
            if (cookie != null)
            {
                string basketId = cookie.Value;
                if (!string.IsNullOrEmpty(basketId))
                {
                    basket = basketContext.Find(basketId);
                }
                else if (createIfull)
                {
                    basket = CreateNewBasket(httpContext);
                }

            }
            else
            {
                if (createIfull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }
            return basket;

        }
        
        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContext.Insert(basket);
            basketContext.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AdddToBasket(HttpContextBase httpContext, string productId)
        {
            // building the basket 
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.basketItems.FirstOrDefault(i => i.ProductId == productId);
           

            if (item == null)
            {
                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };
                basket.basketItems.Add(item);
            }
            else
            {
                item.Quantity = item.Quantity + 1;
                 
            }
            basketContext.Commit();

        }
        
        public void RemoveFromBasket(HttpContextBase httpContext,string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.basketItems.FirstOrDefault(i => i.Id == itemId);

            if(item != null)
            {
                basket.basketItems.Remove(item);
                basketContext.Commit();
               
            }
        }

        public List<BasketItemViewModel> GetBasketItems(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            // building the items in the basket 
            if (basket != null)
            {
                var result = (from b in basket.basketItems
                              join
                              p in productContext.Collection() on
                              b.ProductId equals p.Id
                              select new BasketItemViewModel()
                              {
                                  Id = b.Id,
                                  Quantity = b.Quantity,
                                  ProductName = p.Name,
                                  Image = p.Image,
                                  Price = p.Price
                              }
                              ).ToList();
                return result;
                        
            }
            else
            {
                return new List<BasketItemViewModel>();
            }
        }
        public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            BasketSummaryViewModel model = new BasketSummaryViewModel(0, 0);

            if (basket != null)
            {
                int? basketCount = (from item in basket.basketItems
                                    select item.Quantity).Sum(); 

                decimal? basketTotal = (from item in basket.basketItems join p in productContext.Collection() on 
                                        item.ProductId equals p.Id select item.Quantity*p.Price).Sum();
                model.BasketCount = basketCount ?? 0;
                model.BasketTotal = basketTotal ?? decimal.Zero;

                return model;
            }
            else
            {
                return model;
            }
        }

    }
}
