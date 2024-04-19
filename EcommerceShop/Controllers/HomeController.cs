﻿using EcommerceShop.DAL;
using EcommerceShop.Models.Home;
using EcommerceShop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EcommerceShop.Controllers
{
    public class HomeController : BaseController
    {
        dbMyOnlineShoppingEntities ctx = new dbMyOnlineShoppingEntities();


        [AllowAnonymous]
        public ActionResult Index(string search, int? page)
        {

            HomeIndexViewModel model = new HomeIndexViewModel();
            return View(model.CreateModel(search, 4, page));
        }

        [Authorize(Roles = "User, Manager")]
        public ActionResult UserIndex(string search, int? page)
        {

            HomeIndexViewModel model = new HomeIndexViewModel();
            return View(model.CreateModel(search, 4, page));
        }

        //[AllowAnonymous]
        //public ActionResult Login()
        //{
        //    if (User.Identity.IsAuthenticated)
        //        return RedirectToAction("UserIndex");
        //    return View();
        //}

        

        //[HttpPost]
        //public ActionResult Login(Tbl_Members u)
        //{
        //    var user = _userRepo.Table.FirstOrDefault(m => m.EmailId == u.EmailId && m.Password == u.Password);
        //    if (user != null)
        //    {
        //        if (user.IsDelete == true)
        //        {
        //            ModelState.AddModelError("", "Your account has set to isInactive wait for Admin Aproval.");
        //            return View();
        //        }

        //        if (!user.IsActive == true)
        //        {
        //            ModelState.AddModelError("", "Your account is not active. Please contact support.");
        //            return View();
        //        }

        //        FormsAuthentication.SetAuthCookie(u.EmailId, false);
        //        return RedirectToAction("UserIndex");
        //    }
        //    ModelState.AddModelError("", "Email does not Exist or Incorrect Password");

        //    return View();
        //}

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult Create(Tbl_Members u)
        {
            u.IsActive = false;
            u.IsDelete = true;
            u.CreatedOn = DateTime.Now;
            _userRepo.Create(u);
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "User, Manager")]
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

        [Authorize(Roles = "User, Manager")]
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

        [Authorize(Roles = "User, Manager")]
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
        
    }
}