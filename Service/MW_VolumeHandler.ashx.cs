﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Macrowill.WebProject.Common;

namespace Macrowill.WebProject.WrigleyDataPanelService
{
    /// <summary>
    /// MW_VolumeHandler 的摘要说明
    /// </summary>
    public class MW_VolumeHandler : IHttpHandler
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
            string day = Request.Params["Date"].ToString();
            string shift = Request.Params["Shift"].ToString();

            string strConn = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DataPanelConnectionString"].ToString();

            SqlConnection con = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_GetVolume";

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
                return;
            }
            finally
            {
                con.Close();
            }

            DataTable dt = ds.Tables[0];
            Response.Write(Kit.Serobj(Kit.ToDicList(dt)));
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