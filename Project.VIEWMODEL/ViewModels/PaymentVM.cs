using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.VIEWMODEL.ViewModels
{
    public class PaymentVM
    {
        //Sanal Post Entegrasyonu
        //Normalde bu tarz sınıflar calıstıgınız bankadan aldıgınız dökümanstasyonların kılavuzlugu sayesinde olusturulur. 
        public int ID { get; set; }
        public string CardUserName { get; set; }
        public string SecurityNumber { get; set; }

        public string CardNumber { get; set; }
        public int CardExpiryMonth { get; set; }
        public int CardExpiryYear { get; set; }
        public decimal ShoppingPrice { get; set; }


    }
}
