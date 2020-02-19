using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace NZF_DAL
{
    using Microsoft.VisualBasic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;

    public class Musteriler : DataAccesBase
    {

        string m_SQL;
        int m_ConCount;
		int m_ID; 


        public String Ad;
        public String Soyad;
        public String Telefon;
        public String Adres;
        public String GSM;
        public String EPosta;
        public String Fax;
        public String TcNo;
        public System.DateTime DogumTarihi = Convert.ToDateTime("1900-01-01");
        public String PasapartNo;
        public String Ulke;
        public String Unvan;
        public String VergiDairesi;
        public String VergiNo;
        public Int32 Yas;
        public String PasaportBitisTarihi;
        public String RezNo;
        public String Cinsiyet;
       
        public System.DateTime EklendigiTarih = Convert.ToDateTime("1900-01-01");
        public String EkleyenPersonel;
        public System.DateTime GuncellendigiTarih = Convert.ToDateTime("1900-01-01");
        public String GuncelleyenPersonel;
        public System.DateTime MailGöndermeTarihi = Convert.ToDateTime("1900-01-01");



        public int ID
        {
            get { return m_ID; }
        }

        public Musteriler ()
        {
        }
        public Musteriler (int pID)
        {
            m_SQL = "Select * from Musteriler where ID=" + pID;
            initialize();
        }

        public Musteriler(string pFIELD_NAME, string pVALUE)
        {
            m_SQL = "Select * from Musteriler where " + pFIELD_NAME + "='" + pVALUE + "'";
            initialize();
        }
 

        public bool initialize()
        {
            DataTable DT = ReturnDataTable(m_SQL);
            try
            {
                m_ConCount = DT.Rows.Count;
                if (DT.Rows.Count == 0)
                {
                    m_ID = 0;
                    return true;
                }
                m_ID = Convert.ToInt32( DT.Rows[0]["ID"]);
                Ad = Convert.ToString(DT.Rows[0]["Ad"]);
                Soyad = Convert.ToString(DT.Rows[0]["Soyad"]);
                Telefon = Convert.ToString(DT.Rows[0]["Telefon"]);
                Adres = Convert.ToString(DT.Rows[0]["Adres"]);
                GSM = Convert.ToString(DT.Rows[0]["GSM"]);
                EPosta = Convert.ToString(DT.Rows[0]["EPosta"]);
                Fax = Convert.ToString(DT.Rows[0]["Fax"]);
                TcNo = Convert.ToString(DT.Rows[0]["TcNo"]);
                DogumTarihi = Convert.ToDateTime(DT.Rows[0]["DogumTarihi"]);
                PasapartNo = Convert.ToString(DT.Rows[0]["PasapartNo"]);
                Ulke = Convert.ToString(DT.Rows[0]["Ulke"]);
                Unvan = Convert.ToString(DT.Rows[0]["Unvan"]);
                VergiDairesi = Convert.ToString(DT.Rows[0]["VergiDairesi"]);
                VergiNo = Convert.ToString(DT.Rows[0]["VergiNo"]);
                Yas = Convert.ToInt32(DT.Rows[0]["Yas"]);
                PasaportBitisTarihi = Convert.ToString(DT.Rows[0]["PasaportBitisTarihi"]);
                RezNo = Convert.ToString(DT.Rows[0]["RezNo"]);
                Cinsiyet = Convert.ToString(DT.Rows[0]["Cinsiyet"]);
               
                EklendigiTarih = Convert.ToDateTime(DT.Rows[0]["EklendigiTarih"]);
                EkleyenPersonel = Convert.ToString(DT.Rows[0]["EkleyenPersonel"]);
                GuncellendigiTarih = Convert.ToDateTime(DT.Rows[0]["GuncellendigiTarih"]);
                GuncelleyenPersonel = Convert.ToString(DT.Rows[0]["GuncelleyenPersonel"]);
                MailGöndermeTarihi = Convert.ToDateTime(DT.Rows[0]["MailGöndermeTarihi"]);
                DT.Dispose();
            }
            catch (Exception ex)
            {
            }
            return true;
        }

       public void Kaydet()
        {
            if (Kontrol())
            {
                switch (m_ID)
                {
                    case 0:
                        KaydetInsert();
                        break;
                    default:
                        KaydetUpdate();
                        break;
                }
            }
        }


        public bool Kontrol()
        {
            return true;
        }

		
		
		
		private int KaydetInsert()
        {
            string SQL = null;

			SQL="Insert Into Musteriler (Ad, Soyad, Telefon, Adres, ";
            SQL += "GSM, EPosta, Fax, TcNo, ";
            SQL += "DogumTarihi, PasapartNo, Ulke, Unvan, ";
            SQL += "VergiDairesi, VergiNo, Yas, PasaportBitisTarihi, ";
            SQL += "RezNo, Cinsiyet, EklendigiTarih, ";
            SQL += "EkleyenPersonel, GuncellendigiTarih, GuncelleyenPersonel, MailGöndermeTarihi) values (";
            SQL += "'" + Ad + "',";
            SQL += "'" + Soyad + "',";
            SQL += "'" + Telefon + "',";
            SQL += "'" + Adres + "',";
            SQL += "'" + GSM + "',";
            SQL += "'" + EPosta + "',";
            SQL += "'" + Fax + "',";
            SQL += "'" + TcNo + "',";
            SQL += "Convert(Datetime ,'" + DogumTarihi.ToString("yyyy-MM-dd") + "',120),";
            SQL += "'" + PasapartNo + "',";
            SQL += "'" + Ulke + "',";
            SQL += "'" + Unvan + "',";
            SQL += "'" + VergiDairesi + "',";
            SQL += "'" + VergiNo + "',";
            SQL += "  " + Yas + " ,";
            SQL += "'" + PasaportBitisTarihi + "',";
            SQL += "'" + RezNo + "',";
            SQL += "'" + Cinsiyet + "',";
            
            SQL += "Convert(Datetime ,'" + EklendigiTarih.ToString("yyyy-MM-dd") + "',120),";
            SQL += "'" + EkleyenPersonel + "',";
            SQL += "Convert(Datetime ,'" + GuncellendigiTarih.ToString("yyyy-MM-dd") + "',120),";
            SQL += "'" + GuncelleyenPersonel + "',  ";
            SQL += "Convert(Datetime ,'" + MailGöndermeTarihi.ToString("yyyy-MM-dd") + "',120)";
            SQL += ") SELECT @@IDENTITY AS ID ";   

            DataSet DS = new DataSet();
            try
            {
                this.FillDataSet(DS, SQL);
                if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
                {
                    m_ID = int.Parse(DS.Tables[0].Rows[0]["ID"].ToString());

                }
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message + " Hatasql:" + SQL);
            }
            finally
            {
                DS.Dispose();
            }
            return 0;
        }
		
		
		
		
		
		
         private int KaydetUpdate()
        {
            string SQL = null;

            SQL = "UPDATE Musteriler SET ";
            SQL += "Ad='" + Ad + "',";
            SQL += "Soyad='" + Soyad + "',";
            SQL += "Telefon='" + Telefon + "',";
            SQL += "Adres='" + Adres + "',";
            SQL += "GSM='" + GSM + "',";
            SQL += "EPosta='" + EPosta + "',";
            SQL += "Fax='" + Fax + "',";
            SQL += "TcNo='" + TcNo + "',";
            SQL += "DogumTarihi = Convert(Datetime ,'" + DogumTarihi.ToString("yyyy-MM-dd") + "',120),";
            SQL += "PasapartNo='" + PasapartNo + "',";
            SQL += "Ulke='" + Ulke + "',";
            SQL += "Unvan='" + Unvan + "',";
            SQL += "VergiDairesi='" + VergiDairesi + "',";
            SQL += "VergiNo='" + VergiNo + "',";
            SQL += "Yas=  " + Yas + " ,";
            SQL += "PasaportBitisTarihi='" + PasaportBitisTarihi + "',";
            SQL += "RezNo='" + RezNo + "',";
            SQL += "Cinsiyet='" + Cinsiyet + "',";
          
            SQL += "EklendigiTarih = Convert(Datetime ,'" + EklendigiTarih.ToString("yyyy-MM-dd") + "',120),";
            SQL += "EkleyenPersonel='" + EkleyenPersonel + "',";
            SQL += "GuncellendigiTarih = Convert(Datetime ,'" + GuncellendigiTarih.ToString("yyyy-MM-dd") + "',120),";
            SQL += "GuncelleyenPersonel='" + GuncelleyenPersonel + "',  ";
            SQL += "MailGöndermeTarihi = Convert(Datetime ,'" + MailGöndermeTarihi.ToString("yyyy-MM-dd") + "',120)";
            SQL += " WHERE ID=" + m_ID;

            try
            {
                this.ExecuteSQL(SQL);
            }
            catch (Exception exp)
            {
                throw new Exception(exp.Message + " Hatasql:" + SQL);
            }
            return 0;
        }
	
	
	
	
	
	
       public object Delete()
        {
            m_SQL = "Delete from Musteriler where ID=" + m_ID;
            this.ExecuteSQL(m_SQL);
            return true;
        }

 
	}
}	

