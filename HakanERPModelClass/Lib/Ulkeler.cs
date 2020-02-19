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

    public class Ulkeler : DataAccesBase
    {

        string m_SQL;
        int m_ConCount;
		int m_UlkeId; 


        public String IkiliKod;
        public String UcluKod;
        public String UlkeAdi;
        public String TelKodu;
		    


        public int UlkeId
        {
            get { return m_UlkeId; }
        }

        public Ulkeler ()
        {
        }
        public Ulkeler (int pUlkeId)
        {
            m_SQL = "Select * from Ulkeler where UlkeId=" + pUlkeId;
            initialize();
        }

        public Ulkeler(string pFIELD_NAME, string pVALUE)
        {
            m_SQL = "Select * from Ulkeler where " + pFIELD_NAME + "='" + pVALUE + "'";
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
                    m_UlkeId = 0;
                    return true;
                }
                m_UlkeId = Convert.ToInt32( DT.Rows[0]["UlkeId"]);
                IkiliKod = Convert.ToString(DT.Rows[0]["IkiliKod"]);
                UcluKod = Convert.ToString(DT.Rows[0]["UcluKod"]);
                UlkeAdi = Convert.ToString(DT.Rows[0]["UlkeAdi"]);
                TelKodu = Convert.ToString(DT.Rows[0]["TelKodu"]);

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
                switch (m_UlkeId)
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

			SQL="Insert Into Ulkeler (IkiliKod, UcluKod, UlkeAdi, TelKodu";
            SQL += ") values (";
            SQL += "'" + IkiliKod + "',";
            SQL += "'" + UcluKod + "',";
            SQL += "'" + UlkeAdi + "',";
            SQL += "'" + TelKodu + "'  ";
            SQL += ") SELECT @@IDENTITY AS UlkeId ";   

            DataSet DS = new DataSet();
            try
            {
                this.FillDataSet(DS, SQL);
                if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
                {
                    m_UlkeId = int.Parse(DS.Tables[0].Rows[0]["UlkeId"].ToString());

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

            SQL = "UPDATE Ulkeler SET ";
            SQL += "IkiliKod='" + IkiliKod + "',";
            SQL += "UcluKod='" + UcluKod + "',";
            SQL += "UlkeAdi='" + UlkeAdi + "',";
            SQL += "TelKodu='" + TelKodu + "'  ";
            SQL += " WHERE UlkeId=" + m_UlkeId;

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
            m_SQL = "Delete from Ulkeler where UlkeId=" + m_UlkeId;
            this.ExecuteSQL(m_SQL);
            return true;
        }

 
	}
}	

