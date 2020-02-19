using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HakanERP.Models
{
    public class UlkelerModel:MailModel
    {
        public int UlkeId { get; set; }

        [Display(Name ="Ülke Adı")]
        public string UlkeAdi { get; set; }
      
    }
}