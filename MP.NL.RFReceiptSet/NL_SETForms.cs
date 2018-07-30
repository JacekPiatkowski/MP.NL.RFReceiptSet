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
using Mantis.LVision.RFApi;

namespace MP.NL.RFReceiptSet
{

	class NL_SETForms : RFForm
	{
		//private System.Data.DataSet m_dsTask = null;
		private RFForm m_Form;

		public override bool Start()

		{

			string su="", ipbstrResult, strSQL, ProductCode, IA_SetValue;
			bool dada;
			//string ExtraInfo = "Produkt ma przypisaną wartość atrybutu SET";
			//ipbstrResult = null;
			//System.Data.DataSet ds;
			//ProductCode = "393260    SETX";
			//IA_SetValue = null;
			this.FormFile = "set\\Set22IAProductSelect.xml";

			Rf.PushScreen();
			//this.Rf.ClearScreen();
			
			//this.Rf.PopScreen();


			//Rf.ReadBufferByLine=true;
			/*Rf. = true;
			Rf.DisplayText("HELLO ", 5, 1);
			Rf.ReadString(ref su);*/
			/*RFControlText rFControlText = new RFControlText
			{
				Visible = true,
				Status = "kod",
				InputSize = 15,
				Size = 15,
				Message = "Skanuj kod produktu",
				Required = true,
				Reverse=false,
				IgnorePrevious=false,
				ControlType = Mantis.LVision.RFApi.enmControlType.ALFANUMERIC,
				OriginalValue="test"
			};
			//rFControlText.Value = "";
			Rf.ReadText(rFControlText, ref su);*/
			Routines dbr = new Routines();
			//Rf.DisplayMessage(ExtraInfo);
			Mantis.LVision.RFTools.frmGetValue frmVal = new Mantis.LVision.RFTools.frmGetValue
			{
				// frmVal.Application = this.Application;
				//frmVal.Parameters = this.Parameters;
				//frmVal.Rf = this.Rf;
				//frmVal.SessionID = this.SessionID;
				TitleMessage = "Zeskanuj ",
				Message = "kod produktu:",
				Size = 15,
				X = 1,
				Y = 1,
				
				Required = true,
				ControlType = Mantis.LVision.RFApi.enmControlType.ALFANUMERIC,
				Caption1_Visible = true,
				Parent = this.Parent
			};
			frmVal.ClearScreen();
			dada = frmVal.Show();
			frmVal.PushScreen();

			//frmVal.ShowForm(m_Form);

			if (frmVal.Value==null|| frmVal.Value=="")
			{
				Rf.DisplayError("zeskanuj kod produktu!!");
			}

			



			//strSQL = "select '1' id, 'TAK' rep UNION ALL  select '2' id, 'NIE' rep";
			

			//ds = dbr.SelectTable(strSQL, m_Form, null);
			//su = m_Form.Rf.DisplayCombo(1, 1, "Produkt: "+ProductCode+(char)13+"Typ SET: " + IA_SetValue.ToString(), "Zmiana? : ", ref ipbstrResult, ds.Tables[0], "id", "id", "rep", eEchoMode.ECHO_ON, null, true, false, "1", true);
	


			return false;
		}

	}
}
