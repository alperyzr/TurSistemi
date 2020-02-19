using HakanERP.Models;
using NZF_DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace HakanERP.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            

                Session["KullaniciAdi"] = null;
                Session["PersonelID"] = null;
                Session["YetkiID"] = null;
                Session["PersonelAd"] = null;
                Session["PersonelSoyAd"] = null;
                
                return View();
           

        }
        [HttpPost]
        public ActionResult Index(PersonellerModel models, string responsables, bool checkResp = false)
        {
            
                DataAccesBase db = new DataAccesBase();
                string sql = "Select * from Personeller";
                DataTable dt = db.ReturnDataTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["KullaniciAdi"].ToString() == models.KullaniciAdi && dr["Sifre"].ToString() == models.Sifre)
                {
                    Session["KullaniciAdi"] = dr["KullaniciAdi"].ToString();

                    Session["PersonelID"] = dr["ID"].ToString();
                    Session["PersonelAd"] = dr["Adi"].ToString();
                    Session["PersonelSoyAd"] = dr["Soyadi"].ToString();
                    if (checkResp == true)
                    {
                        HttpCookie cerez = new HttpCookie("cerezim");
                        cerez.Values.Add("Sifre", models.Sifre);
                        cerez.Values.Add("Adi", models.KullaniciAdi);
                        cerez.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(cerez);
                    }

                    string isimbasHarf = Session["PersonelAd"].ToString().Substring(0, 1);
                    string soyisimbasHarf = Session["PersonelSoyAd"].ToString().Substring(0, 1);
                    Session["BasHarfler"] = isimbasHarf.ToUpper() + " " + soyisimbasHarf.ToUpper();


                    return RedirectToAction("Index", "Tutar");

                }
            }
                Response.Write("<script>alert('Kullanıcı Adı veya Şifre Yanlış')</script>");
                return View();
          

        }

        public string mails;
        public string sifre;
        public string Ad;
        public string Soyad;
        public string Kullaniciad;
        public ActionResult SifreUnut()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifreUnut(PersonellerModel p)
        {
            
                string sql;
                sql = "Select * from Personeller as p where p.EMail=" + "'" + p.EMail + "'";
                DataAccesBase db = new DataAccesBase();
                DataTable model = db.ReturnDataTable(sql);
                foreach (DataRow item in model.Rows)
                {
                    Kullaniciad = item["KullaniciAdi"].ToString();
                    Soyad = item["Soyadi"].ToString();
                    Ad = item["Adi"].ToString();
                    sifre = item["Sifre"].ToString();
                    mails = item["EMail"].ToString();

                }
                if (mails == null)
                {

                    Response.Write("<script>alert('Böyle bir mail yoktur')</script>");
                }
                else
                {
                    string Mesaj = "Sayın " + Ad + " " + Soyad + " Kullanıcı Adını: " + Kullaniciad + " Şifreniz: " + sifre;
                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                    mail.From = new MailAddress("bilgi@technorob.com.tr");//Verici
                    mail.To.Add(mails);//Alıcı
                    mail.IsBodyHtml = true;//Html mi 
                    mail.Subject = "Rapor Kullanıcı Bilgileri";//Mail Konusu
                    mail.BodyEncoding = System.Text.Encoding.UTF8;//UTF-8 Encoding
                    mail.Body = Mesaj;//Mail Mesajı
                    SmtpClient sc = new SmtpClient();
                    sc.Host = "mail.technorob.com";//Smtp Host
                    sc.Port = 587;//Smtp Port
                    sc.EnableSsl = false;//Enable SSL
                    sc.Credentials = new NetworkCredential("bilgi@technorob.com.tr", "TEchnorob18");//Gmail Kulanıcı - Şifre
                    sc.Send(mail);//Mail Gönder
                    Response.Write("<script>alert('Şifreniz Mailinize Gönderilmiştir')</script>");
                }
                return View();
          

        }
    }
}