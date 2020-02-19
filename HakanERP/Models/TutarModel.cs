using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HakanERP.Models
{
    public class TutarModel: UlkelerModel
    {
       
        public int ID { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        [Display(Name = "Tur Adı")]
        public string TurAdi { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime BaslangicTarihi { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Bitiş Tarihi")]
        public DateTime BitisTarihi { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Tur Eklendiği Tarih")]
        public DateTime ET { get; set; }

        [Display(Name = "Tur Ekleyen Personel")]
        public string EP { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Tur Güncelleme Tarihi")]
        public DateTime GT { get; set; }

        [Display(Name = "Turu Güncelleyen Personel")]
        public string GP { get; set; }
    }
}