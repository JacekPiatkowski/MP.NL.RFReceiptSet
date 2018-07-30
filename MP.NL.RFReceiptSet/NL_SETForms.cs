using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mantis.LVision;
// using Mantis.LVision.Proxy.WsStock
using Mantis.LVision.DBAccess.DbRoutines;
using System.Globalization;
using System.Threading;

namespace MP.NL.RFReceiptSet
{

	class NL_SETForms : Mantis.LVision.RFApi.RFForm
	{
		private System.Data.DataSet m_dsTask = null;
		private Mantis.LVision.RFApi.RFForm m_frm;

		public override bool Start()

		{

			/*	string su, ipbstrResult;
	ipbstrResult = null;
	System.Data.DataSet ds;
	strSQL = "select '1' id, 'TAK' rep UNION ALL  select '2' id, 'NIE' rep";

	ds = dbr.SelectTable(strSQL, m_Form, null);
	su = m_Form.Rf.DisplayCombo(1, 1, "Produkt: "+ProductCode+(char)13+"Typ SET: " + IA_SetValue.ToString(), "Zmiana? : ", ref ipbstrResult, ds.Tables[0], "id", "id", "rep", eEchoMode.ECHO_ON, null, true, false, "1", true);
	*/


			return false;
		}

	}
}
