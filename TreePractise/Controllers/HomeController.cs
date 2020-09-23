using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TreePractise.Controllers
{
    public class HomeController : Controller
    {
        private SqlConnection conn;
        private SqlDataAdapter da;
        private DataTable dt;

        public ActionResult Index()
        {
            ViewBag.DOM_TreeViewMenu = PopulateMenuDataTable();

            return View();
         
        }

		private string PopulateMenuDataTable()
		{
			string DOM = "";

			string sql = @"SELECT MenuNumber, ParentNumber, MenuName, Uri, Icon FROM example";
			conn = new SqlConnection(@"Data Source = IXC1-LT1BTJ2X2; 
	                                Initial Catalog = TreeView; 
	                                persist security info=True;Integrated Security=SSPI");
			conn.Open();

			da = new SqlDataAdapter(sql, conn);
			da.SelectCommand.CommandTimeout = 10000;

			dt = new DataTable();
			da.Fill(dt);

			if (conn.State == ConnectionState.Open)
			{
				conn.Close();
			}
			conn.Dispose();

			DOM = GetDOMTreeView(dt);

			return DOM;
		}
		private string GetDOMTreeView(DataTable dt)
		{
			string DOMTreeView = "";

			DOMTreeView += CreateTreeViewOuterParent(dt);
			DOMTreeView += CreateTreeViewMenu(dt, "0");

			DOMTreeView += "</ul>";

			return DOMTreeView;
		}
		private string CreateTreeViewOuterParent(DataTable dt)
		{
			string DOMDataList = "";

			DataRow[] drs = dt.Select("MenuNumber = 0");

			foreach (DataRow row in drs)
			{
				//row[2], 2 is column number start with 0, which is the MenuName
				DOMDataList = "<ul><li class='header'>" + row[2].ToString() + "</li>";
			}

			return DOMDataList;
		}
		private string CreateTreeViewMenu(DataTable dt, string ParentNumber)
		{
			string DOMDataList = "";

			string menuNumber = "";
			string menuName = "";
			string uri = "";
			string icon = "";

			DataRow[] drs = dt.Select("ParentNumber = " + ParentNumber);

			foreach (DataRow row in drs)
			{
				menuNumber = row[0].ToString();
				menuName = row[2].ToString();
				uri = row[3].ToString();
				icon = row[4].ToString();

				DOMDataList += "<li class='treeview'>";
				DOMDataList += "<a  onClick=showmenu('" + menuName.Replace(" ", "") + "')><i class='" + icon + "'></i><span>  "
								+ menuName + "</span></a>";

				DataRow[] drschild = dt.Select("ParentNumber = " + menuNumber);
				if (drschild.Count() != 0)
				{
					DOMDataList += "<ul class='treeview-menu' id='"+ menuName.Replace(" ", "") + "' style='display:none;'>";
					DOMDataList += CreateTreeViewMenu(dt, menuNumber).Replace
								   ("<li class='treeview'>", "<li>");
					DOMDataList += "</ul></li>";
				}
				else
				{
					DOMDataList += "</li>";
				}
			}
			return DOMDataList;
		}
	}
}