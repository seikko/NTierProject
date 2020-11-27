using Project.BLL.DesignPatterns.GenericRepository.ConcRep;
using Project.ENTITIES.Models;
using Project.WEBUI.AuthenticationClasses;
using Project.WEBUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.WEBUI.Areas.Admin.Controllers
{
    //[AdminAuthentication]
    
    public class CategoryController : Controller
    {

        CategoryRepository crep;


        public CategoryController()
        {
            crep = new CategoryRepository();
        }
        // GET: Admin/Category
        public ActionResult CategoryList()
        {
            CategoryVM cvm = new CategoryVM
            {
                Categories = crep.GetAll()
            };
            return View(cvm);
        }

        public ActionResult CategoryByID(int id)
        {
            CategoryVM cvm = new CategoryVM()
            {
                Category = crep.Find(id)
            };
            return View(cvm);
        }

        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCategory([Bind(Prefix ="Category")] Category item)
        {
            crep.Add(item);
            return RedirectToAction("CategoryList");
        }


        public ActionResult UpdateCategory(int id)
        {
            CategoryVM cvm = new CategoryVM()
            {
                Category = crep.Find(id)
            };
            return View(cvm);
        }

        [HttpPost]
        public ActionResult UpdateCategory([Bind(Prefix ="Category")] Category item)
        {
            crep.Update(item);
            return RedirectToAction("CategoryList");
        }

        public ActionResult DeleteCategory(int id)
        {
            crep.Delete(crep.Find(id));
            return RedirectToAction("CategoryList");
        }
    }
}