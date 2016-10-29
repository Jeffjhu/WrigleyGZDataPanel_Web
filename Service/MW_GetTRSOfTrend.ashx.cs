using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Macrowill.WebProject.Common;

namespace WrigleyDataPanel.Service
{
    /// <summary>
    /// MW_GetTRSOfTimeSpan 的摘要说明
    /// </summary>
    public class MW_GetTRSOfTrend : IHttpHandler
    {

        public HttpRequest Request
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }

        public HttpResponse Response
        {
            get
            {
                return HttpContext.Current.Response;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            Response.ContentType = "application/json";
            string line = Request.Params["Line"].ToString();
            string dayFrom = Request.Params["DayFrom"].ToString();
            string dayTo = Request.Params["DayTo"].ToString();

            DateTime dtFrom = DateTime.Parse(dayFrom);
            DateTime dtTo = DateTime.Parse(dayTo);
  
            string strConn = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DataPanelConnectionString"].ToString();

            SqlConnection con = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_GetVolume";
            cmd.Connection = con;

            SqlDataReader dr = null;
            SqlParameter[] pm = new SqlParameter[]{null,null,null};

            float trs = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"Trend\":[");

            try
            {
                pm[0] = new SqlParameter();
                pm[0].ParameterName = "@Line";
                pm[0].DbType = DbType.String;
                pm[0].Value = line;

                pm[1] = new SqlParameter();
                pm[1].ParameterName = "@Day";
                pm[1].DbType = DbType.String;

                pm[2] = new SqlParameter();
                pm[2].ParameterName = "@Shift";
                pm[2].DbType = DbType.Int32;

                con.Open();

                while (dtFrom <= dtTo)
                {
                    pm[1].Value = dtFrom.ToString("yyyy-MM-dd");
                    pm[2].Value = 1;

                    cmd.Parameters.AddRange(pm);

                    dr = cmd.ExecuteReader();
                    trs += CalTRS(dr);
                    dr.Close();
                    dr.Dispose();

                    cmd.Parameters[2].Value = 2;

                    dr = cmd.ExecuteReader();
                    trs += CalTRS(dr);
                    dr.Close();
                    dr.Dispose();

                    cmd.Parameters[2].Value = 3;

                    dr = cmd.ExecuteReader();
                    trs += CalTRS(dr);
                    dr.Close();

                    sb.Append("{" + string.Format("\"Point\":\"{0}\",\"TRS\":{1}",dtFrom.ToString("yyyy-MM-dd"),(trs / 3.0).ToString("F2")) + "},");
                    dtFrom = dtFrom.AddDays(1);
                    cmd.Parameters.Clear();
                    trs = 0;
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append("]}");
                Response.Write(sb.ToString());
                Response.End();
            }
            finally
            {
                con.Close();
            }

        }

        private float CalTRS(SqlDataReader dr)
        {
            int spanCnt = 0;
            float aVol = 0;
            float sVol = 0;
            List<float> trs = new List<float>();
            List<float> trsVol = new List<float>();
            string lastCode = string.Empty;

            while (dr.Read())
            {
                if (dr["Status"].ToString() == "0")
                {
                   if(dr["RecipeCode"].ToString().ToUpper() == lastCode)
                    {
                        spanCnt++;
                        aVol = float.Parse(dr["aVolume"].ToString());
                        sVol = float.Parse(dr["sVolume"].ToString()) * 30 * spanCnt;
                        trs.Add(aVol / sVol);
                        trsVol.Add(sVol);
                    }
                    else
                    {
                        spanCnt = 0;                     
                        trs.Add(0);
                        trsVol.Add(0);
                        lastCode = dr["RecipeCode"].ToString().ToUpper();
                    }
                }
                else
                {
                    trs.Add(0);
                    trsVol.Add(0);
                }
            }

            float tolStdVol = 0;
            float tolActualVol = 0;
            float rate = 0;

            for (int i = 0; i < trs.Count; i++)
            {
                tolActualVol += trsVol[i] * trs[i];
                tolStdVol += trsVol[i];
            }

            if (tolStdVol == 0)
                rate = 0;
            else
                rate = tolActualVol / tolStdVol;

                return rate;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }

}