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
    public class RegisterController : Controller
    {
        AppUserRepository apRep;
        ProfileRepository apdRep;
        public RegisterController()
        {
            apRep = new AppUserRepository();
            apdRep = new ProfileRepository();
        }
        // GET: Register
        public ActionResult RegisterNow()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterNow(AppUserVM apvm)
        {
            AppUser appUser = apvm.AppUser;
            UserProfile profile = apvm.Profile;

            appUser.Password = DantexCrypt.Crypt(appUser.Password); //sifreyi kriptoladık

            //AppUser.Password = DantexCrypt.DeCrypt(apvm.AppUser.Password);

            if (apRep.Any(x=>x.UserName==appUser.UserName))
            {
                ViewBag.ZatenVar = "Kullanıcı ismi daha önce alınmıs";
                return View();
            }
            else if (apRep.Any(x => x.Email == appUser.Email))
            {
                ViewBag.ZatenVar = "Email zaten kayıtlı";
                return View();
            }

            //Kullanıcı basarılı bir şekilde register işlemini tamamladıysa ona mail gönder

            string gonderilecekMail = "Tebrikler...Hesabınız olusturulmustur. Hesabınızı aktive etmek icin https://localhost:44362/Register/Activation/"+appUser.ActivationCode+" linkine tıklayabilirsiniz.";

            MailSender.Send(appUser.Email, body: gonderilecekMail, subject: "Hesap aktivasyon!");
            apRep.Add(appUser); //öncelikle bunu eklemelisiniz. Cnkü AppUser'in ID'si ilk basta olusmalı... Cünkü biz birebir ilişkide AppUser zorunlu alan Profile ise opsiyonel alandır. Dolayısıyla ilk basta AppUser'in ID'si SaveChanges ile olusmalı ki sonra Profile'i rahatca ekleyebilelim...

            if (!string.IsNullOrEmpty(profile.FirstName) || !string.IsNullOrEmpty(profile.LastName) || !string.IsNullOrEmpty(profile.Address))
            {
                profile.ID = appUser.ID;
                apdRep.Add(profile);
            }

            return View("RegisterOk");



        }

        public ActionResult Activation(Guid id)
        {
            AppUser aktifEdilecek = apRep.FirstOrDefault(x => x.ActivationCode == id);
            if (aktifEdilecek != null)
            {
                aktifEdilecek.Active = true;
                apRep.Update(aktifEdilecek);

                TempData["HesapAktifmi"] = "Hesabınız aktif hale getirildi";
                return RedirectToAction("Login","Home");
            }
            TempData["HesapAktifmi"] = "Aktif edilecek hesap bulunamadı";
            return RedirectToAction("Login","Home");
        }

        public ActionResult RegisterOk()
        {
            return View();
        }
    }
}