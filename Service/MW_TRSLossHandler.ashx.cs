using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Macrowill.WebProject.Common;

namespace Macrowill.WebProject.WrigleyDataPanelService
{
    /// <summary>
    /// MW_TRSLossHandler 的摘要说明
    /// </summary>
    public class MW_TRSLossHandler : IHttpHandler
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
            string method = Request.HttpMethod.ToLower();

            string strConn = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DataPanelConnectionString"].ToString();
            SqlConnection con = new SqlConnection(strConn);

            //处理请求返回数据
            if (method == "get")
            {
               string line = Request.Params["Line"].ToUpper();
               string day = Request.Params["Date"].ToString();
               string shift = Request.Params["Shift"].ToString();

               SqlCommand cmd = new SqlCommand();
               cmd.CommandType = CommandType.StoredProcedure;
               cmd.CommandText = "SP_GetTRSLoss";

              SqlParameter sp1 = new SqlParameter();
              sp1.ParameterName = "@Line";
              sp1.SqlDbType = SqlDbType.NVarChar;
              sp1.Direction = ParameterDirection.Input;
              sp1.SqlValue = line;
              cmd.Parameters.Add(sp1);

              SqlParameter sp2 = new SqlParameter();
              sp2.ParameterName = "@Day";
              sp2.SqlDbType = SqlDbType.NVarChar;
              sp2.Direction = ParameterDirection.Input;
              sp2.SqlValue = day;
              cmd.Parameters.Add(sp2);

              SqlParameter sp3 = new SqlParameter();
              sp3.ParameterName = "@Shift";
              sp3.SqlDbType = SqlDbType.Int;
              sp3.Direction = ParameterDirection.Input;
              sp3.SqlValue = int.Parse(shift);
              cmd.Parameters.Add(sp3);

              cmd.Connection = con;

              SqlDataAdapter da = new SqlDataAdapter();
              da.SelectCommand = cmd;
              DataSet ds = new DataSet();

              try
              {
                 con.Open();
                 da.Fill(ds);
              }
              catch
              {
                  Response.Write("{\"ERR\":1}");
                 return;
              }
              finally
              {
                 con.Close();
              }

               var obj = new { RUNNING = ds.Tables[1].Rows.Count == 0?0:ds.Tables[1].Rows[0][0], TRSLoss = Kit.ToDicList(ds.Tables[0]) };
               Response.Write(Kit.Serobj(obj));
             }

              //更新数据
              if (method == "post")
              {
                  string SQL = string.Empty;
                  var jsSer = new System.Web.Script.Serialization.JavaScriptSerializer();
                  string data = Request.Params["data"];

                  if (string.IsNullOrEmpty(data))
                  {
                      Response.Write("{\"ERR\":1}");
                      Response.End();
                   }

                   var list = jsSer.Deserialize<List<Dictionary<string, string>>>(data);
                   SqlCommand cmd = new SqlCommand();
                   cmd.CommandType = CommandType.Text;
                   cmd.Connection = con;

                   try
                   {
                       foreach (var item in list)
                       {
                          SQL += string.Format("UPDATE dbo.TRSLoss SET DownTimeCode = '{0}' WHERE Id = {1};", item["code"], item["id"]);
                       }
                       cmd.CommandText = SQL;
                       con.Open();
                       cmd.ExecuteNonQuery();                  
                       Response.Write("{\"ERR\":2}");
                    }
                    catch
                    {
                        Response.Write("{\"ERR\":3}");
                    }

                 }

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