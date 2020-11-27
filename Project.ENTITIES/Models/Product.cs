using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ENTITIES.Models
{
    public class Product:BaseEntity
    {
        //[Required,MinLength(4,ErrorMessage ="{0} sadece {1} karakter olabilir")]
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }

        public string ImagePath { get; set; }

        public int? CategoryID { get; set; }



        //Relational Properties
        public virtual Category Category { get; set; }

        public virtual List<OrderDetail> OrderDetails { get; set; }



    }
}
