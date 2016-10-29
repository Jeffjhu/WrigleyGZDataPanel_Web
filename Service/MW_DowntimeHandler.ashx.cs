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
    /// MW_DowntimeHandler 的摘要说明
    /// </summary>
    public class MW_DowntimeHandler : IHttpHandler
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
            string line = Request.Params["Line"].ToUpper();
            string strConn = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DataPanelConnectionString"].ToString();
            string SQL = string.Format("SELECT * FROM dbo.DowntimeCode  WHERE DowntimeClass='Plan downtime' AND CHARINDEX(Line,'{0}') > 0; SELECT * FROM dbo.DowntimeCode  WHERE DowntimeClass='Unplan downtime' AND CHARINDEX(Line, '{1}') > 0",line,line);

            SqlConnection con = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = SQL;
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
                return;
            }
            finally
            {
                con.Close();
            }

            var obj = new
            {
                PDownTime = Kit.ToDicList(ds.Tables[0]),
                UDownTime = Kit.ToDicList(ds.Tables[1]),
            };
                Response.Write(Kit.Serobj(obj));
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