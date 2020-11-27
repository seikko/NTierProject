using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using Project.WEBUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.ENTITIES.Models;
using Project.COMMON.Tools;

namespace Project.WEBUI.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        ProductRepository pRep;
        CategoryRepository cRep;
        public ProductController()
        {
            pRep = new ProductRepository();
            cRep = new CategoryRepository();
        }
        // GET: Admin/Product
        public ActionResult ProductList(int? id)
        {
            ProductVM pvm = new ProductVM
            {
                Products = id == null ? pRep.GetAll() : pRep.Where(x => x.CategoryID == id),
            };


            return View(pvm);
        }


        public ActionResult AddProduct()
        {
            ProductVM pvm = new ProductVM
            {
                Product = new Product(),
                Categories = cRep.GetActives()
            };

            return View(pvm);
        }

        [HttpPost]
        public ActionResult AddProduct([Bind(Prefix ="Product")] Product item,HttpPostedFileBase resim)
        {
            item.ImagePath = ImageUploader.UploadImage("~/Pictures/", resim);

            pRep.Add(item);
            return RedirectToAction("ProductList");


        }

        public ActionResult UpdateProduct(int id)
        {
            ProductVM pvm = new ProductVM
            {
                Categories = cRep.GetActives(),
                Product = pRep.Find(id)
            };
            return View(pvm);
        }

        [HttpPost]
        public ActionResult UpdateProduct([Bind(Prefix ="Product")] Product item)
        {
            pRep.Update(item);
            return RedirectToAction("ProductList");
        }

        public ActionResult DeleteProduct(int id)
        {
            pRep.Delete(pRep.Find(id));
            return RedirectToAction("ProductList");
        }
    }
}