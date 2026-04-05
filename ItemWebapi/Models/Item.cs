using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ItemWebapi.Models
{
    public class Item
    {
      
        public int Id { get; set; }

        public string ItemName { get; set; }
     
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public string Supplier { get; set; }
    }
}