﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.ViewModels
{
    public class BasketItemViewModel
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
        public string  ProductName { get; set; }
        
        public decimal Price { get; set; }
        public string  Image { get; set; }
    }
}
