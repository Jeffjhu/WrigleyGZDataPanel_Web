using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Macrowill.WebProject.Common
{
    public class Kit
    {    
        #region DataTable转为json
        /// <summary> 
        /// DataTable转为List<Dictionary<string, object>>list
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns>List<Dictionary<string, object>></returns> 
        public static List<Dictionary<string, object>> ToDicList(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, Convert.IsDBNull(dr[dc]) ? "" : dr[dc].ToString());
                }
                list.Add(result);
            }
            return list;
        }

        public static string Serobj(object obj)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            return js.Serialize(obj);
        }

        #endregion
    }
}