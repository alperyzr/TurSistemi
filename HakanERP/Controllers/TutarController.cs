using HakanERP.Models;
using NZF_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HakanERP.Controllers
{
    public class TutarController : Controller
    {

       
        public ActionResult Index()
        {
            if (Session["KullaniciAdi"] != null)
            {

            
                List<TurMusModel> TurList = new List<TurMusModel>();
                string sql;
                DataAccesBase db = new DataAccesBase();
                sql = "select * from Turlar order by ID desc";
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    TurList.Add(new TurMusModel
                    {

                      BaslangicTarihi = (DateTime)item["BaslangicTarihi"],
                        BitisTarihi = (DateTime)item["BitisTarihi"],
                        TurAdi = item["TurAdi"].ToString(),
                        TurID = Convert.ToInt32(item["ID"]),
                        ET = (DateTime)item["ET"]

                    });
                }

              
                return View(TurList);

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
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Ekle(TutarModel t, int ID = 0)
        {
            if (ID == 0)
            {
                Turlar tur = new Turlar(t.ID);
                tur.BaslangicTarihi = t.BaslangicTarihi;
                tur.BitisTarihi = t.BitisTarihi;
                tur.TurAdi = t.TurAdi;
                string TurAdi = tur.TurAdi.ToUpper();
                tur.TurAdi = karakterCevir(TurAdi);

                tur.ET = t.ET;
                tur.ET = DateTime.Now;
                tur.EP = t.EP;
                tur.EP = Session["PersonelAd"].ToString() + " " + Session["PersonelSoyAd"].ToString();
                string Ep = tur.EP.ToUpper();
                tur.EP = Ep;
                tur.Kaydet();
                Response.Write("<script language='javascript'>alert('Kayıt Başarıyla Eklendi.');</script>");
                return View();
            }
            else
            {
                Turlar tur = new Turlar(t.ID);
                tur.TurAdi = t.TurAdi;
                string Turadi = tur.TurAdi.ToUpper();
                tur.TurAdi = karakterCevir(Turadi);
                tur.BaslangicTarihi = t.BaslangicTarihi;
                tur.BitisTarihi = t.BitisTarihi;

                tur.GT = t.GT;
                tur.GT = DateTime.Now;
                tur.GP = t.GP;
                tur.GP = Session["PersonelAd"].ToString() + " " + Session["PersonelSoyAd"].ToString();
                string gp = tur.GP.ToUpper();
                tur.GP = gp;
                tur.Kaydet();

                Response.Write("<script>alert('Kayıt başarıyla güncellendi')</script>");
                return View();
            }
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
        public ActionResult Guncelle(int ID)
        {
            if (Session["KullaniciAdi"] != null)
            {
                TutarModel m = new Models.TutarModel();
                Turlar mus = new Turlar(ID);

                m.TurAdi = mus.TurAdi;
                m.BaslangicTarihi = mus.BaslangicTarihi;
                m.BitisTarihi = mus.BitisTarihi;


                return View("Ekle", m);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult Detay(int ID)
        {
            if (Session["KullaniciAdi"] != null)
            {


                List<TutarModel> TurList = new List<TutarModel>();
                string sql;
                DataAccesBase db = new DataAccesBase();
                sql = "select * from Turlar where ID="+ID+" order by ID desc";
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    TurList.Add(new TurMusModel
                    {

                        BaslangicTarihi = (DateTime)item["BaslangicTarihi"],
                        BitisTarihi = (DateTime)item["BitisTarihi"],
                        TurAdi = item["TurAdi"].ToString(),
                        TurID = Convert.ToInt32(item["ID"]),
                        ET = (DateTime)item["ET"],
                        EP = item["EP"].ToString(),
                        GT = (DateTime)item["GT"],
                        GP = item["GP"].ToString()

                    });
                }


                return View(TurList);

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public JsonResult Sil(int id)
        {
            Turlar b = new NZF_DAL.Turlar(id);
            var deleteState = b.Delete();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Katilanlar(int ID)
        {
            if (Session["KullaniciAdi"] != null)
            {
                List<TurMusModel> MList = new List<TurMusModel>();
                string sql;
                DataAccesBase db = new DataAccesBase();
                sql = "Select tm.ID,tm.EkTarih as[Eklendigi Tarih],tm.TurID as[TurID], m.Ad as[MusteriAd],m.Soyad as[MusteriSoyad],m.GSM as[MusteriTel],m.EPosta as[MusteriEposta],m.TcNo as[MusteriTC],t.TurAdi as[TurAdi],t.BaslangicTarihi as[BaslangicTarihi],t.BitisTarihi as[BitisTarihi],tm.Tutar as[Tutar],p.Adi as[PersonelAd],p.Soyadi as[PersonelSoyAd],u.UlkeAdi as[UlkeAdi],*  from TurMus tm   join Personeller p on tm.PersonelID = p.ID join Musteriler m on tm.MusteriID = m.ID join Turlar t on tm.TurID = t.ID join Ulkeler as u on m.Ulke = u.UlkeId where t.ID=" + ID+ " order by tm.ID desc";
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    MList.Add(new TurMusModel
                    {
                        ID = (int)item["ID"],
                        EkTarih = (DateTime)item["Eklendigi Tarih"],
                        TurID = (int)item["TurID"],
                        TurAdi = item["TurAdi"].ToString(),
                        BaslangicTarihi = (DateTime)item["BaslangicTarihi"],
                        BitisTarihi = (DateTime)item["BitisTarihi"],

                        MusteriID = (int)item["MusteriID"],
                        Ad = item["MusteriAd"].ToString(),
                        Soyad = item["MusteriSoyad"].ToString(),
            
                        PersonelID = (int)item["PersonelID"],
                        Adi = item["PersonelAd"].ToString(),
                        Soyadi = item["PersonelSoyAd"].ToString(),
                        Tutar = (int)item["Tutar"]
                        
                    });
                }
                return View(MList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult YolcuEkle(TurMusModel f, string TcNo)
        {
            if (Session["KullaniciAdi"] != null)
            {


                Musteriler mu = new Musteriler("TcNo", "" + TcNo + "");
                string sql = "Select * from Musteriler where TcNo='" + f.TcNo + "'";
                DataAccesBase db = new DataAccesBase();
                DataTable model = db.ReturnDataTable(sql);
                if (model.Rows.Count == 1)
                {
                    TurMus t = new TurMus(f.ID);
                    t.TurID = f.TurID;

                    t.EkTarih = f.EkTarih;
                    t.EkTarih = DateTime.Now;
                    t.EkPersonel = f.EkPersonel;
                    t.EkPersonel = Session["PersonelAd"].ToString() + " " + Session["PersonelSoyAd"].ToString();

                    t.PersonelID = f.PersonelID;
                    t.PersonelID = Int32.Parse(Session["PersonelID"].ToString());

                    t.MusteriID = mu.ID;
                    t.Tutar = f.Tutar;
                    t.Kaydet();


                    return RedirectToAction("Index", "Tutar");
                }
                else
                {
                    return RedirectToAction("Ekle", "Musteri");
                }
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
        public JsonResult YolcuSil(int id)
        {
            TurMus b = new NZF_DAL.TurMus(id);
            var deleteState = b.Delete();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        
        //Tur Excel
        public void Excel()
        {
            List<TutarModel> TurList = new List<TutarModel>();
            string sql;
            DataAccesBase db = new DataAccesBase();
            sql = "Select t.ET as[Eklendigi Tarih],t.EP as[Ekleyen Personel],t.GT as[Guncellendigi Tarih],t.GP as [Guncelleyen Personel],t.ID,t.TurAdi as[TurAdi],t.BaslangicTarihi as[BaslangicTarihi],t.BitisTarihi as[BitisTarihi] from Turlar as t ORDER BY t.ID DESC";
            DataTable model = db.ReturnDataTable(sql);
            foreach (DataRow item in model.Rows)
            {
                TurList.Add(new TutarModel
                {

                    BaslangicTarihi = (DateTime)item["BaslangicTarihi"],
                    BitisTarihi = (DateTime)item["BitisTarihi"],
                    TurAdi = item["TurAdi"].ToString(),
                    ID = Convert.ToInt32(item["ID"]),
                    ET = (DateTime)item["Eklendigi Tarih"],
                    EP = item["Ekleyen Personel"].ToString(),
                    GT = (DateTime)item["Guncellendigi Tarih"],
                    GP = item["Guncelleyen Personel"].ToString()

                });
            }

            Export export = new Export();
            export.ToExcel(Response, model);
        }
        public class Export
        {
            public void ToExcel(HttpResponseBase Response, object clientsList)
            {
                var grid = new System.Web.UI.WebControls.GridView();
                grid.DataSource = clientsList;
                grid.DataBind();
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=Turlar.xls");
                Response.ContentType = "application/excel";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);
                Response.Write(sw.ToString());
                Response.End();
            }
        }


        //Tura Katılanlar Excel
        public void ExcelKatilan(int ID)
        {
            List<TurMusModel> MList = new List<TurMusModel>();
            string sql;
            DataAccesBase db = new DataAccesBase();
            sql = "Select tm.ID as[ID],tm.EkPersonel as[Yolcuyu Ekleyen Personel],tm.EkTarih as[Yolcunun Eklendigi Tarih],tm.GuPersonel as[Yolcuyu Guncelleyen Personel],tm.GuTarih as[Yolcuyunun Güncellendigi Tarih],tm.MusteriID as[MusteriID],u.UlkeAdi as[UlkeAdi],m.TcNo as[MusteriTC], m.Ad as[MusteriAd],m.Soyad as[MusteriSoyad],m.Cinsiyet as[Cinsiyet],m.Telefon as[Telefon],m.EPosta as[MusteriEposta],m.Adres as[Adres],m.DogumTarihi as[Dogum Tarihi],m.Yas as[Yas],m.GSM as[GSM],m.Fax as[Fax],m.PasapartNo as[Pasapart No],m.PasaportBitisTarihi as[Pasaport Bitis Tarihi],m.VergiDairesi as[Vergidairesi],m.VergiNo as[VergiNo],m.Unvan as[Unvan],m.RezNo as[RezNo],tm.TurID as[TurID], t.TurAdi as[TurAdi],t.BaslangicTarihi as[Baslangic Tarihi],t.BitisTarihi as[Bitis Tarihi],tm.Tutar as[Tutar],tm.PersonelID as[PersonelID],p.Adi as[PersonelAd],p.Soyadi as[PersonelSoyAd] from TurMus tm join Personeller p on tm.PersonelID = p.ID join Musteriler m on tm.MusteriID = m.ID join Turlar t on tm.TurID = t.ID join Ulkeler as u on m.Ulke = u.UlkeId where t.ID=" + ID;
            DataTable model = db.ReturnDataTable(sql);
            foreach (DataRow item in model.Rows)
            {
                MList.Add(new TurMusModel
                {
                    ID = (int)item["ID"],
                    EkPersonel = item["Yolcuyu Ekleyen Personel"].ToString(),
                    EkTarih = (DateTime)item["Yolcunun Eklendigi Tarih"],
                    GuPersonel = item["Yolcuyu Guncelleyen Personel"].ToString(),
                    GuTarih = (DateTime)item["Yolcuyunun Güncellendigi Tarih"],

                    TurID = (int)item["TurID"],
                    TurAdi = item["TurAdi"].ToString(),
                    BaslangicTarihi = (DateTime)item["Baslangic Tarihi"],
                    BitisTarihi = (DateTime)item["Bitis Tarihi"],
                   Tutar = (int)item["Tutar"],

                    MusteriID = (int)item["MusteriID"],
                    Ad = item["MusteriAd"].ToString(),
                    Soyad = item["MusteriSoyad"].ToString(),
                    Telefon = item["Telefon"].ToString(),
                    Adres = item["Adres"].ToString(),
                    GSM = item["GSM"].ToString(),
                    EPosta = item["MusteriEposta"].ToString(),
                    Fax = item["Fax"].ToString(),
                    TcNo = item["MusteriTC"].ToString(),
                    DogumTarihi = (DateTime)item["Dogum Tarihi"],
                    PasapartNo = item["Pasapart No"].ToString(),
                    PasaportBitisTarihi = item["Pasaport Bitis Tarihi"].ToString(),
                    VergiDairesi = item["Vergidairesi"].ToString(),
                    VergiNo = item["VergiNo"].ToString(),
                    Unvan = item["Unvan"].ToString(),
                    UlkeAdi = item["UlkeAdi"].ToString(),
                    Yas = (int)item["Yas"],
                   
                    Cinsiyet = item["Cinsiyet"].ToString(),
                    RezNo = item["RezNo"].ToString(),

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
                Response.AddHeader("content-disposition", "attachment; filename=TuraKatilanlar.xls");
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
