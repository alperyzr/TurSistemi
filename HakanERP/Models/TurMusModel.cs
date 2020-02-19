using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HakanERP.Models
{
    public class TurMusModel : MusterilerModel
    {
       
        public  int ID { get; set; }
        public int MusteriID { get; set; }
        public int TurID { get; set; }
        public int PersonelID { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Yolcu Eklendiği Tarih")]
        public DateTime EkTarih { get; set; }

        [Display(Name = "Yolcu Ekleyen Personel")]
        public string EkPersonel { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Yolcu Güncelleme Tarihi")]
        public DateTime GuTarih { get; set; }

        [Display(Name = "Yolcu Güncelleyen Personel")]
        public string GuPersonel { get; set; }
        public int Tutar { get; set; }
    }
}