using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HakanERP.Models
{
    public class IlcelerModel:SehirlerModel
    {
        public int ilceId { get; set; }
        public int SehirId { get; set; }
        public string IlceAdi { get; set; }
        public string SehirAdi { get; set; }
    }
}