using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HakanERP.Models
{
    public class PersonellerModel : TutarModel
    {
      
        public int ID { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        [Display(Name = "Personel Adı")]
        public string Adi { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        [Display(Name = "Personel Soyadı")]
        public string Soyadi { get; set; }

        [Display(Name = "Şİfre")]
        public string Sifre { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; }

        [Display(Name = "E-Posta")]
        public string EMail { get; set; }
    }
}