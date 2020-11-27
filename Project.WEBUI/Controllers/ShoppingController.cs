using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList; //bizim sayfalama işlemlerini yapmak icin indirdigimiz kütüphaneyi kullandıgımız alan.. Bu klasik PagedList kütüphanesi BackEnd'de her türlü destegi sunarken RazorView'in kullanılabildigi alan sadece MVC'dir...O yüzden
using Project.WEBUI.Models;
using Project.WEBUI.Models.ShoppingTools;
using Project.ENTITIES.Models;
using Project.WEBUI.AuthenticationClasses;
using System.Net.Http;
using System.Threading.Tasks;
using Project.COMMON.Tools;

namespace Project.WEBUI.Controllers
{
    public class ShoppingController : Controller
    {

        OrderRepository oRep;
        ProductRepository pRep;
        CategoryRepository cRep;
        OrderDetailRepository odRep;

        public ShoppingController()
        {
            oRep = new OrderRepository();
            pRep = new ProductRepository();
            cRep = new CategoryRepository();
            odRep = new OrderDetailRepository();
        }

        // GET: Shopping

        //Sayfalama işlemleri yapmak icin Pagination kütüphanesinden yararlanıyoruz (PagedList)
        public ActionResult ShoppingList(int? page,int? categoryID) //nullable int vermemizin sebebi aslında buradaki int'in sayfa sayımızı temsil edecek olmasıdır...Ancak birisi direkt alısveriş sayfasına ulastıgında sayfa sayısını gönderemeyeceginden bu sekilde de Action'ın calısabilmesini istiyoruz.
        {


            //string a = "Cagri";

            //string b = a??"Cagri"; //eger a null ise b'ye null at...null degilse b'ye git a'nın degerini at demektir...

            //            page??1

            //Alt tarafta page?? demek page null geliyorsa demektir

            PAVM pavm = new PAVM
            {
                PagedProducts = categoryID == null? pRep.GetActives().ToPagedList(page??1,9):pRep.Where(x=>x.CategoryID==categoryID).ToPagedList(page??1,9),//eger page parametresi null ise(??) sayfamız 1 olacak ve her sayfada 9 veri listelenecek...
                Categories = cRep.GetActives()
            };

            if (categoryID != null) TempData["catID"] = categoryID;

            return View(pavm);
        }

        public ActionResult AddToCart(int id)
        {
            Cart c = Session["scart"] == null ? new Cart() : Session["scart"] as Cart;

            Product eklenecekUrun = pRep.Find(id);

            CartItem ci = new CartItem
            {
                ID = eklenecekUrun.ID,
                Name = eklenecekUrun.ProductName,
                Price = eklenecekUrun.UnitPrice,
                ImagePath = eklenecekUrun.ImagePath
            };

            c.SepeteEkle(ci);
            Session["scart"] = c;
            return RedirectToAction("ShoppingList");


        }

        public ActionResult CartPage()
        {
            if (Session["scart"]!=null)
            {
                CartPageVM cpvm = new CartPageVM();
                Cart c = Session["scart"] as Cart;
                cpvm.Cart = c;
                return View(cpvm);

            }

            TempData["sepetBos"] = "Sepetinizde ürün bulunmamaktadır";
            return RedirectToAction("ShoppingList");
        }

        public ActionResult DeleteFromCart(int id)
        {
            if (Session["scart"]!=null)
            {
                Cart c = Session["scart"] as Cart;
                c.SepettenSil(id);

                if (c.Sepetim.Count==0)
                {
                    Session.Remove("scart");
                    TempData["sepetBos"] = "Sepetinizde ürün bulunmamaktadır";
                    return RedirectToAction("ShoppingList");
                }
                return RedirectToAction("CartPage");
            }
            return RedirectToAction("ShoppingList");
        }

        //[MemberAuthentication]
        public ActionResult SiparisiOnayla()
        {
            AppUser mevcutKullanici;
            if (Session["member"]!=null)
            {
                mevcutKullanici = Session["member"] as AppUser;
            }
            else
            {
                TempData["anonim"] = "Kullanıcı uye degil";
            }
            return View();

        }




        //https://localhost:44363/api/Payment/ReceivePayment

        [HttpPost]
        public ActionResult SiparisiOnayla(OrderVM ovm)
        {
            bool result;
            Cart sepet = Session["scart"] as Cart;

            ovm.Order.TotalPrice = ovm.PaymentVM.ShoppingPrice = sepet.TotalPrice.Value;

            //API kullanımı 
            //WebApiRestService.WebAPIClient kütüphanesi indirmeyi unutmayın..Yoksa BackEnd'den API'ya istek gönderemezsiniz...
            using (HttpClient client=new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44363/api/");

                Task<HttpResponseMessage> postTask = client.PostAsJsonAsync("Payment/ReceivePayment", ovm.PaymentVM);

                HttpResponseMessage sonuc;

                try
                {
                    sonuc = postTask.Result;
                }
                catch (Exception ex)
                {

                    TempData["baglantiRed"] = "Banka baglantıyı reddetti";
                    return RedirectToAction("ShoppingList");
                }

                if (sonuc.IsSuccessStatusCode)
                {
                    result = true;
                }
                else result = false;

                if (result)
                {
                    if (Session["member"] !=null)
                    {
                        AppUser kullanici = Session["member"] as AppUser;
                        ovm.Order.AppUserID = kullanici.ID;
                        ovm.Order.UserName = kullanici.UserName;
                    }
                    else
                    {
                        ovm.Order.AppUserID = null;
                        ovm.Order.UserName = TempData["anonim"].ToString();
                    }

                    oRep.Add(ovm.Order); //OrderRepository bu noktada Order'i eklerken onun ID'sini olusturuyor...

                    foreach (CartItem item in sepet.Sepetim)
                    {
                        OrderDetail od = new OrderDetail();
                        od.OrderID = ovm.Order.ID;
                        od.ProductID = item.ID;
                        od.TotalPrice = item.SubTotal;
                        od.Quantity = item.Amount;
                        odRep.Add(od);

                        //Stoktan düsmesini istiyorsanız 
                        Product stokDus = pRep.Find(item.ID);
                        stokDus.UnitsInStock -= item.Amount;
                        pRep.Update(stokDus);
                    }

                    TempData["odeme"] = "Siparişiniz bize ulasmıstır...Tesekkür ederiz";

                    MailSender.Send(ovm.Order.Email, body: $"Siparişiniz basarıyla alındı...{ovm.Order.TotalPrice}");

                    return RedirectToAction("ShoppingList");




                }

                else
                {
                    TempData["sorun"] = "Odeme ile ilgili bir sorun olustu..Lutfen bankanız iletişime geciniz";
                    return RedirectToAction("ShoppingList");
                }



            }

            
           
        }



    }
}