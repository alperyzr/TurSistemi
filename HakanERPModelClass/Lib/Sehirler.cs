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

    public class Sehirler : DataAccesBase
    {

        string m_SQL;
        int m_ConCount;
		int m_SehirId; 


        public String SehirAdi;
        public Int32 PlakaNo;
        public Int32 TelefonKodu;
        public Int32 RowNumber;
		    


        public int SehirId
        {
            get { return m_SehirId; }
        }

        public Sehirler ()
        {
        }
        public Sehirler (int pSehirId)
        {
            m_SQL = "Select * from Sehirler where SehirId=" + pSehirId;
            initialize();
        }

        public Sehirler(string pFIELD_NAME, string pVALUE)
        {
            m_SQL = "Select * from Sehirler where " + pFIELD_NAME + "='" + pVALUE + "'";
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
                    m_SehirId = 0;
                    return true;
                }
                m_SehirId = Convert.ToInt32( DT.Rows[0]["SehirId"]);
                SehirAdi = Convert.ToString(DT.Rows[0]["SehirAdi"]);
                PlakaNo = Convert.ToInt32(DT.Rows[0]["PlakaNo"]);
                TelefonKodu = Convert.ToInt32(DT.Rows[0]["TelefonKodu"]);
                RowNumber = Convert.ToInt32(DT.Rows[0]["RowNumber"]);

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
                switch (m_SehirId)
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

			SQL="Insert Into Sehirler (SehirAdi, PlakaNo, TelefonKodu, RowNumber";
            SQL += ") values (";
            SQL += "'" + SehirAdi + "',";
            SQL += "  " + PlakaNo + " ,";
            SQL += "  " + TelefonKodu + " ,";
            SQL += "  " + RowNumber + "   ";
            SQL += ") SELECT @@IDENTITY AS SehirId ";   

            DataSet DS = new DataSet();
            try
            {
                this.FillDataSet(DS, SQL);
                if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
                {
                    m_SehirId = int.Parse(DS.Tables[0].Rows[0]["SehirId"].ToString());

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

            SQL = "UPDATE Sehirler SET ";
            SQL += "SehirAdi='" + SehirAdi + "',";
            SQL += "PlakaNo=  " + PlakaNo + " ,";
            SQL += "TelefonKodu=  " + TelefonKodu + " ,";
            SQL += "RowNumber=  " + RowNumber + "   ";
            SQL += " WHERE SehirId=" + m_SehirId;

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
            m_SQL = "Delete from Sehirler where SehirId=" + m_SehirId;
            this.ExecuteSQL(m_SQL);
            return true;
        }

 
	}
}	

