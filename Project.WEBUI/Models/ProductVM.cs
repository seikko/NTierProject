using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.WEBUI.Models
{
    public class ProductVM //PAVM ile aynı sekilde acılmıstır..Aslında aynı görevleri yapıyor gizi gözükmetedirler...Fakat PAVM ek olarak alısveriş tarafında pagination işlemlerini de yapacak bir vm sınıfı oldugu icin ProductVM'den ayrılmıstır...Cünkü Admin'de pagination işlemleri zaten var...
    {
        public List<Product> Products { get; set; }
        public Product Product { get; set; }
        public List<Category> Categories { get; set; }

    }
}