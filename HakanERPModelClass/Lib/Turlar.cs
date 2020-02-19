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

    public class Turlar : DataAccesBase
    {

        string m_SQL;
        int m_ConCount;
		int m_ID; 


        public String TurAdi;
        public System.DateTime BaslangicTarihi = Convert.ToDateTime("1900-01-01");
        public System.DateTime BitisTarihi = Convert.ToDateTime("1900-01-01");

        public System.DateTime ET = Convert.ToDateTime("1900-01-01");
        public String EP;
        public System.DateTime GT = Convert.ToDateTime("1900-01-01");
        public String GP;
		    


        public int ID
        {
            get { return m_ID; }
        }

        public Turlar ()
        {
        }
        public Turlar (int pID)
        {
            m_SQL = "Select * from Turlar where ID=" + pID;
            initialize();
        }

        public Turlar(string pFIELD_NAME, string pVALUE)
        {
            m_SQL = "Select * from Turlar where " + pFIELD_NAME + "='" + pVALUE + "'";
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
                TurAdi = Convert.ToString(DT.Rows[0]["TurAdi"]);
                BaslangicTarihi = Convert.ToDateTime(DT.Rows[0]["BaslangicTarihi"]);
                BitisTarihi = Convert.ToDateTime(DT.Rows[0]["BitisTarihi"]);
                ET = Convert.ToDateTime(DT.Rows[0]["ET"]);
                EP = Convert.ToString(DT.Rows[0]["EP"]);
                GT = Convert.ToDateTime(DT.Rows[0]["GT"]);
                GP = Convert.ToString(DT.Rows[0]["GP"]);

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

			SQL="Insert Into Turlar (TurAdi, BaslangicTarihi, BitisTarihi, ET, ";
            SQL += "EP, GT, GP) values (";
            SQL += "'" + TurAdi + "',";
            SQL += "Convert(Datetime ,'" + BaslangicTarihi.ToString("yyyy-MM-dd") + "',120),";
            SQL += "Convert(Datetime ,'" + BitisTarihi.ToString("yyyy-MM-dd") + "',120),";
            SQL += "Convert(Datetime ,'" + ET.ToString("yyyy-MM-dd") + "',120),";
            SQL += "'" + EP + "',";
            SQL += "Convert(Datetime ,'" + GT.ToString("yyyy-MM-dd") + "',120),";
            SQL += "'" + GP + "'  ";
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

            SQL = "UPDATE Turlar SET ";
            SQL += "TurAdi='" + TurAdi + "',";
            SQL += "BaslangicTarihi = Convert(Datetime ,'" + BaslangicTarihi.ToString("yyyy-MM-dd") + "',120),";
            SQL += "BitisTarihi = Convert(Datetime ,'" + BitisTarihi.ToString("yyyy-MM-dd") + "',120),";
            SQL += "ET = Convert(Datetime ,'" + ET.ToString("yyyy-MM-dd") + "',120),";
            SQL += "EP='" + EP + "',";
            SQL += "GT = Convert(Datetime ,'" + GT.ToString("yyyy-MM-dd") + "',120),";
            SQL += "GP='" + GP + "'  ";
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
            m_SQL = "Delete from Turlar where ID=" + m_ID;
            this.ExecuteSQL(m_SQL);
            return true;
        }

 
	}
}	

