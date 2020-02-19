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

    public class TurMus : DataAccesBase
    {

        string m_SQL;
        int m_ConCount;
		int m_ID; 


        public Int32 MusteriID;
        public Int32 TurID;
        public Int32 PersonelID;
        public System.DateTime EkTarih = Convert.ToDateTime("1900-01-01");
        public String EkPersonel;
        public System.DateTime GuTarih = Convert.ToDateTime("1900-01-01");
        public String GuPersonel;
        public Int32 Tutar;
		    


        public int ID
        {
            get { return m_ID; }
        }

        public TurMus ()
        {
        }
        public TurMus (int pID)
        {
            m_SQL = "Select * from TurMus where ID=" + pID;
            initialize();
        }

        public TurMus(string pFIELD_NAME, string pVALUE)
        {
            m_SQL = "Select * from TurMus where " + pFIELD_NAME + "='" + pVALUE + "'";
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
                MusteriID = Convert.ToInt32(DT.Rows[0]["MusteriID"]);
                TurID = Convert.ToInt32(DT.Rows[0]["TurID"]);
                PersonelID = Convert.ToInt32(DT.Rows[0]["PersonelID"]);
                EkTarih = Convert.ToDateTime(DT.Rows[0]["EkTarih"]);
                EkPersonel = Convert.ToString(DT.Rows[0]["EkPersonel"]);
                GuTarih = Convert.ToDateTime(DT.Rows[0]["GuTarih"]);
                GuPersonel = Convert.ToString(DT.Rows[0]["GuPersonel"]);
                Tutar = Convert.ToInt32(DT.Rows[0]["Tutar"]);
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

			SQL="Insert Into TurMus (MusteriID, TurID, PersonelID, EkTarih, ";
            SQL += "EkPersonel, GuTarih, GuPersonel, Tutar) values (";
            SQL += "  " + MusteriID + " ,";
            SQL += "  " + TurID + " ,";
            SQL += "  " + PersonelID + " ,";
            SQL += "Convert(Datetime ,'" + EkTarih.ToString("yyyy-MM-dd") + "',120),";
            SQL += "'" + EkPersonel + "',";
            SQL += "Convert(Datetime ,'" + GuTarih.ToString("yyyy-MM-dd") + "',120),";
            SQL += "'" + GuPersonel + "',  ";
            SQL += "  " + Tutar + " ";
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

            SQL = "UPDATE TurMus SET ";
            SQL += "MusteriID=  " + MusteriID + " ,";
            SQL += "TurID=  " + TurID + " ,";
            SQL += "PersonelID=  " + PersonelID + " ,";
            SQL += "EkTarih = Convert(Datetime ,'" + EkTarih.ToString("yyyy-MM-dd") + "',120),";
            SQL += "EkPersonel='" + EkPersonel + "',";
            SQL += "GuTarih = Convert(Datetime ,'" + GuTarih.ToString("yyyy-MM-dd") + "',120),";
            SQL += "GuPersonel='" + GuPersonel + "',  ";
            SQL += "Tutar='" + Tutar + "'  ";
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
            m_SQL = "Delete from TurMus where ID=" + m_ID;
            this.ExecuteSQL(m_SQL);
            return true;
        }

 
	}
}	

