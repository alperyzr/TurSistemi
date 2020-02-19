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
    public class PersonelController : Controller
    {
        // GET: Personel
        public ActionResult Index()
        {
            if (Session["KullaniciAdi"] != null)
            {
                List<PersonellerModel> PersonelList = new List<PersonellerModel>();
                string sql;
                DataAccesBase db = new DataAccesBase();
                sql = "Select * from Personeller order by ID desc";
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    PersonelList.Add(new PersonellerModel
                    {
                        ID = Convert.ToInt32(item["ID"]),
                        Adi = item["Adi"].ToString(),
                        Soyadi = item["Soyadi"].ToString(),
                        Sifre = item["Sifre"].ToString(),
                        KullaniciAdi = item["KullaniciAdi"].ToString(),
                        EMail = item["EMail"].ToString()



                    });
                }
                return View(PersonelList);
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
        public ActionResult Ekle(PersonellerModel m, int ID = 0)
        {
            if (ID == 0)
            {
                Personeller mus = new Personeller(m.ID);
                mus.Adi = m.Adi;
                if (mus.Adi != null)
                {
                    string ad = mus.Adi.ToUpper();
                    mus.Adi = ad;
                }
                mus.Sifre = m.Sifre;
                mus.Soyadi = m.Soyadi;
                if (mus.Soyadi != null)
                {
                    string soyad = mus.Soyadi.ToUpper();
                    mus.Soyadi = soyad;
                }
                mus.KullaniciAdi = m.KullaniciAdi;
                mus.EMail = m.EMail;

                mus.Kaydet();
                Response.Write("<script language='javascript'>alert('Kayıt Başarıyla Eklendi.');</script>");
                return View();
            }
            else
            {
                Personeller mus = new Personeller(m.ID);
                mus.Adi = m.Adi;
                if (mus.Adi != null)
                {
                    string ad = mus.Adi.ToUpper();
                    mus.Adi = ad;
                }
                mus.Sifre = m.Sifre;
                if (mus.Soyadi != null)
                {
                    string soyad = mus.Soyadi.ToUpper();
                    mus.Soyadi = soyad;
                }
                mus.KullaniciAdi = m.KullaniciAdi;
                mus.EMail = m.EMail;

                mus.Kaydet();

                return RedirectToAction("Index", "Personel");
            }
        }

       
        public ActionResult Guncelle(int ID)
        {
            if (Session["KullaniciAdi"] != null)
            {
                PersonellerModel m = new Models.PersonellerModel();
                Personeller mus = new Personeller(ID);

                m.Adi = mus.Adi;
                m.Sifre = mus.Sifre;
                m.Soyadi = mus.Soyadi;
                m.KullaniciAdi = mus.KullaniciAdi;
                m.EMail = mus.EMail;

                return View("Ekle", m);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public JsonResult Sil(int id)
        {
            Personeller m = new NZF_DAL.Personeller(id);
            var silinen = m.Delete();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detay(int ID)
        {
            if (Session["KullaniciAdi"] != null)
            {
                List<PersonellerModel> MList = new List<PersonellerModel>();
                string sql;
                DataAccesBase db = new DataAccesBase();
                sql = "Select * from Personeller as m where m.ID=" + ID;
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    MList.Add(new PersonellerModel
                    {
                        ID = (int)item["ID"],
                        Adi = item["Adi"].ToString(),
                        Sifre = item["Sifre"].ToString(),
                        Soyadi = item["Soyadi"].ToString(),
                        KullaniciAdi = item["KullaniciAdi"].ToString(),
                        EMail = item["EMail"].ToString()

                    });
                }
                return View(MList);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult IsımAra(PersonellerModel m)
        {
            List<PersonellerModel> list = new List<PersonellerModel>();
            string sql;
            DataAccesBase db = new DataAccesBase();
            sql = "Select * from Personeller as c where c.Adi like  '%" + m.Adi + "%'";
            DataTable model = db.ReturnDataTable(sql);
            foreach (DataRow item in model.Rows)
            {
                list.Add(new MusterilerModel
                {
                    ID = Convert.ToInt32(item["ID"]),
                    Adi = item["Adi"].ToString(),
                    Soyadi = item["Soyadi"].ToString(),
                    

                });
            }
            return PartialView("Index", list);
        }

        public void Excel()
        {
            List<PersonellerModel> PersonelList = new List<PersonellerModel>();
            string sql;
            DataAccesBase db = new DataAccesBase();
            sql = "Select * from Personeller";
            DataTable model = db.ReturnDataTable(sql);
            foreach (DataRow item in model.Rows)
            {
                PersonelList.Add(new PersonellerModel
                {
                    ID = Convert.ToInt32(item["ID"]),
                    Adi = item["Adi"].ToString(),
                    Soyadi = item["Soyadi"].ToString(),
                    Sifre = item["Sifre"].ToString(),
                    KullaniciAdi = item["KullaniciAdi"].ToString(),
                    EMail = item["EMail"].ToString()



                });
            }

            Export export = new Export();
            export.ToExcel(Response, PersonelList);
        }
        public class Export
        {
            public void ToExcel(HttpResponseBase Response, object clientsList)
            {
                var grid = new System.Web.UI.WebControls.GridView();
                grid.DataSource = clientsList;
                grid.DataBind();
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=Personeller.xls");
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