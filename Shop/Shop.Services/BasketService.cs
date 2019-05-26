using Shop.Core.Contracts;
using Shop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Shop.Services
{
    class BasketService
    {
        IRepository<Product> productContext;
        IRepository<Basket> basketContext;

        public const string basketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> ProductContext,
        IRepository<Basket> BasketContext)
        {
            this.productContext = ProductContext;
            this.basketContext = BasketContext;

        }
        private Basket GetBasket(HttpContextBase httpContext, bool creatIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(basketSessionName);

            Basket basket = new Basket();

            if(cookie != null)
            {
                string basketId = cookie.Value;
                if (!string.IsNullOrEmpty(basketId))
                {
                    basket = basketContext.Find(basketId);
                }
                else
                {
                    if (creatIfNull)
                    {
                        basket = CreatNewBasket(httpContext);
                    }
                }
            }
            else
            {
                if (creatIfNull)
                {
                    basket = CreatNewBasket(httpContext);
                }
            }

            return basket;
        }

        private Basket CreatNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContext.Insert(basket);
            basketContext.Commit();


            HttpCookie cookie = new HttpCookie(basketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };

                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quantity++;
            }

            basketContext.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if(item != null)
            {
                basket.BasketItems.Remove(item);
                basketContext.Commit();
            }
        }
    }
}
