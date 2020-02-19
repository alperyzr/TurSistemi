using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HakanERP.Models
{
    public class SehirlerModel
    {
        public int SehirId { get; set; }
        public string SehirAdi { get; set; }
        public int PlakaNo { get; set; }
        public int TelefonKodu { get; set; }
        public int RowNumber { get; set; }
    }
}