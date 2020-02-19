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

    public class Ilceler : DataAccesBase
    {

        string m_SQL;
        int m_ConCount;
		int m_ilceId; 


        public Int32 SehirId;
        public String IlceAdi;
        public String SehirAdi;
		    


        public int ilceId
        {
            get { return m_ilceId; }
        }

        public Ilceler ()
        {
        }
        public Ilceler (int pilceId)
        {
            m_SQL = "Select * from Ilceler where ilceId=" + pilceId;
            initialize();
        }

        public Ilceler(string pFIELD_NAME, string pVALUE)
        {
            m_SQL = "Select * from Ilceler where " + pFIELD_NAME + "='" + pVALUE + "'";
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
                    m_ilceId = 0;
                    return true;
                }
                m_ilceId = Convert.ToInt32( DT.Rows[0]["ilceId"]);
                SehirId = Convert.ToInt32(DT.Rows[0]["SehirId"]);
                IlceAdi = Convert.ToString(DT.Rows[0]["IlceAdi"]);
                SehirAdi = Convert.ToString(DT.Rows[0]["SehirAdi"]);

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
                switch (m_ilceId)
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

			SQL="Insert Into Ilceler (SehirId, IlceAdi, SehirAdi) values (";
            SQL += "  " + SehirId + " ,";
            SQL += "'" + IlceAdi + "',";
            SQL += "'" + SehirAdi + "'  ";
            SQL += ") SELECT @@IDENTITY AS ilceId ";   

            DataSet DS = new DataSet();
            try
            {
                this.FillDataSet(DS, SQL);
                if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
                {
                    m_ilceId = int.Parse(DS.Tables[0].Rows[0]["ilceId"].ToString());

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

            SQL = "UPDATE Ilceler SET ";
            SQL += "SehirId=  " + SehirId + " ,";
            SQL += "IlceAdi='" + IlceAdi + "',";
            SQL += "SehirAdi='" + SehirAdi + "'  ";
            SQL += " WHERE ilceId=" + m_ilceId;

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
            m_SQL = "Delete from Ilceler where ilceId=" + m_ilceId;
            this.ExecuteSQL(m_SQL);
            return true;
        }

 
	}
}	

