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

    public class Personeller : DataAccesBase
    {

        string m_SQL;
        int m_ConCount;
		int m_ID; 


        public String Adi;
        public String Soyadi;
        public String Sifre;
        public String KullaniciAdi;
        public String EMail;
		    


        public int ID
        {
            get { return m_ID; }
        }

        public Personeller ()
        {
        }
        public Personeller (int pID)
        {
            m_SQL = "Select * from Personeller where ID=" + pID;
            initialize();
        }

        public Personeller(string pFIELD_NAME, string pVALUE)
        {
            m_SQL = "Select * from Personeller where " + pFIELD_NAME + "='" + pVALUE + "'";
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
                Adi = Convert.ToString(DT.Rows[0]["Adi"]);
                Soyadi = Convert.ToString(DT.Rows[0]["Soyadi"]);
                Sifre = Convert.ToString(DT.Rows[0]["Sifre"]);
                KullaniciAdi = Convert.ToString(DT.Rows[0]["KullaniciAdi"]);
                EMail = Convert.ToString(DT.Rows[0]["EMail"]);

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

			SQL="Insert Into Personeller (Adi, Soyadi, Sifre, KullaniciAdi, ";
            SQL += "EMail) values (";
            SQL += "'" + Adi + "',";
            SQL += "'" + Soyadi + "',";
            SQL += "'" + Sifre + "',";
            SQL += "'" + KullaniciAdi + "',";
            SQL += "'" + EMail + "'  ";
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

            SQL = "UPDATE Personeller SET ";
            SQL += "Adi='" + Adi + "',";
            SQL += "Soyadi='" + Soyadi + "',";
            SQL += "Sifre='" + Sifre + "',";
            SQL += "KullaniciAdi='" + KullaniciAdi + "',";
            SQL += "EMail='" + EMail + "'  ";
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
            m_SQL = "Delete from Personeller where ID=" + m_ID;
            this.ExecuteSQL(m_SQL);
            return true;
        }

 
	}
}	

