using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ItemWebapi.Models
{
    public class login
    {
        
        public int id { get; set; }

       
        public string username { get; set; }

        
        public string password { get; set; }


     
        public string usertype { get; set; }

       
        public string email { get; set; }

        public string contact { get; set; }

    }
}