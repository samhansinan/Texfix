using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ItemWebapi.Models
{
    public class Quotation
    {

        public int Id { get; set; }

        public string ItemName { get; set; }

        public int Quantity { get; set; }
  
        public string Supplier { get; set; }

        public string Description { get; set; }

       
        public DateTime Date { get; set; }

        public string Status { get; set; }
        public string ResponseInfo { get; set; }

        public string ResponseStatus {  get; set; }



    }
}