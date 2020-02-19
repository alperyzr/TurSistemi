using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HakanERP.Models
{
    public class MailModel
    {
        public int ID { get; set; }
        public string Baslik { get; set; }
        public string Icerik { get; set; }
        public DateTime MailGondermeTarihi { get; set; }
        public string MailGonderenPersonel { get; set; }
        public int MusteriID { get; set; }
    }
}