using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HakanERP.Models
{
    public class SemtModel:IlcelerModel
    {
        public int SemtMahId { get; set; }
        public string SemtAdi { get; set; }
        public string MahalleAdi { get; set; }
        public string PostaKodu { get; set; }
        public int ilceId { get; set; }
    }
}