using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HakanERP.Models
{
    public class MusterilerModel : PersonellerModel
    {
       
        public int ID { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        [StringLength(20, ErrorMessage = "{0} alanı en az 3 karakter uzunluğunda olmalıdır!", MinimumLength = 3)]
        [Display(Name = "Müşteri Adı")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        [StringLength(20, ErrorMessage = "{0} alanı en az 2 karakter uzunluğunda olmalıdır!", MinimumLength = 2)]
        [Display(Name = "Müşteri Soyadı")]
        public string Soyad { get; set; }

        [Display(Name = "Telefon")]
        public string Telefon { get; set; }

        [StringLength(250, ErrorMessage = "{0} alanı en az 15 karakter uzunluğunda olmalıdır!", MinimumLength = 15)]
        [Display(Name = "Adres")]
        public string Adres { get; set; }

        [Display(Name = "Telefon 2")]
        public string GSM { get; set; }

        [Required(ErrorMessage = "Email boş bırakılamaz")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                           @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                           @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                           ErrorMessage = "Email adresi geçersiz")]
        [Display(Name = "E-Posta")]
        public string EPosta { get; set; }


        public string Fax { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        [Range(10000000000, 99999999999, ErrorMessage = "T.C No 11 karakterden oluşmaldır!")]
        [Display(Name = "T.C Numarası")]
        public string TcNo { get; set; }


        [Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.DateTime, ErrorMessage = "Doğum tarihi hatalı!")]
        [Display(Name = "Doğum Tarihi")]
        public DateTime DogumTarihi { get; set; }

        [Display(Name = "Pasaport No")]
        public string PasapartNo { get; set; }

        [Display(Name = "Ünvan")]
        public string Unvan { get; set; }

        [Display(Name = "Vergi Dairesi")]
        public string VergiDairesi { get; set; }

        [Display(Name = "Vergi No")]
        public string VergiNo { get; set; }

        [Required(ErrorMessage = "Ülke boş bırakılamaz")]
        [Display(Name = "Ülke")]
        public string Ulke { get; set; }

        [Display(Name = "Yaş")]
        public int Yas { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Pasaport Bitiş Tarihi")]
        public String PasaportBitisTarihi { get; set; }

        public string Tutar { get; set; }
        public string Cinsiyet { get; set; }

        [Display(Name = "Rezervasyon No")]
        public string RezNo { get; set; }



        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Musteri Eklendiği Tarih")]
        public DateTime EklendigiTarih { get; set; }

        [Display(Name = "Müşteri Ekleyen Personel")]
        public string EkleyenPersonel { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Müşteri Güncellendiği Tarih")]
        public DateTime GuncellendigiTarih { get; set; }
        [Display(Name = "Müşteri Güncelleyen Personel")]
        public string GuncelleyenPersonel { get; set; }
        public DateTime MailGöndermeTarihi { get; set; }
    }
}
