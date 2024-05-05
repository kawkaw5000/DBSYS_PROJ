using EcommerceShop.DAL;
using EcommerceShop.Models.Home;
using EcommerceShop.Repository;
using EcommerceShop.Utils;
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
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();


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

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("UserIndex");
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Login(Tbl_Members u)
        {
            var user = _userRepo.Table.FirstOrDefault(m => m.Username == u.Username && m.Password == u.Password);
            if (user != null)
            {
                if (user.IsDelete == true)
                {
                    ModelState.AddModelError("", "Your account has set to isInactive wait for Admin Aproval.");
                    return View();
                }

                if (!user.IsActive == true)
                {
                    ModelState.AddModelError("", "Your account is not active. Please contact support.");
                    return View();
                }

                FormsAuthentication.SetAuthCookie(u.EmailId, false);
                return RedirectToAction("UserIndex");
            }
            ModelState.AddModelError("", "Email does not Exist or Incorrect Password");

            return View();
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

        public List<SelectListItem> GetMembers(string loggedInUserId)
        {
            List<SelectListItem> list = new List<SelectListItem>();


            var mem = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetAllRecords()
                      .Where(m => m.Username == loggedInUserId);

            foreach (var item in mem)
            {
                list.Add(new SelectListItem { Value = item.id.ToString(), Text = item.Username });
            }

            return list;
        }

        public ActionResult AccountInfo()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Members>().GetMembers());
        }
        public ActionResult AccountEdit(int memberId)
        {
            string loggedInUserId = User.Identity.Name;

            Tbl_Members member = _unitOfWork.GetRepositoryInstance<Tbl_Members>()
                                  .GetAllRecords()
                                  .FirstOrDefault(m => m.Username == loggedInUserId && m.id == memberId);

            if (member == null)
            {
                return RedirectToAction("AccessDenied");
            }

            ViewBag.MembersList = GetMembers(loggedInUserId);
            return View(member);
        }

        [HttpPost]
        public ActionResult AccountEdit(Tbl_Members tbl)
        {

            tbl.ModifiedOn = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Members>().Update(tbl);
            ViewBag.MembersList = GetMembers(User.Identity.Name);
            return RedirectToAction("UserIndex");
        }
        public List<SelectListItem> GetMembersInfo()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var mem = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetAllRecords();
            foreach (var item in mem)
            {
                list.Add(new SelectListItem { Value = item.id.ToString(), Text = item.Username });
            }
            return list;
        }

        [AllowAnonymous]
        public ActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("UserIndex");

            ViewBag.Roles = new SelectList(ctx.Tbl_Roles, "id", "RoleName");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SignUp(Tbl_Members ua, String ConfirmPass, int roleId)
        {
            if (!ua.Password.Equals(ConfirmPass))
            {
                ModelState.AddModelError(String.Empty, "Password not match");
                ViewBag.Roles = new SelectList(ctx.Tbl_Roles, "id", "RoleName");
                return View(ua);
            }

        
            ua.roleId = roleId;

            if (_userManager.SignUp(ua, ref ErrorMessage) != Contracts.ErrorCode.Success)
            {
                ModelState.AddModelError(String.Empty, ErrorMessage);
                ViewBag.Roles = new SelectList(ctx.Tbl_Roles, "id", "RoleName");
                return View(ua);
            }

            int memberId = ua.id;      
            TempData["username"] = ua.Username;
            return RedirectToAction("AddUserInfo", new { memberId = memberId }); ;
        }

        public ActionResult MemberInfo()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_MemberInfo>().GetMemberInfo());
        }

        public ActionResult AddUserInfo(int memberId)
        {
            ViewBag.MemberList = GetMembersInfo();

            ViewBag.MemberId = memberId;
            return View();
        }
        [HttpPost]
        public ActionResult AddUserInfo(Tbl_MemberInfo tbl, HttpPostedFileBase file)
        {

            int memberId = Convert.ToInt32(Request.Form["MemberId"]);

            tbl.MemberId = memberId;

            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/UserImg/"), pic);
                file.SaveAs(path);
            }
            tbl.UserImage = pic;
            _unitOfWork.GetRepositoryInstance<Tbl_MemberInfo>().Add(tbl);
            return RedirectToAction("Index");
        }
        public ActionResult AddUserInfod()
        {
            return View();
        }

    }
}