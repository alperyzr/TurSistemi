using HakanERP.Models;
using NZF_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace HakanERP.Controllers
{
    public class TurMusController : Controller
    {

        public ActionResult Index()
        {
            if (Session["KullaniciAdi"] != null)
            {
                List<TurMusModel> TurBilgisi = new List<Models.TurMusModel>();
                string sql;
                DataAccesBase db = new DataAccesBase();

                sql = "Select tm.ID, tm.TurID as[TurID], m.Ad as[MusteriAd],m.Soyad as[MusteriSoyad],m.GSM as[MusteriTel],m.EPosta as[MusteriEposta],m.TcNo as[MusteriTC],t.TurAdi as[TurAdi],t.BaslangicTarihi as[BaslangicTarihi],t.BitisTarihi as[BitisTarihi],p.Adi as[PersonelAdi],p.Soyadi as[PersonelSoyAdi],*  from TurMus tm  left join Personeller p on tm.PersonelID = p.ID  left join Musteriler m on tm.MusteriID = m.ID left join Turlar t on tm.TurID = t.ID order by tm.ID desc";
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    TurBilgisi.Add(new TurMusModel
                    {
                        ID = (int)item["ID"],
                        TurID = (int)item["TurID"],
                        TurAdi = item["TurAdi"].ToString(),
                        BaslangicTarihi = (DateTime)item["BaslangicTarihi"],
                        BitisTarihi = (DateTime)item["BitisTarihi"],
                        Ad = item["MusteriAd"].ToString(),
                        Soyad = item["MusteriSoyad"].ToString(),
                        GSM = item["MusteriTel"].ToString(),
                        EPosta = item["MusteriEposta"].ToString(),
                        TcNo = item["MusteriTC"].ToString(),
                        Adi = item["PersonelAdi"].ToString(),
                        Soyadi = item["PersonelSoyAdi"].ToString()


                    });
                }
                ViewBag.TurMus = TurBilgisi;
                return View(TurBilgisi);
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

                List<TutarModel> TutarM = new List<Models.TutarModel>();
                string sql;
                DataAccesBase db = new DataAccesBase();
                sql = "Select * from Turlar";
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    TutarM.Add(new TutarModel
                    {
                        ID = (int)item["ID"],
                        TurAdi = item["TurAdi"].ToString(),
                        BaslangicTarihi = (DateTime)item["BaslangicTarihi"],
                        BitisTarihi = (DateTime)item["BitisTarihi"]

                    });
                }
                ViewBag.Turlar = TutarM;

                List<PersonellerModel> personeller = new List<Models.PersonellerModel>();
                string sql2;
                DataAccesBase db2 = new DataAccesBase();
                sql2 = "Select * from Personeller";
                DataTable model2 = db2.ReturnDataTable(sql2);
                foreach (DataRow item in model2.Rows)
                {
                    personeller.Add(new PersonellerModel
                    {
                        ID = (int)item["ID"],
                        Adi = item["Adi"].ToString() + " " + item["Soyadi"].ToString()


                    });
                }
                ViewBag.personel = personeller;

                List<MusterilerModel> musteriler = new List<Models.MusterilerModel>();
                string sql3;
                DataAccesBase db3 = new DataAccesBase();
                sql3 = "Select * from Musteriler";
                DataTable model3 = db3.ReturnDataTable(sql3);
                foreach (DataRow item in model3.Rows)
                {
                    musteriler.Add(new MusterilerModel
                    {
                        ID = (int)item["ID"],
                        Ad = item["Ad"].ToString() + " " + item["Soyad"].ToString()


                    });
                }
                ViewBag.musteri = musteriler;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
        [HttpPost]
        public ActionResult Ekle(TurMusModel tm, int ID = 0)
        {
            if (ID == 0)
            {
                TurMus t = new TurMus(tm.ID);
                t.MusteriID = tm.MusteriID;
                t.PersonelID = tm.PersonelID;
                t.TurID = tm.TurID;
                t.Kaydet();


                return RedirectToAction("Index", "TurMus");

            }
            else
            {
                TurMus t = new TurMus(tm.ID);
                t.MusteriID = tm.MusteriID;
                t.PersonelID = tm.PersonelID;
                t.TurID = tm.TurID;
                t.Kaydet();


                return RedirectToAction("Index", "TurMus");
            }

        }
        public ActionResult Guncelle(TurMusModel tm)
        {
            if (Session["KullaniciAdi"] != null)
            {
                #region
                List<TutarModel> TutarM = new List<Models.TutarModel>();
                string sql;
                DataAccesBase db = new DataAccesBase();
                sql = "Select * from Turlar";
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    TutarM.Add(new TutarModel
                    {
                        ID = (int)item["ID"],
                        TurAdi = item["TurAdi"].ToString(),


                    });
                }
                ViewBag.Turlar = TutarM;


                List<PersonellerModel> personeller = new List<Models.PersonellerModel>();
                string sql2;
                DataAccesBase db2 = new DataAccesBase();
                sql2 = "Select * from Personeller";
                DataTable model2 = db2.ReturnDataTable(sql2);
                foreach (DataRow item in model2.Rows)
                {
                    personeller.Add(new PersonellerModel
                    {
                        ID = (int)item["ID"],
                        Adi = item["Adi"].ToString() + " " + item["Soyadi"].ToString()


                    });
                }
                ViewBag.personel = personeller;



                List<MusterilerModel> musteriler = new List<Models.MusterilerModel>();
                string sql3;
                DataAccesBase db3 = new DataAccesBase();
                sql3 = "Select * from Musteriler";
                DataTable model3 = db3.ReturnDataTable(sql3);
                foreach (DataRow item in model3.Rows)
                {
                    musteriler.Add(new MusterilerModel
                    {
                        ID = (int)item["ID"],
                        Ad = item["Ad"].ToString() + " " + item["Soyad"].ToString()


                    });
                }
                ViewBag.musteri = musteriler;
                #endregion


                TurMus t = new TurMus(tm.ID);

                tm.TurID = t.TurID;
                tm.PersonelID = t.PersonelID;
                tm.MusteriID = t.MusteriID;

                return View("Ekle", tm);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public JsonResult Sil(int id)
        {
            TurMus b = new NZF_DAL.TurMus(id);
            var deleteState = b.Delete();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detay(TurMusModel m)
        {
            if (Session["KullaniciAdi"] != null)
            {
                string sql;
                List<TurMusModel> list = new List<Models.TurMusModel>();
                sql = "Select * from TurMus as t left join Musteriler as m on t.MusteriID=m.ID left join Personeller as p on t.PersonelID= p.ID left join Turlar as tur on t.TurID=tur.ID where t.ID=" + m.ID;
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
                        Telefon = item["Telefon"].ToString()


                    });
                }
                return View(list);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult YolcuEkle(TurMusModel f, string TcNo)
        {
            Musteriler mu = new Musteriler("TcNo", "" + TcNo + "");
            string sql = "Select * from Musteriler where TcNo='" + f.TcNo + "'";
            DataAccesBase db = new DataAccesBase();
            DataTable model = db.ReturnDataTable(sql);
            if (model.Rows.Count == 1)
            {
                TurMus t = new TurMus(f.ID);
                t.TurID = f.TurID;
                t.PersonelID = f.PersonelID;
                t.MusteriID = mu.ID;
                t.Kaydet();
                return RedirectToAction("Index", "TurMus");
            }
            else
            {
                return RedirectToAction("Ekle", "Musteri");
            }

        }
    }
}