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
    [Authorize(Roles = "Manager, Admin")]
    public class AdminController : BaseController
    { 
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        public List<SelectListItem> GetMembers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var mem = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetAllRecords();
            foreach (var item in mem)
            {
                list.Add(new SelectListItem { Value = item.id.ToString(), Text = item.Username });
            }
            return list;
        }

        public List<SelectListItem> GetCategory()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecords().Where(c => !c.IsDelete.GetValueOrDefault());
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
      
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // ADMIN USER EDIT------------------------------------------------------------
        [Authorize(Roles = "Admin")]
        public ActionResult Members()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Members>().GetMembers());
        }
        public ActionResult MembersEdit(int memberId)
        {
           
            ViewBag.MembersList = GetMembers();
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Members>().GetFirstorDefault(memberId));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult MembersEdit(Tbl_Members model)
        {
            if (ModelState.IsValid)
            {
                var member = _unitOfWork.GetRepositoryInstance<Tbl_Members>().GetFirstorDefault(model.id);
                if (member != null)
                {
                    member.IsActive = model.IsActive;
                    member.IsDelete = model.IsDelete;
                    // Update other properties if needed

                    _unitOfWork.GetRepositoryInstance<Tbl_Members>().Update(member);
                    _unitOfWork.SaveChanges();
                    return RedirectToAction("Members"); // Redirect to appropriate action
                }
                else
                {
                    ModelState.AddModelError("", "Member not found.");
                }
            }

            // If ModelState is not valid, return to the same view with validation errors
            ViewBag.MembersList = GetMembers();
            return View(model);
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
  
        }
        [HttpPost]
        public ActionResult AddCategory(Tbl_Category tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Add(tbl);
            return RedirectToAction("Categories");
        
        }

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
            // Filter out products with IsDelete = true
            var products = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetProduct().Where(p => !(p.IsDelete ?? false));
            return View(products);
}
        //public ActionResult Product()
        //{
        //    return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetProduct());
        //}
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