using HakanERP.Models;
using NZF_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace HakanERP.Controllers
{
    public class MusteriController : Controller
    {
        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase excelFile)
        {

            if (excelFile == null
            || excelFile.ContentLength == 0)
            {
                ViewBag.Error = "Lütfen dosya seçimi yapınız.";

                return RedirectToAction("Index");
            }
            else
            {

                if (excelFile.FileName.EndsWith("xls")
                || excelFile.FileName.EndsWith("xlsx"))
                {

                    string path = Server.MapPath("~/Content/" + excelFile.FileName);

                    //Dosya kontrol edilir, varsa silinir.
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    //Excel path altına kaydedilir.
                    excelFile.SaveAs(path);

                    //+Exceli açıyoruz
                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;
                    //-


                    List<MusterilerModel> localList = new List<MusterilerModel>();
                    for (int i = 2; i <= range.Rows.Count; i++)
                    {
                        List<TurMusModel> tmmm = new List<TurMusModel>();
                        DateTime dt = Convert.ToDateTime(((Excel.Range)range.Cells[i, 2]).Text);
                        string dt1 = dt.ToString("yyyy-MM-dd");
                        string sql = "  Select * from Turlar where BaslangicTarihi='" + dt1 + "'";
                        DataAccesBase db = new DataAccesBase();
                        DataTable model = db.ReturnDataTable(sql);
                        int MusID, TurID;
                        foreach (DataRow item in model.Rows)
                        {

                            string sqll = "select * from Musteriler where TcNo='" + ((Excel.Range)range.Cells[i, 10]).Text + "'";
                            if (sqll != "")
                            {
                                DataAccesBase dbb = new DataAccesBase();
                                DataTable modell = dbb.ReturnDataTable(sqll);

                                foreach (DataRow itemm in modell.Rows)
                                {
                                    MusID = (int)itemm["ID"];
                                    TurID = (int)item["ID"];
                                    TurMus tm = new TurMus();
                                    tm.MusteriID = MusID;
                                    tm.TurID = TurID;
                                    tm.PersonelID = 1008;
                                    tm.Kaydet();

                                }
                            }
                            else
                            {
                                TurMus tm = new TurMus();
                                tm.MusteriID = 1;
                                tm.TurID = 1;
                                tm.PersonelID = 1008;
                                tm.Kaydet();

                            }

                        }

                    }

                    application.Quit();


                    ViewBag.ListDetay = localList;

                    return View("Listele");
                }
                else
                {
                    ViewBag.Error = "Dosya tipiniz yanlış, lütfen '.xls' yada '.xlsx' uzantılı dosya yükleyiniz.";

                    return View();
                }
            }
        }

        public ActionResult Index()
        {

            if (Session["KullaniciAdi"] != null)
            {


                List<MusterilerModel> MusteriList = new List<MusterilerModel>();
                string sql;
                DataAccesBase db = new DataAccesBase();
                sql = "Select * from Musteriler order by ID desc";
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    MusteriList.Add(new MusterilerModel
                    {
                        ID = Convert.ToInt32(item["ID"]),
                        Ad = item["Ad"].ToString(),
                        Soyad = item["Soyad"].ToString(),
                        TcNo = item["TcNo"].ToString(),
                        Telefon = item["Telefon"].ToString(),
                        EPosta = item["EPosta"].ToString(),

                        EklendigiTarih = (DateTime)item["EklendigiTarih"]

                    });
                }


                return View(MusteriList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Ekle()
        {
            if (Session["KullaniciAdi"] != null)
            {
                List<UlkelerModel> ulkeler = new List<Models.UlkelerModel>();
                string sql = "Select * from Ulkeler as u left join Musteriler as m on u.UlkeAdi = m.Ulke";
                DataAccesBase db = new DataAccesBase();
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    ulkeler.Add(new UlkelerModel
                    {
                        UlkeId = (int)item["UlkeId"],
                        UlkeAdi = item["UlkeAdi"].ToString()
                    });
                }

                ViewBag.Ulke = ulkeler;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        [HttpPost]
        public ActionResult Ekle(MusterilerModel m, int ID = 0)
        {

            List<UlkelerModel> ulkeler = new List<Models.UlkelerModel>();
            string sqls = "Select * from Ulkeler as u left join Musteriler as m on u.UlkeAdi = m.Ulke";
            DataAccesBase dbs = new DataAccesBase();
            DataTable models = dbs.ReturnDataTable(sqls);
            foreach (DataRow item in models.Rows)
            {
                ulkeler.Add(new UlkelerModel
                {
                    UlkeId = (int)item["UlkeId"],
                    UlkeAdi = item["UlkeAdi"].ToString()
                });
            }

            ViewBag.Ulke = ulkeler;

            if (ID == 0)
            {
                string sql = "Select * from Musteriler as m left join Ulkeler as u on m.Ulke=u.UlkeId where m.TcNo='" + m.TcNo + "'";
                DataAccesBase db = new DataAccesBase();
                DataTable model = db.ReturnDataTable(sql);
                if (model.Rows.Count == 0)
                {
                    Musteriler mus = new Musteriler(m.ID);

                    mus.Ad = m.Ad;
                    string Ad = mus.Ad.ToUpper();
                    mus.Ad = karakterCevir(Ad);

                    mus.Adres = m.Adres;
                    if (mus.Adres != null)
                    {
                        string Adres = mus.Adres.ToUpper();
                        mus.Adres = karakterCevir(Adres);

                    }



                    mus.DogumTarihi = Convert.ToDateTime(m.DogumTarihi);
                    mus.EPosta = m.EPosta;
                    mus.Fax = m.Fax;
                    mus.GSM = m.GSM;
                    mus.PasapartNo = m.PasapartNo;

                    mus.Soyad = m.Soyad;
                    string Soyad = mus.Soyad.ToUpper();
                    mus.Soyad = karakterCevir(Soyad);

                    mus.TcNo = m.TcNo;
                    mus.Telefon = m.Telefon;

                    mus.Unvan = m.Unvan;
                    if (mus.Unvan != null)
                    {
                        string Unvan = mus.Unvan.ToUpper();
                        mus.Unvan = karakterCevir(Unvan);

                    }

                    mus.VergiDairesi = m.VergiDairesi;
                    if (mus.VergiDairesi != null)
                    {
                        string VergiDairesi = mus.VergiDairesi.ToUpper();
                        mus.VergiDairesi = karakterCevir(VergiDairesi);


                    }
                    mus.VergiNo = m.VergiNo;
                    mus.Ulke = m.Ulke;
                    mus.Yas = m.Yas;
                    mus.Yas = ((DateTime.Now.Year) - (mus.DogumTarihi.Year));

                    mus.PasaportBitisTarihi = m.PasaportBitisTarihi;

                    mus.RezNo = m.RezNo;
                    mus.Cinsiyet = m.Cinsiyet;

                    mus.EklendigiTarih = m.EklendigiTarih;
                    mus.EklendigiTarih = DateTime.Now;
                    mus.EkleyenPersonel = m.EkleyenPersonel;
                    mus.EkleyenPersonel = Session["PersonelAd"].ToString().ToUpper() + " " + Session["PersonelSoyAd"].ToString().ToUpper();

                    mus.Kaydet();
                    Response.Write("<script language='javascript'>alert('Kayıt Başarıyla Eklendi.');</script>");


                    return View();
                }
                else
                {
                    Response.Write("<script>alert('Böyle bir T.C numarası Mevcut. Lütfen Kimlik numaranızı kontrol edin')</script>");
                    return View();
                }

            }
            else
            {
                Musteriler mus = new Musteriler(m.ID);

                mus.Ad = m.Ad;
                string Ad = mus.Ad.ToUpper();
                mus.Ad = karakterCevir(Ad);

                mus.Adres = m.Adres;
                if (mus.Adres != null)
                {
                    string Adres = mus.Adres.ToUpper();
                    mus.Adres = karakterCevir(Adres);
                }

                mus.DogumTarihi = Convert.ToDateTime(m.DogumTarihi);
                mus.EPosta = m.EPosta;
                mus.Fax = m.Fax;
                mus.GSM = m.GSM;
                mus.PasapartNo = m.PasapartNo;

                mus.Soyad = m.Soyad;
                string Soyad = mus.Soyad.ToUpper();
                mus.Soyad = karakterCevir(Soyad);

                mus.TcNo = m.TcNo;
                mus.Telefon = m.Telefon;

                mus.Unvan = m.Unvan;
                if (mus.Unvan != null)
                {
                    string Unvan = mus.Unvan.ToUpper();
                    mus.Unvan = karakterCevir(Unvan);
                }

                mus.VergiDairesi = m.VergiDairesi;
                if (mus.VergiDairesi != null)
                {
                    string VergiDairesi = mus.VergiDairesi.ToUpper();
                    mus.VergiDairesi = karakterCevir(VergiDairesi);

                }
                mus.VergiNo = m.VergiNo;
                mus.Ulke = m.Ulke;
                mus.Yas = m.Yas;
                mus.Yas = ((DateTime.Now.Year) - (mus.DogumTarihi.Year));
                mus.PasaportBitisTarihi = m.PasaportBitisTarihi;
                mus.RezNo = m.RezNo;
                mus.Cinsiyet = m.Cinsiyet;

                mus.GuncellendigiTarih = m.GuncellendigiTarih;
                mus.GuncellendigiTarih = DateTime.Now;
                mus.GuncelleyenPersonel = m.GuncelleyenPersonel;
                mus.GuncelleyenPersonel = Session["PersonelAd"].ToString() + " " + Session["PersonelSoyAd"].ToString();
                string gp = mus.GuncelleyenPersonel.ToUpper();
                mus.GuncelleyenPersonel = gp;
                mus.Kaydet();

                Response.Write("<script language='javascript'>alert('Kayıt Başarıyla Güncellendi');</script>");
                return View();
            }
        }

        public ActionResult Guncelle(MusterilerModel s, int ID)
        {
            List<UlkelerModel> ulkeler = new List<Models.UlkelerModel>();
            string sqls = "Select * from Ulkeler as u left join Musteriler as m on u.UlkeAdi = m.Ulke";
            DataAccesBase dbs = new DataAccesBase();
            DataTable models = dbs.ReturnDataTable(sqls);
            foreach (DataRow item in models.Rows)
            {
                ulkeler.Add(new UlkelerModel
                {
                    UlkeId = (int)item["UlkeId"],
                    UlkeAdi = item["UlkeAdi"].ToString(),

                });
                s.Ulke = item["UlkeAdi"].ToString();
            }

            ViewBag.Ulke = ulkeler;
            if (Session["KullaniciAdi"] != null)
            {

                MusterilerModel m = new Models.MusterilerModel();
                Musteriler mus = new Musteriler(ID);

                m.Ad = mus.Ad;
                m.Adres = mus.Adres;
                m.DogumTarihi = mus.DogumTarihi;
                m.EPosta = mus.EPosta;
                m.Fax = mus.Fax;
                m.GSM = mus.GSM;
                m.PasapartNo = mus.PasapartNo;
                m.Soyad = mus.Soyad;
                m.TcNo = mus.TcNo;
                m.Telefon = mus.Telefon;
                m.Unvan = mus.Unvan;
                m.VergiDairesi = mus.VergiDairesi;
                m.VergiNo = mus.VergiNo;
                m.Ulke = s.Ulke;
                m.Ulke = mus.Ulke;
                m.Yas = mus.Yas;
                m.PasaportBitisTarihi = mus.PasaportBitisTarihi;
                m.Cinsiyet = mus.Cinsiyet;
                m.RezNo = mus.RezNo;


                return View("Ekle", m);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public JsonResult Sil(int id)
        {
            Musteriler b = new NZF_DAL.Musteriler(id);
            var deleteState = b.Delete();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detay(int ID)
        {


            if (Session["KullaniciAdi"] != null)
            {
                List<MusterilerModel> MList = new List<MusterilerModel>();
                string sql;
                DataAccesBase db = new DataAccesBase();
                sql = "Select * from Ulkeler as u join Musteriler as m on u.UlkeId = m.Ulke where m.ID=" + ID;

                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    MList.Add(new MusterilerModel
                    {
                        ID = (int)item["ID"],
                        Ad = item["Ad"].ToString(),
                        Adres = item["Adres"].ToString(),
                        DogumTarihi = (DateTime)item["DogumTarihi"],
                        EPosta = item["EPosta"].ToString(),
                        Fax = item["Fax"].ToString(),
                        GSM = item["GSM"].ToString(),
                        PasapartNo = item["PasapartNo"].ToString(),
                        Soyad = item["Soyad"].ToString(),
                        TcNo = item["TcNo"].ToString(),
                        Telefon = item["Telefon"].ToString(),
                        UlkeAdi = item["UlkeAdi"].ToString(),
                        Unvan = item["Unvan"].ToString(),
                        VergiDairesi = item["VergiDairesi"].ToString(),
                        VergiNo = item["VergiNo"].ToString(),
                        Yas = (int)item["Yas"],
                        PasaportBitisTarihi = item["PasaportBitisTarihi"].ToString(),
                        Cinsiyet = item["Cinsiyet"].ToString(),
                        RezNo = item["RezNo"].ToString(),

                        EklendigiTarih = (DateTime)item["EklendigiTarih"],
                        EkleyenPersonel = item["EkleyenPersonel"].ToString(),
                        GuncellendigiTarih = (DateTime)item["GuncellendigiTarih"],
                        GuncelleyenPersonel = item["GuncelleyenPersonel"].ToString()

                    });
                }

                return View(MList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult TurKatilim(int ID)
        {
            if (Session["KullaniciAdi"] != null)
            {
                List<TurMusModel> KatilimTur = new List<TurMusModel>();
                string sql2 = "Select * from TurMus as tm left join Personeller as p on tm.PersonelID=p.ID  join Musteriler as m on tm.MusteriID=m.ID  join Turlar as t on tm.TurID=t.ID where m.ID=" + ID + " order by tm.ID desc";
                DataAccesBase db = new DataAccesBase();
                DataTable model = db.ReturnDataTable(sql2);
                foreach (DataRow item in model.Rows)
                {
                    KatilimTur.Add(new TurMusModel
                    {
                        ID = (int)item["ID"],
                        EkTarih = (DateTime)item["EkTarih"],
                        MusteriID = ID,
                        Ad = item["Ad"].ToString(),
                        Soyad = item["Soyad"].ToString(),

                        TurID = (int)item["TurID"],
                        TurAdi = item["TurAdi"].ToString(),
                        BaslangicTarihi = (DateTime)item["BaslangicTarihi"],
                        BitisTarihi = (DateTime)item["BitisTarihi"],

                        PersonelID = (int)item["PersonelID"],
                        Adi = item["Adi"].ToString(),
                        Soyadi = item["Soyadi"].ToString(),
                        Tutar = (int)item["Tutar"]
                    });
                }

                return View(KatilimTur);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult YolcuDetay(TurMusModel m)
        {
            if (Session["KullaniciAdi"] != null)
            {
                string sql;
                List<TurMusModel> list = new List<Models.TurMusModel>();
                sql = "Select * from TurMus as tm left join Musteriler as m on tm.MusteriID=m.ID left join Personeller as p on tm.PersonelID= p.ID left join Turlar as t on tm.TurID=t.ID  left join Ulkeler as u on u.UlkeId = m.Ulke where tm.ID=" + m.ID;
                DataAccesBase db = new DataAccesBase();
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    list.Add(new TurMusModel
                    {


                        Ad = item["Ad"].ToString(),
                        Soyad = item["Soyad"].ToString(),
                        Adi = item["Adi"].ToString(),
                        Soyadi = item["Soyadi"].ToString(),
                        TurAdi = item["TurAdi"].ToString(),
                        BaslangicTarihi = (DateTime)item["BaslangicTarihi"],
                        BitisTarihi = (DateTime)item["BitisTarihi"],
                        GSM = item["GSM"].ToString(),
                        TcNo = item["TcNo"].ToString(),
                        EPosta = item["EPosta"].ToString(),
                        Telefon = item["Telefon"].ToString(),
                        PasapartNo = item["PasapartNo"].ToString(),
                        PasaportBitisTarihi = item["PasaportBitisTarihi"].ToString(),
                        VergiDairesi = item["VergiDairesi"].ToString(),
                        VergiNo = item["VergiNo"].ToString(),
                        Yas = (int)item["Yas"],
                        Cinsiyet = item["Cinsiyet"].ToString(),
                        DogumTarihi = (DateTime)item["DogumTarihi"],
                        EMail = item["EMail"].ToString(),
                        UlkeAdi = item["UlkeAdi"].ToString(),
                        Adres = item["Adres"].ToString(),
                        Unvan = item["Unvan"].ToString(),
                        Fax = item["Fax"].ToString(),
                        RezNo = item["RezNo"].ToString(),
                        Tutar = (int)item["Tutar"],
                        EkTarih = (DateTime)item["EkTarih"],
                        EkPersonel = item["EkPersonel"].ToString(),
                        GuTarih = (DateTime)item["GuTarih"],
                        GuPersonel = item["GuPersonel"].ToString(),
                        ET = (DateTime)item["ET"],
                        EP = item["EP"].ToString(),
                        GT = (DateTime)item["GT"],
                        GP = item["GP"].ToString(),
                        EklendigiTarih = (DateTime)item["EklendigiTarih"],
                        EkleyenPersonel = item["EkleyenPersonel"].ToString(),
                        GuncellendigiTarih = (DateTime)item["GuncellendigiTarih"],
                        GuncelleyenPersonel = item["GuncelleyenPersonel"].ToString()


                    });
                }
                return View(list);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public JsonResult KatilimTurSil(int id)
        {
            TurMus b = new NZF_DAL.TurMus(id);
            var deleteState = b.Delete();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsımAra(MusterilerModel m)
        {
            List<MusterilerModel> list = new List<MusterilerModel>();
            string sql;
            DataAccesBase db = new DataAccesBase();
            sql = "Select * from Musteriler as c where c.Ad like  '%" + m.Ad + "%'";
            DataTable model = db.ReturnDataTable(sql);
            foreach (DataRow item in model.Rows)
            {
                list.Add(new MusterilerModel
                {
                    ID = Convert.ToInt32(item["ID"]),
                    Ad = item["Ad"].ToString(),
                    Soyad = item["Soyad"].ToString(),
                    TcNo = item["TcNo"].ToString(),
                    Telefon = item["Telefon"].ToString(),
                    EPosta = item["EPosta"].ToString()

                });
            }
            return PartialView("Index", list);
        }

        [HttpPost]
        public JsonResult Sorgula(string TcNo)
        {
            List<UlkelerModel> ulkeler = new List<Models.UlkelerModel>();
            string sql = "Select * from Ulkeler as u left join Musteriler as m on u.UlkeAdi = m.Ulke";
            DataAccesBase db = new DataAccesBase();
            DataTable model = db.ReturnDataTable(sql);
            foreach (DataRow item in model.Rows)
            {
                ulkeler.Add(new UlkelerModel
                {
                    UlkeId = (int)item["UlkeId"],
                    UlkeAdi = item["UlkeAdi"].ToString()
                });
            }

            ViewBag.Ulke = ulkeler;

            string sqls = "Select * From Musteriler where TcNo='" + TcNo + "'";
            DataTable models = db.ReturnDataTable(sqls);
            if (models.Rows.Count == 0)
            {

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult DogumGunu(MusterilerModel m)
        {
            string bugün = DateTime.Now.ToString("dd-MM");
            List<MusterilerModel> musterilist = new List<MusterilerModel>();
            string sql = "Select * from Musteriler";
            DataAccesBase db = new DataAccesBase();
            DataTable model = db.ReturnDataTable(sql);
            foreach (DataRow item in model.Rows)
            {
                DateTime dogum = (DateTime)item["Dogumtarihi"];
                string AyGun = dogum.ToString("dd-MM");
                if (AyGun == bugün)
                {
                    musterilist.Add(new MusterilerModel
                    {

                        ID = (int)item["ID"],
                        Ad = item["Ad"].ToString(),
                        Soyad = item["Soyad"].ToString(),
                        DogumTarihi = (DateTime)item["Dogumtarihi"],
                        EPosta = item["EPosta"].ToString(),
                        EklendigiTarih = (DateTime)item["EklendigiTarih"]

                    });

                }


            }
            return View(musterilist);
        }

        public string Ad;
        public string SoyAd;
        public string Eposta;
        public DateTime DogumTarihi;
        public int ID;
        public ActionResult DogumGunuGonder()
        {

            string bugün = DateTime.Now.ToString("dd-MM");
            List<MusterilerModel> musterilist = new List<MusterilerModel>();
            string sql = "Select * from Musteriler";
            DataAccesBase db = new DataAccesBase();
            DataTable model = db.ReturnDataTable(sql);

            foreach (DataRow item in model.Rows)
            {
                DateTime dogum = (DateTime)item["Dogumtarihi"];
                string AyGun = dogum.ToString("dd-MM");
                if (AyGun == bugün)
                {

                    musterilist.Add(new MusterilerModel
                    {

                        ID = (int)item["ID"],
                        Ad = item["Ad"].ToString(),
                        Soyad = item["Soyad"].ToString(),
                        DogumTarihi = (DateTime)item["Dogumtarihi"],
                        EPosta = item["EPosta"].ToString(),
                        EklendigiTarih = (DateTime)item["EklendigiTarih"]

                    });
                    Eposta = item["EPosta"].ToString();
                    Ad = item["Ad"].ToString();
                    SoyAd = item["Soyad"].ToString();

                    string Mesaj = "Sayın " + Ad + " " + SoyAd + " nice senelere iyiki doğdunuz, size özel indirimli turları görmek için sitemize bekleriz " +
                  "http://tur.technorob.com.tr"
                 ;
                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                    mail.To.Add(Eposta);//Alıcı
                    mail.IsBodyHtml = true;//Html mi 
                    mail.Subject = "Doğum Gününüz Sevdiklerinizle hep mutlu olsun";//Mail Konusu
                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                    mail.Body = Mesaj;//Mail Mesajı
                    SmtpClient sc = new SmtpClient();
                    sc.Host = "mail.technorob.com";//Smtp Host
                    sc.Port = 587;//Smtp Port
                    sc.EnableSsl = false;//Enable SSL
                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                    sc.Send(mail);//Mail Gönder
                }

            }
            Response.Write("<script>alert('Mail gönderimi başarıyla gerçekleşti')</script>");
            return View("DogumGunu", musterilist);

        }

        public static string karakterCevir(string kelime)
        {
            string mesaj = kelime;
            char[] oldValue = new char[] { 'ö', 'Ö', 'ü', 'Ü', 'ç', 'Ç', 'İ', 'ı', 'Ğ', 'ğ', 'Ş', 'ş' };
            char[] newValue = new char[] { 'o', 'O', 'u', 'U', 'c', 'C', 'I', 'i', 'G', 'g', 'S', 's' };
            for (int sayac = 0; sayac < oldValue.Length; sayac++)
            {
                mesaj = mesaj.Replace(oldValue[sayac], newValue[sayac]);
            }
            return mesaj;
        }

        public ActionResult Filtrele()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Filtrele(MusterilerModel m, string Yas1, string Yas2, string Yas3, string Baslik, string Icerik)
        {
            Musteriler mus = new Musteriler();
            List<MusterilerModel> musteriList = new List<Models.MusterilerModel>();
            DataAccesBase db = new DataAccesBase();
            #region Girilen Yaş
            if (Yas3 != "" && Yas1 == "" && Yas2 == "")
            {
                if (m.Cinsiyet == "Seçiniz")
                {
                    if (Baslik != "" && Icerik != "")
                    {

                        string sql = "Select * from Musteriler where Yas=" + Yas3;
                        DataTable model = db.ReturnDataTable(sql);
                        foreach (DataRow item in model.Rows)
                        {
                            if (model.Rows.Count > 200)
                            {
                                Response.Write("<script>alert('1 saatte en fazla 200 mail gönderebilirsiniz lütfen daha sonra tekrar deneyin')</script>");
                                break;
                            }
                            else if (model.Rows.Count >= 30 && model.Rows.Count <= 200)
                            {

                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {

                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                    System.Threading.Thread.Sleep(2020);
                                }
                                else
                                {
                                    continue;
                                }



                            }
                            else
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                }
                                else
                                {
                                    continue;
                                }
                            }


                        }
                        Response.Write("<script>alert('Mail Gönderimi başarıyla gerçekleşti')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lütfen Başlık ve İçerik Alanlarını doldurunuz')</script>");
                        return View();
                    }

                }
                else
                {
                    if (Baslik != "" && Icerik != "")
                    {
                        string sql = "Select * from Musteriler where Yas=" + Yas3 + " AND Cinsiyet='" + m.Cinsiyet + "'";
                        DataTable model = db.ReturnDataTable(sql);
                        foreach (DataRow item in model.Rows)
                        {
                            if (model.Rows.Count > 200)
                            {
                                Response.Write("<script>alert('1 saatte en fazla 200 mail gönderebilirsiniz lütfen daha sonra tekrar deneyin')</script>");
                                break;
                            }
                            else if (model.Rows.Count >= 30 && model.Rows.Count <= 200)
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                    System.Threading.Thread.Sleep(2020);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                }
                                else
                                {
                                    continue;
                                }
                            }


                        }
                        Response.Write("<script>alert('Mail Gönderimi başarıyla gerçekleşti')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lütfen Başlık ve İçerik Alanlarını doldurunuz')</script>");
                        return View();
                    }
                }

            }
            #endregion
            #region TümMüşteriler
            else if ((Yas1 == "" && Yas2 == "" && Yas3 == "") || (Yas1 != "" && Yas2 != "" && Yas3 != ""))
            {
                if (m.Cinsiyet == "Seçiniz")
                {
                    if (Baslik != "" && Icerik != "")
                    {
                        string sql = "Select * from Musteriler";
                        DataTable model = db.ReturnDataTable(sql);
                        foreach (DataRow item in model.Rows)
                        {
                            if (model.Rows.Count > 200)
                            {
                                Response.Write("<script>alert('1 saatte en fazla 200 mail gönderebilirsiniz lütfen daha sonra tekrar deneyin')</script>");
                                break;
                            }
                            else if (model.Rows.Count >= 30 && model.Rows.Count <= 199)
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                    System.Threading.Thread.Sleep(2020);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                }
                                else
                                {
                                    continue;
                                }
                            }


                        }
                        Response.Write("<script>alert('Mail Gönderimi başarıyla gerçekleşti')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lütfen Başlık ve İçerik Alanlarını doldurunuz')</script>");
                        return View();
                    }
                }
                else
                {
                    if (Baslik != "" && Icerik != "")
                    {
                        string sql = "Select * from Musteriler where Cinsiyet='" + m.Cinsiyet + "'";
                        DataTable model = db.ReturnDataTable(sql);
                        foreach (DataRow item in model.Rows)
                        {
                            if (model.Rows.Count>200)
                            {
                                Response.Write("<script>alert('1 saatten en fazla 200 mail gönderebilirsiniz. Lütfen daha sonra tekrar deneyin')</script>");
                                break;
                            }
                            else if (model.Rows.Count >= 30 && model.Rows.Count <= 200)
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                    System.Threading.Thread.Sleep(2020);
                                }
                                else
                                {
                                    continue;
                                }
                            }


                        }
                        Response.Write("<script>alert('Mail Gönderimi başarıyla gerçekleşti')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lütfen Başlık ve İçerik Alanlarını doldurunuz')</script>");
                        return View();
                    }
                }

            }
            #endregion
            #region Yaşı Küçük olan
            else if (Yas1 == "" && Yas2 != "" && Yas3 == "")
            {
                if (m.Cinsiyet == "Seçiniz")
                {

                    if (Baslik != "" && Icerik != "")
                    {
                        string sql = "Select * from Musteriler where Yas<=" + Yas2;
                        DataTable model = db.ReturnDataTable(sql);
                        foreach (DataRow item in model.Rows)
                        {
                            if (model.Rows.Count > 200)
                            {
                                Response.Write("<script>alert('1 saatte en fazla 200 mail gönderebilirsiniz lütfen daha sonra tekrar deneyin')</script>");
                                break;
                            }
                            else if (model.Rows.Count >= 30 && model.Rows.Count<=200)
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                    System.Threading.Thread.Sleep(2020);
                                }
                                else
                                {
                                    continue;
                                }

                            }
                            else
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                }
                                else
                                {
                                    continue;
                                }
                            }


                        }
                        Response.Write("<script>alert('Mail Gönderimi başarıyla gerçekleşti')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lütfen Başlık ve İçerik Alanlarını doldurunuz')</script>");
                        return View();
                    }
                }
                else
                {
                    if (Baslik != "" && Icerik != "")
                    {
                        string sql = "Select * from Musteriler where Yas<=" + Yas2 + " AND Cinsiyet ='" + m.Cinsiyet + "'";
                        DataTable model = db.ReturnDataTable(sql);
                        foreach (DataRow item in model.Rows)
                        {
                            if (model.Rows.Count > 200)
                            {
                                Response.Write("<script>alert('1 saatte en fazla 200 mail gönderebilirsiniz lütfen daha sonra tekrar deneyin')</script>");
                                break;
                            }
                            else if (model.Rows.Count >= 30 && model.Rows.Count<=200)
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                    System.Threading.Thread.Sleep(2020);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                }
                                else
                                {
                                    continue;
                                }
                            }


                        }
                        Response.Write("<script>alert('Mail Gönderimi başarıyla gerçekleşti')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lütfen Başlık ve İçerik Alanlarını doldurunuz')</script>");
                        return View();
                    }
                }

            }
            #endregion
            #region Yaşı Büyük Olan
            else if (Yas2 == "" && Yas1 != "" && Yas3 == "")
            {
                if (m.Cinsiyet == "Seçiniz")
                {
                    if (Baslik != "" && Icerik != "")
                    {
                        string sql = "Select * from Musteriler where Yas>=" + Yas1;
                        DataTable model = db.ReturnDataTable(sql);
                        foreach (DataRow item in model.Rows)
                        {
                            if (model.Rows.Count > 200)
                            {
                                Response.Write("<script>alert('1 saatte en fazla 200 mail gönderebilirsiniz lütfen daha sonra tekrar deneyin')</script>");
                                break;
                            }
                           else if (model.Rows.Count >= 30 && model.Rows.Count<=200)
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                    System.Threading.Thread.Sleep(2020);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                }
                                else
                                {
                                    continue;
                                }
                            }


                        }
                        Response.Write("<script>alert('Mail Gönderimi başarıyla gerçekleşti')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lütfen Başlık ve İçerik Alanlarını doldurunuz')</script>");
                        return View();
                    }
                }
                else
                {
                    if (Baslik != "" && Icerik != "")
                    {
                        string sql = "Select * from Musteriler where Yas>=" + Yas1 + " AND Cinsiyet='" + m.Cinsiyet + "'";
                        DataTable model = db.ReturnDataTable(sql);
                        foreach (DataRow item in model.Rows)
                        {
                            if (model.Rows.Count>200)
                            {
                                Response.Write("<script>alert('1 saatte en fazla 200 mail gönderebilirsiniz Lütfen daha sonra tekrar deneyin')</script>");
                                break;
                            }
                            else if (model.Rows.Count >= 30 && model.Rows.Count<=200)
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                    System.Threading.Thread.Sleep(2020);
                                }
                                else
                                {
                                    continue;
                                }
                                    
                            }
                            else
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                }
                                else
                                {
                                    continue;
                                }
                            }

                        }
                        Response.Write("<script>alert('Mail Gönderimi başarıyla gerçekleşti')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lütfen Başlık ve İçerik Alanlarını doldurunuz')</script>");
                        return View();
                    }
                }
            }
            #endregion
            #region Yaş arasında
            else if (Yas1 != "" && Yas2 != "" && Yas3 == "")
            {
                if (m.Cinsiyet == "Seçiniz")
                {
                    if (Baslik != "" && Icerik != "")
                    {
                        string sql = "Select * from Musteriler where Yas>=" + Yas1 + " AND Yas<=" + Yas2;
                        DataTable model = db.ReturnDataTable(sql);
                        foreach (DataRow item in model.Rows)
                        {
                            if (model.Rows.Count>200)
                            {
                                Response.Write("<script>alert('1 saatten en fazla 200 mail gönderebilirsiniz. Lütfen daha sonra tekrar deneyin')</script>");
                                break;
                            }
                            else if (model.Rows.Count >= 30 && model.Rows.Count<=200)
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                    System.Threading.Thread.Sleep(2020);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                }
                                else
                                {
                                    continue;
                                }
                            }


                        }
                        Response.Write("<script>alert('Mail Gönderimi başarıyla gerçekleşti')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lütfen Başlık ve İçerik Alanlarını doldurunuz')</script>");
                        return View();
                    }
                }
                else
                {
                    if (Baslik != "" && Icerik != "")
                    {
                        string sql = "Select * from Musteriler where Yas>=" + Yas1 + " AND Yas<=" + Yas2 + " Cinsiyet ='" + m.Cinsiyet + "'";
                        DataTable model = db.ReturnDataTable(sql);
                        foreach (DataRow item in model.Rows)
                        {
                            if (model.Rows.Count>200)
                            {
                                Response.Write("<script>alert('1 saatte en fazla 200 mail gönderebilirsiniz. Lütfen daha sonra tekrar deneyiniz')</script>");
                                break;
                            }
                           else if (model.Rows.Count >= 30 && model.Rows.Count<=200)
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                    System.Threading.Thread.Sleep(2020);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if ((DateTime)item["MailGöndermeTarihi"] != DateTime.Now || (DateTime)item["MailGöndermeTarihi"] == null)
                                {
                                    mus.MailGöndermeTarihi = (DateTime)item["MailGöndermeTarihi"];
                                    mus.MailGöndermeTarihi = DateTime.Now;
                                    mus.Kaydet();

                                    string Posta = item["EPosta"].ToString();
                                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                                    mail.To.Add(Posta);//Alıcı
                                    mail.IsBodyHtml = true;//Html mi 
                                    mail.Subject = Baslik;//Mail Konusu
                                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                                    mail.Body = Icerik;//Mail Mesajı
                                    SmtpClient sc = new SmtpClient();
                                    sc.Host = "mail.technorob.com";//Smtp Host
                                    sc.Port = 587;//Smtp Port
                                    sc.EnableSsl = false;//Enable SSL
                                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                                    sc.Send(mail);//Mail Gönder
                                }
                                else
                                {
                                    continue;
                                }
                            }


                        }
                        Response.Write("<script>alert('Mail Gönderimi başarıyla gerçekleşti')</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Lütfen Başlık ve İçerik Alanlarını doldurunuz')</script>");
                        return View();
                    }
                }
            }
            #endregion

            return View();

        }


        [HttpPost]
        public JsonResult Goster(MusterilerModel m, string Yas1, string Yas2, string Yas3)
        {
            List<MusterilerModel> musteriList = new List<Models.MusterilerModel>();
            DataAccesBase db = new DataAccesBase();
            #region Girilen Yaş
            if (Yas3 != "" && Yas1 == "" && Yas2 == "")
            {
                if (m.Cinsiyet == "Seçiniz")
                {
                    string sql = "Select * from Musteriler where Yas=" + Yas3;
                    DataTable model = db.ReturnDataTable(sql);
                    foreach (DataRow item in model.Rows)
                    {
                        musteriList.Add(new MusterilerModel
                        {
                            ID = (int)item["ID"],
                            Ad = item["Ad"].ToString(),
                            Soyad = item["Soyad"].ToString(),
                            EPosta = item["EPosta"].ToString(),

                            Yas = (int)item["Yas"],
                            Cinsiyet = item["Cinsiyet"].ToString(),
                            Telefon = item["Telefon"].ToString(),
                            TcNo = item["TcNo"].ToString()
                        });
                    }
                }
                else
                {
                    string sql = "Select * from Musteriler where Yas=" + Yas3 + " AND Cinsiyet='" + m.Cinsiyet + "'";
                    DataTable model = db.ReturnDataTable(sql);
                    foreach (DataRow item in model.Rows)
                    {
                        musteriList.Add(new MusterilerModel
                        {
                            ID = (int)item["ID"],
                            Ad = item["Ad"].ToString(),
                            Soyad = item["Soyad"].ToString(),
                            EPosta = item["EPosta"].ToString(),
                            EklendigiTarih = (DateTime)item["EklendigiTarih"],
                            Yas = (int)item["Yas"],
                            Cinsiyet = item["Cinsiyet"].ToString(),
                            Telefon = item["Telefon"].ToString(),
                            TcNo = item["TcNo"].ToString()
                        });
                    }
                }

            }
            #endregion
            #region TümMüşteriler
            else if ((Yas1 == "" && Yas2 == "" && Yas3 == "") || (Yas1 != "" && Yas2 != "" && Yas3 != ""))
            {
                if (m.Cinsiyet == "Seçiniz")
                {
                    string sql = "Select * from Musteriler";
                    DataTable model = db.ReturnDataTable(sql);
                    foreach (DataRow item in model.Rows)
                    {
                        musteriList.Add(new MusterilerModel
                        {
                            ID = (int)item["ID"],
                            Ad = item["Ad"].ToString(),
                            Soyad = item["Soyad"].ToString(),
                            EPosta = item["EPosta"].ToString(),

                            Yas = (int)item["Yas"],
                            Cinsiyet = item["Cinsiyet"].ToString(),
                            Telefon = item["Telefon"].ToString(),
                            TcNo = item["TcNo"].ToString()
                        });
                    }
                }
                else
                {
                    string sql = "Select * from Musteriler where Cinsiyet='" + m.Cinsiyet + "'";
                    DataTable model = db.ReturnDataTable(sql);
                    foreach (DataRow item in model.Rows)
                    {
                        musteriList.Add(new MusterilerModel
                        {
                            ID = (int)item["ID"],
                            Ad = item["Ad"].ToString(),
                            Soyad = item["Soyad"].ToString(),
                            EPosta = item["EPosta"].ToString(),

                            Yas = (int)item["Yas"],
                            Cinsiyet = item["Cinsiyet"].ToString(),
                            Telefon = item["Telefon"].ToString(),
                            TcNo = item["TcNo"].ToString()
                        });
                    }
                }

            }
            #endregion
            #region Yaşı Küçük olan
            else if (Yas1 == "" && Yas2 != "" && Yas3 == "")
            {
                if (m.Cinsiyet == "Seçiniz")
                {

                    string sql = "Select * from Musteriler where Yas <=" + Yas2;
                    DataTable model = db.ReturnDataTable(sql);
                    foreach (DataRow item in model.Rows)
                    {
                        musteriList.Add(new MusterilerModel
                        {
                            ID = (int)item["ID"],
                            Ad = item["Ad"].ToString(),
                            Soyad = item["Soyad"].ToString(),
                            EPosta = item["EPosta"].ToString(),

                            Yas = (int)item["Yas"],
                            Cinsiyet = item["Cinsiyet"].ToString(),
                            Telefon = item["Telefon"].ToString(),
                            TcNo = item["TcNo"].ToString()
                        });
                    }
                }
                else
                {
                    string sql = "Select * from Musteriler where Yas<=" + Yas2 + " AND Cinsiyet='" + m.Cinsiyet + "'";
                    DataTable model = db.ReturnDataTable(sql);
                    foreach (DataRow item in model.Rows)
                    {
                        musteriList.Add(new MusterilerModel
                        {
                            ID = (int)item["ID"],
                            Ad = item["Ad"].ToString(),
                            Soyad = item["Soyad"].ToString(),
                            EPosta = item["EPosta"].ToString(),

                            Yas = (int)item["Yas"],
                            Cinsiyet = item["Cinsiyet"].ToString(),
                            Telefon = item["Telefon"].ToString(),
                            TcNo = item["TcNo"].ToString()
                        });
                    }
                }

            }
            #endregion
            #region Yaşı Büyük Olan
            else if (Yas2 == "" && Yas1 != "" && Yas3 == "")
            {
                if (m.Cinsiyet == "Seçiniz")
                {
                    string sql = "Select * from Musteriler where Yas>=" + Yas1;
                    DataTable model = db.ReturnDataTable(sql);
                    foreach (DataRow item in model.Rows)
                    {
                        musteriList.Add(new MusterilerModel
                        {
                            ID = (int)item["ID"],
                            Ad = item["Ad"].ToString(),
                            Soyad = item["Soyad"].ToString(),
                            EPosta = item["EPosta"].ToString(),

                            Yas = (int)item["Yas"],
                            Cinsiyet = item["Cinsiyet"].ToString(),
                            Telefon = item["Telefon"].ToString(),
                            TcNo = item["TcNo"].ToString()
                        });
                    }
                }
                else
                {
                    string sql = "Select * from Musteriler where Yas>=" + Yas1 + " AND Cinsiyet='" + m.Cinsiyet + "'";
                    DataTable model = db.ReturnDataTable(sql);
                    foreach (DataRow item in model.Rows)
                    {
                        musteriList.Add(new MusterilerModel
                        {
                            ID = (int)item["ID"],
                            Ad = item["Ad"].ToString(),
                            Soyad = item["Soyad"].ToString(),
                            EPosta = item["EPosta"].ToString(),

                            Yas = (int)item["Yas"],
                            Cinsiyet = item["Cinsiyet"].ToString(),
                            Telefon = item["Telefon"].ToString(),
                            TcNo = item["TcNo"].ToString()
                        });
                    }
                }
            }
            #endregion
            #region Yaş arasında
            else if (Yas1 != "" && Yas2 != "" && Yas3 == "")
            {
                if (m.Cinsiyet == "Seçiniz")
                {
                    string sql = "Select * from Musteriler where Yas >=" + Yas1 + " AND Yas <=" + Yas2;
                    DataTable model = db.ReturnDataTable(sql);
                    foreach (DataRow item in model.Rows)
                    {
                        musteriList.Add(new MusterilerModel
                        {
                            ID = (int)item["ID"],
                            Ad = item["Ad"].ToString(),
                            Soyad = item["Soyad"].ToString(),
                            EPosta = item["EPosta"].ToString(),

                            Yas = (int)item["Yas"],
                            Cinsiyet = item["Cinsiyet"].ToString(),
                            Telefon = item["Telefon"].ToString(),
                            TcNo = item["TcNo"].ToString()
                        });
                    }
                }
                else
                {
                    string sql = "Select * from Musteriler where Yas >=" + Yas1 + " AND Yas <=" + Yas2 + " AND Cinsiyet='" + m.Cinsiyet + "'";
                    DataTable model = db.ReturnDataTable(sql);
                    foreach (DataRow item in model.Rows)
                    {
                        musteriList.Add(new MusterilerModel
                        {
                            ID = (int)item["ID"],
                            Ad = item["Ad"].ToString(),
                            Soyad = item["Soyad"].ToString(),
                            EPosta = item["EPosta"].ToString(),

                            Yas = (int)item["Yas"],
                            Cinsiyet = item["Cinsiyet"].ToString(),
                            Telefon = item["Telefon"].ToString(),
                            TcNo = item["TcNo"].ToString()
                        });
                    }
                }
            }
            #endregion

            return Json(musteriList, JsonRequestBehavior.AllowGet);
        }


        //Musteriler Excel
        public void Excel()
        {
            List<MusterilerModel> MusteriList = new List<MusterilerModel>();
            string sql;
            DataAccesBase db = new DataAccesBase();
            sql = "Select u.UlkeAdi as[UlkeAdi],* from Musteriler as m join Ulkeler as u on m.Ulke = u.UlkeId";
            DataTable model = db.ReturnDataTable(sql);
            foreach (DataRow item in model.Rows)
            {
                MusteriList.Add(new MusterilerModel
                {
                    ID = Convert.ToInt32(item["ID"]),
                    Ad = item["Ad"].ToString(),
                    Soyad = item["Soyad"].ToString(),
                    TcNo = item["TcNo"].ToString(),
                    Telefon = item["Telefon"].ToString(),
                    EPosta = item["EPosta"].ToString(),
                    GSM = item["GSM"].ToString(),
                    PasapartNo = item["PasapartNo"].ToString(),
                    Adres = item["Adres"].ToString(),
                    Fax = item["Fax"].ToString(),
                    RezNo = item["RezNo"].ToString(),
                    Cinsiyet = item["Cinsiyet"].ToString(),

                    PasaportBitisTarihi = item["PasaportBitisTarihi"].ToString(),
                    DogumTarihi = (DateTime)item["DogumTarihi"],
                    EklendigiTarih = (DateTime)item["EklendigiTarih"],
                    EkleyenPersonel = item["EkleyenPersonel"].ToString(),
                    GuncellendigiTarih = (DateTime)item["GuncellendigiTarih"],
                    GuncelleyenPersonel = item["GuncelleyenPersonel"].ToString(),
                    Yas = (int)item["Yas"],
                    VergiDairesi = item["VergiDairesi"].ToString(),
                    VergiNo = item["VergiNo"].ToString(),
                    Unvan = item["Unvan"].ToString(),
                    Ulke = item["UlkeAdi"].ToString(),



                });
            }

            Export export = new Export();
            export.ToExcel(Response, MusteriList);
        }
        public class Export
        {
            public void ToExcel(HttpResponseBase Response, object clientsList)
            {
                var grid = new System.Web.UI.WebControls.GridView();
                grid.DataSource = clientsList;
                grid.DataBind();
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=Musteriler.xls");
                Response.ContentType = "application/excel";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);
                Response.Write(sw.ToString());
                Response.End();
            }
        }

        //Müşteri Katıldığı Turlar Excel
        public void Excel2(int ID)
        {
            List<TurMusModel> MList = new List<TurMusModel>();
            string sql;
            DataAccesBase db = new DataAccesBase();
            sql = "Select tm.ID,tm.EkTarih as[Yolcunun Eklendigi Tarih],tm.EkPersonel as[Yolcuyu Ekleyen Personel],tm.GuTarih as [Yolcunun Guncellendigi Tarih],tm.GuPersonel as[Yolcuyu Guncelleyen Personel],tm.TurID as[TurID],t.TurAdi as[TurAdi],t.BaslangicTarihi as[BaslangicTarihi],tm.Tutar as[Tutar],t.BitisTarihi as[BitisTarihi],t.ET as[Tur Kayıt Tarihi],t.EP as[Turu Ekleyen Personel],t.GT as[Turun Guncellenme Tarihi],t.GP as[Turu Güncelleyen Personel],tm.MusteriID as[MusteriID],m.Ad as[MusteriAd],m.Soyad as[MusteriSoyad],m.Telefon as[Telefon],m.EPosta as[MusteriEposta],m.TcNo as[MusteriTC],m.Adres as[Adres],m.DogumTarihi as[DogumTarihi],m.GSM as[GSM],m.Fax as[Fax],m.PasapartNo as[PasapartNo],m.Cinsiyet as[Cinsiyet],m.RezNo as[RezNo],m.EklendigiTarih as[Musterinin Eklendigi Tarih],m.EkleyenPersonel as[Musteriyi Ekleyen Personel],m.GuncellendigiTarih as[Musterinin Guncellendigi Tarih],m.GuncelleyenPersonel as[Musteriyi Guncelleyen Personel],tm.PersonelID as[PersonelID],p.Adi as[PersonelAd],p.Soyadi as[PersonelSoyAd],u.UlkeAdi as[UlkeAdi],* from TurMus as tm left join Personeller as p on tm.PersonelID=p.ID join Musteriler as m on tm.MusteriID=m.ID join Turlar as t on tm.TurID=t.ID join Ulkeler as u on m.Ulke = u.UlkeId where m.ID=" + ID;
            DataTable model = db.ReturnDataTable(sql);
            foreach (DataRow item in model.Rows)
            {
                MList.Add(new TurMusModel
                {
                    ID = (int)item["ID"],
                    EkTarih = (DateTime)item["Yolcunun Eklendigi Tarih"],
                    EkPersonel = item["Yolcuyu Ekleyen Personel"].ToString(),
                    GuTarih = (DateTime)item["Yolcunun Guncellendigi Tarih"],
                    GuPersonel = item["Yolcuyu Guncelleyen Personel"].ToString(),

                    TurID = (int)item["TurID"],
                    TurAdi = item["TurAdi"].ToString().ToUpper(),
                    BaslangicTarihi = (DateTime)item["BaslangicTarihi"],
                    BitisTarihi = (DateTime)item["BitisTarihi"],
                    ET = (DateTime)item["Tur Kayıt Tarihi"],
                    EP = item["Turu Ekleyen Personel"].ToString(),
                    GT = (DateTime)item["Turun Guncellenme Tarihi"],
                    GP = item["Turu Güncelleyen Personel"].ToString(),
                    Tutar = (int)item["Tutar"],
                    MusteriID = (int)item["MusteriID"],
                    Ad = item["MusteriAd"].ToString(),
                    Soyad = item["MusteriSoyad"].ToString(),
                    Telefon = item["Telefon"].ToString(),
                    EPosta = item["MusteriEposta"].ToString(),
                    TcNo = item["MusteriTC"].ToString(),
                    Adres = item["Adres"].ToString(),
                    DogumTarihi = (DateTime)item["DogumTarihi"],
                    GSM = item["GSM"].ToString(),
                    Fax = item["Fax"].ToString(),
                    PasapartNo = item["PasapartNo"].ToString(),
                    Cinsiyet = item["Cinsiyet"].ToString(),
                    RezNo = item["RezNo"].ToString(),

                    PasaportBitisTarihi = item["PasaportBitisTarihi"].ToString(),
                    Ulke = item["UlkeAdi"].ToString(),
                    Unvan = item["Unvan"].ToString(),
                    VergiNo = item["VergiNo"].ToString(),
                    VergiDairesi = item["VergiDairesi"].ToString(),
                    Yas = (int)item["Yas"],

                    EklendigiTarih = (DateTime)item["Musterinin Eklendigi Tarih"],
                    EkleyenPersonel = item["Musteriyi Ekleyen Personel"].ToString(),
                    GuncellendigiTarih = (DateTime)item["Musterinin Guncellendigi Tarih"],
                    GuncelleyenPersonel = item["Musteriyi Guncelleyen Personel"].ToString(),


                    PersonelID = (int)item["PersonelID"],
                    Adi = item["PersonelAd"].ToString(),
                    Soyadi = item["PersonelSoyAd"].ToString()

                });
            }

            Export2 export = new Export2();
            export.ToExcel2(Response, model);
        }
        public class Export2
        {
            public void ToExcel2(HttpResponseBase Response, object clientsList)
            {
                var grid = new System.Web.UI.WebControls.GridView();
                grid.DataSource = clientsList;
                grid.DataBind();
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=MusteriTurlari.xls");
                Response.ContentType = "application/excel";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);
                Response.Write(sw.ToString());
                Response.End();
            }
        }


    }

}

