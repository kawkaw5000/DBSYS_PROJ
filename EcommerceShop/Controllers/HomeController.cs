using EcommerceShop.DAL;
using EcommerceShop.Models.Home;
using EcommerceShop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceShop.Controllers
{
    public class HomeController : BaseController
    {
        dbMyOnlineShoppingEntities ctx = new dbMyOnlineShoppingEntities();
       

        public ActionResult Index(string search, int? page)
        {

            HomeIndexViewModel model = new HomeIndexViewModel();
            return View(model.CreateModel(search, 4, page));
        } 

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Tbl_Members u)
        {
            u.IsActive = true;
            u.CreatedOn = DateTime.Now;
            _userRepo.Create(u);
            return RedirectToAction("Index");
        }

        public ActionResult DecreaseQty(int productId)
        {
            if (Session["cart"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = ctx.Tbl_Product.Find(productId);
                foreach (var item in cart)
                {
                    if (item.Product.ProductId == productId)
                    {
                        int prevQty = item.Quantity;
                        if (prevQty > 0)
                        {
                            cart.Remove(item);
                            cart.Add(new Item()
                            {
                                Product = product,
                                Quantity = prevQty - 1
                            });
                        }
                        break;
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Index");
        }

        //public ActionResult AddToCart(int productId, string url)
        //{
        //    if (Session["cart"] == null)
        //    {
        //        List<Item> cart = new List<Item>();
        //        var product = ctx.Tbl_Product.Find(productId);
        //        cart.Add(new Item()
        //        {
        //            Product = product,
        //            Quantity = 1
        //        });
        //        Session["cart"] = cart;
        //    }
        //    else
        //    {
        //        List<Item> cart = (List<Item>)Session["cart"];
        //        var count = cart.Count();
        //        var product = ctx.Tbl_Product.Find(productId);
        //        for (int i = 0; i < count; i++)
        //        {
        //            if (cart[i].Product.ProductId == productId)
        //            {
        //                int prevQty = cart[i].Quantity;
        //                cart.Remove(cart[i]);
        //                cart.Add(new Item()
        //                {
        //                    Product = product,
        //                    Quantity = prevQty + 1
        //                });
        //                break;
        //            }
        //            else
        //            {
        //                var prd = cart.Where(x => x.Product.ProductId == productId).SingleOrDefault();
        //                if (prd == null)
        //                {
        //                    cart.Add(new Item()
        //                    {
        //                        Product = product,
        //                        Quantity = 1
        //                    });
        //                }
        //            }
        //        }
        //        Session["cart"] = cart;
        //    }
        //    return Redirect("Index");
        //}

        public ActionResult AddToCart(int productId, string url)
        {
            if (Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();
                var product = ctx.Tbl_Product.Find(productId);
                cart.Add(new Item()
                {
                    Product = product,
                    Quantity = 1
                });
                Session["cart"] = cart;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = ctx.Tbl_Product.Find(productId);
                var existingItem = cart.FirstOrDefault(item => item.Product.ProductId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cart.Add(new Item()
                    {
                        Product = product,
                        Quantity = 1
                    });
                }

                Session["cart"] = cart;
            }
            return RedirectToAction("Index");
        }


        public ActionResult RemoveFromCart(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            List<Item> itemsToRemove = new List<Item>();

            foreach (var item in cart)
            {
                if (item.Product.ProductId == productId)
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                cart.Remove(itemToRemove);
            }

            Session["cart"] = cart;
            return Redirect("Index");
        }

        //public ActionResult RemoveFromCart(int productId)
        //{
        //    List<Item> cart = (List<Item>)Session["cart"];
        //    foreach (var item in cart)
        //    {
        //        if (item.Product.ProductId == productId)
        //        {
        //            cart.Remove(item);
        //            break;
        //        }
        //    }
        //    Session["cart"] = cart;
        //    return Redirect("Index");
        //}
    }
}