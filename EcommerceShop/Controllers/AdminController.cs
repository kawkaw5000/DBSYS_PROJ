using EcommerceShop.DAL;
using EcommerceShop.Models;
using EcommerceShop.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EcommerceShop.Controllers
{
    [Authorize(Roles = "Manager")]
    public class AdminController : BaseController
    {
        // GET: Admin

        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();


        public List<SelectListItem> GetMembers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var mem = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetAllRecords();
            foreach (var item in mem)
            {
                list.Add(new SelectListItem { Value = item.id.ToString(), Text = item.EmailId });
            }
            return list;
        }

        public List<SelectListItem> GetCategory()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecords();
            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.CategoryId.ToString(), Text = item.CategoryName });
            }
            return list;
        }

        public ActionResult Dashboard()
        {
            return View();
        }
        //[AllowAnonymous]
        //public ActionResult AdminLogin()
        //{
        //    if (User.Identity.IsAuthenticated)
        //        return RedirectToAction("Dashboard");
        //    return View();
        //}
        //[AllowAnonymous]
        //[HttpPost]
        //public ActionResult AdminLogin(Tbl_Members u)
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
        //        return RedirectToAction("Dashboard");
        //    }
        //    ModelState.AddModelError("", "Email does not Exist or Incorrect Password");

        //    return View();
        //}


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // ADMIN USER EDIT------------------------------------------------------------
        public ActionResult Members()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Members>().GetMembers());
        }
        public ActionResult MembersEdit(int memberId)
        {
           
            ViewBag.MembersList = GetMembers();
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Members>().GetFirstorDefault(memberId));
        }
        [HttpPost]
        public ActionResult MembersEdit(Tbl_Members tbl)
        {

            tbl.ModifiedOn = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Members>().Update(tbl);
            return RedirectToAction("Members");
        }



        // ADMIN CATEGORIES EDIT------------------------------------------------------------
        public ActionResult Categories()
        {
            List<Tbl_Category> AllCategories = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecordsIQueryable().Where(i => i.IsDelete == false).ToList();
            return View(AllCategories);
        }

        public ActionResult AddCategory()
        {
            ViewBag.CategoryList = GetCategory();
            return View();
            ////return UpdateCategory(0);
        }
        [HttpPost]
        public ActionResult AddCategory(Tbl_Category tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Add(tbl);
            return RedirectToAction("Categories");
            ////return UpdateCategory(0);
        }

        //public ActionResult UpdateCategory(int categoryId)
        //{
        //    CategoryDetail cd;
        //    if(categoryId != null)
        //    {
        //        cd = JsonConvert.DeserializeObject<CategoryDetail>(JsonConvert.SerializeObject(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetFirstorDefault(categoryId)));
        //    }
        //    else
        //    {
        //        cd = new CategoryDetail();
        //    }
        //    return View("UpdateCategory", cd);
        //}
        public ActionResult CategoryEdit(int catId)
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetFirstorDefault(catId));
        }
        [HttpPost]
        public ActionResult CategoryEdit(Tbl_Category tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Update(tbl);
            return RedirectToAction("Categories");
        }


        // ADMIN PRODUCT EDIT------------------------------------------------------------
        public ActionResult Product()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetProduct());
        }
        public ActionResult ProductEdit(int productId)
        {
            ViewBag.CategoryList = GetCategory();
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetFirstorDefault(productId));
        }
        [HttpPost]
        public ActionResult ProductEdit(Tbl_Product tbl, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                file.SaveAs(path);
            }
            tbl.ProductImage = file != null ? pic : tbl.ProductImage;
            tbl.ModifiedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Update(tbl);
            return RedirectToAction("Product");
        }
        public ActionResult ProductAdd()
        {
            ViewBag.CategoryList = GetCategory();
            return View();
        }
        [HttpPost]
        public ActionResult ProductAdd(Tbl_Product tbl, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                file.SaveAs(path);
            }
            tbl.ProductImage = pic;
            tbl.CreatedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Add(tbl);
            return RedirectToAction("Product");
        }

    }
}