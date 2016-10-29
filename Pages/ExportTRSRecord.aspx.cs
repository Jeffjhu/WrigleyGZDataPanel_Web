using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.IO;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace WrigleyDataPanel.Pages
{
    public partial class ExportTRSRecord : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
              string line = Request.Params["Line"];
              selLine.Text = line;
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string line = selLine.Text;
            string dayFrom = this.dayFrom.Text;
            string dayTo = this.dayTo.Text;
            string strConn = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DataPanelConnectionString"].ToString();         

            SqlConnection con = new SqlConnection(strConn);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_GetTRSLossForExport";
            SqlParameter sp1 = new SqlParameter();

            sp1.ParameterName = "@Line";
            sp1.Value = line;
            cmd.Parameters.Add(sp1);

            SqlParameter sp2 = new SqlParameter();
            sp2.ParameterName = "@DayFrom";
            sp2.Value = dayFrom;
            cmd.Parameters.Add(sp2);

            SqlParameter sp3 = new SqlParameter();
            sp3.ParameterName = "@DayTo";
            sp3.Value = dayTo;
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
            catch(Exception ex)
            {
                Response.Write("<script>alert('数据库操作发生意外错误! 描述: '" + ex.Message + ")</script>");
                return;
            }
            finally
            {
                con.Close();
            }

            //开始导出过程
            DataGrid dgExcel = new DataGrid();
            ds.Tables[0].TableName = "TRS Records";
            try
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string execlName = "TRS_Records_" + dayFrom + "_" + dayTo;
                    Encoding encodingType = System.Text.Encoding.UTF8;
                    dgExcel.DataMember = ds.Tables[0].TableName;
                    dgExcel.DataSource = ds.Tables[0];

                    Response.Buffer = true;
                    Response.Charset = "utf-8";
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + execlName + ".xls");
                    Response.ContentEncoding = encodingType;
                    Response.ContentType = "application/ms-excel";
                    StringWriter oStringWriter = new StringWriter();
                    HtmlTextWriter oHtmlTextWriter = new HtmlTextWriter(oStringWriter);
                    dgExcel.DataBind();
                    dgExcel.Visible = true;
                    dgExcel.RenderControl(oHtmlTextWriter);
                    Response.Write(oStringWriter.ToString());
                    Response.Flush();
                    Response.Close();
                    dgExcel.DataSource = null;
                    dgExcel.Visible = false;
                }
                else
                {
                    Response.Write("<script>alert('没有任何数据可供导出!')</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('导出TRS停机记录时发生错误!" + ex.Message + "')</script>");
            } 
           //导出结束
        }
    }
}