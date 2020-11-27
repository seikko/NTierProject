using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using Project.COMMON.Tools;
using Project.ENTITIES.Models;
using Project.WEBUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.WEBUI.Controllers
{
    public class HomeController : Controller
    {
        AppUserRepository apRep;
        public HomeController()
        {
            apRep = new AppUserRepository();
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login([Bind(Prefix = "AppUser")] AppUser item)
        {
            AppUser yakalanan = apRep.FirstOrDefault(x => x.UserName == item.UserName);
            string decrypted = DantexCrypt.DeCrypt(yakalanan.Password);

            if (item.Password == decrypted && yakalanan != null && yakalanan.Role == ENTITIES.Enums.UserRole.Admin)
            {
                if (!yakalanan.Active)
                {
                    return AktifKontrol();
                }
                Session["admin"] = yakalanan;
                return RedirectToAction("CategoryList", "Category", new { Area = "Admin" });
            }
            else if (yakalanan.Role == ENTITIES.Enums.UserRole.Member)
            {
                if (!yakalanan.Active)
                {
                   return  AktifKontrol();
                }
                Session["member"] = yakalanan;
                return RedirectToAction("ShoppingList", "Shopping");
            }

            ViewBag.KullaniciYok = "Kullanıcı bulunamadı";
            return View();


        }

     
        private ActionResult AktifKontrol()
        {
            ViewBag.AktifDegil = "Lutfen hesabınızı aktif hale getiriniz...Mailinizi kontrol ediniz";
            return View("Login");
        }

    }
}