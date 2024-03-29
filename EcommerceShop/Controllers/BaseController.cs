using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EcommerceShop.Repository;
using EcommerceShop.DAL;

namespace EcommerceShop.Controllers
{
    public class BaseController : Controller
    {
        public dbMyOnlineShoppingEntities _db;
        public BaseRepository<Tbl_Members> _userRepo;
        public BaseController()
        {
            _db = new dbMyOnlineShoppingEntities();
            _userRepo = new BaseRepository<Tbl_Members>();
        }
    }
}