using Project.ENTITIES.Models;
using Project.VIEWMODEL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.WEBUI.Models.ShoppingTools
{
    public class OrderVM
    {
        public PaymentVM PaymentVM { get; set; }
        public Order Order { get; set; }

    }
}