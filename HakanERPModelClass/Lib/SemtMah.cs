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

    public class SemtMah : DataAccesBase
    {

        string m_SQL;
        int m_ConCount;
		int m_SemtMahId; 


        public String SemtAdi;
        public String MahalleAdi;
        public String PostaKodu;
        public Int32 ilceId;
		    


        public int SemtMahId
        {
            get { return m_SemtMahId; }
        }

        public SemtMah ()
        {
        }
        public SemtMah (int pSemtMahId)
        {
            m_SQL = "Select * from SemtMah where SemtMahId=" + pSemtMahId;
            initialize();
        }

        public SemtMah(string pFIELD_NAME, string pVALUE)
        {
            m_SQL = "Select * from SemtMah where " + pFIELD_NAME + "='" + pVALUE + "'";
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
                    m_SemtMahId = 0;
                    return true;
                }
                m_SemtMahId = Convert.ToInt32( DT.Rows[0]["SemtMahId"]);
                SemtAdi = Convert.ToString(DT.Rows[0]["SemtAdi"]);
                MahalleAdi = Convert.ToString(DT.Rows[0]["MahalleAdi"]);
                PostaKodu = Convert.ToString(DT.Rows[0]["PostaKodu"]);
                ilceId = Convert.ToInt32(DT.Rows[0]["ilceId"]);

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
                switch (m_SemtMahId)
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

			SQL="Insert Into SemtMah (SemtAdi, MahalleAdi, PostaKodu, ilceId";
            SQL += ") values (";
            SQL += "'" + SemtAdi + "',";
            SQL += "'" + MahalleAdi + "',";
            SQL += "'" + PostaKodu + "',";
            SQL += "  " + ilceId + "   ";
            SQL += ") SELECT @@IDENTITY AS SemtMahId ";   

            DataSet DS = new DataSet();
            try
            {
                this.FillDataSet(DS, SQL);
                if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
                {
                    m_SemtMahId = int.Parse(DS.Tables[0].Rows[0]["SemtMahId"].ToString());

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

            SQL = "UPDATE SemtMah SET ";
            SQL += "SemtAdi='" + SemtAdi + "',";
            SQL += "MahalleAdi='" + MahalleAdi + "',";
            SQL += "PostaKodu='" + PostaKodu + "',";
            SQL += "ilceId=  " + ilceId + "   ";
            SQL += " WHERE SemtMahId=" + m_SemtMahId;

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
            m_SQL = "Delete from SemtMah where SemtMahId=" + m_SemtMahId;
            this.ExecuteSQL(m_SQL);
            return true;
        }

 
	}
}	

