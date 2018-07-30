using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mantis.LVision.RFTools;
using Mantis.LVision.RFApi;
using Mantis.LVision.RFProduct;
using Mantis.LVision.RFReceipt;
using System.Globalization;
using Mantis.LVision.DBAccess.DbRoutines;
using Mantis.LVision.Interfaces;


namespace MP.NL.RFReceiptSet
{
	/*custom RF forms handling*/
	public class RFTools_Forms_frmEmpty : IFormExit
	{
		private Forms.frmEmpty m_EmptyForm;
		private frmGetProduct m_ParentGetProduct;
		public int ProductID = 0;
		Routines dbr = new Routines();

		public RFTools_Forms_frmEmpty()
		{
		}

		public void Control_C_PrdID_Validating(object sender, OnValidationEventArgs e)
		{
			Mantis.LVision.RFApi.RFControlText Control_C_PrdID = GetControlByName("C_PrdID");
			if (Control_C_PrdID != null)
			{
				string key ="" ;
				string ProductID = Control_C_PrdID.Value.ToString();

				string strSQL = "SELECT LPA.pat_ID FROM dbo.LV_ProductAttributes LPA WHERE LPA.pat_Code='SET';";
				object queryReturnValue = dbr.SelectSingleValue(strSQL, m_EmptyForm);
				if (queryReturnValue != null)
				{
					string IA_SetID = queryReturnValue.ToString();
					strSQL = "SELECT  LPAV.pav_Value + ' - ' + LALV.all_Value FROM dbo.LV_ProductAttributesValues LPAV  INNER JOIN dbo.LV_ProductAttributeList LPAL ON LPAL.pal_AttributeID = LPAV.pav_attributeID  AND LPAL.pal_Code = LPAV.pav_Value  INNER JOIN dbo.LV_AttributeListValue LALV ON all_PrdAttrListID = pal_ID WHERE LPAV.pav_ProductID = " + ProductID + " AND LPAV.pav_attributeID = " + IA_SetID + " AND LALV.all_LanguageID = " + this.m_EmptyForm.LanguageID.ToString() + ";";
					object queryReturnedValue = dbr.SelectSingleValue(strSQL, m_EmptyForm);
					if (queryReturnedValue != null)
					{
						string IA_SetValue = queryReturnedValue.ToString();
						m_EmptyForm.Rf.ClearScreen();
						/*m_EmptyForm.Rf.DisplayText("Produkt ma już  ustawiony SET:  " + IA_SetValue.ToString(), 1, 1);
						m_EmptyForm.Rf.DisplayText("Chcesz zmienić?", 5, 1);
						m_EmptyForm.Rf.DisplayText("1 - Tak", 7, 1);
						m_EmptyForm.Rf.DisplayText("2 - Nie", 8, 1);
						m_EmptyForm.Rf.ReadString(ref key);*/
						strSQL = "select '1' id, 'TAK' rep UNION ALL  select '2' id, 'NIE' rep";

						string  ipbstrResult = null;
						m_EmptyForm.Rf.ClearScreen();
						System.Data.DataSet ds = dbr.SelectTable(strSQL, m_EmptyForm, null);
						key = m_EmptyForm.Rf.DisplayCombo(1, 1, "Produkt ma już  ustawiony SET:  " + IA_SetValue.ToString(), "Chcesz zmienić?:", ref ipbstrResult, ds.Tables[0], "id", "rep", null, eEchoMode.ECHO_ON, null, true, false, "1", true);
						RFControlText combokey = GetControlByName("combokey");
						combokey.Value = key;
						combokey.ReadOnly = true;
						combokey.OriginalValue = key;
					}
				}
			}
		}

		/*public void Forms_frmEmpty_Validating (object sender, OnValidatingEventArgs e)
		{
			string any = "";
			
		}*/
		public void Initialize(RFForm args)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			if (!(args is Forms.frmEmpty))
				return;
			m_EmptyForm = (Forms.frmEmpty)args;
			

			/*only if current form i Set27IAUpdate.xml*/
			if (m_EmptyForm.FormFile != null /*&& string.Compare(m_EmptyForm.FormFile.ToString(), "Set27IAUpdate.xml", true) == 0*/
				&& m_EmptyForm.FormFile.ToString().ToLower(invariantCulture).EndsWith("Set27IAUpdate.xml".ToLower(invariantCulture)))
			{
				{
					/*only if we have parent of parent and it is frmGetProduct: Set27IAUpdate->Set26IASet->frmGetProduct*/
					if (m_EmptyForm.Parent.Parent != null && string.Compare(m_EmptyForm.Parent.Parent.GetType().ToString().ToLower(invariantCulture), "Mantis.LVision.RFProduct.frmGetProduct".ToLower(invariantCulture), false) == 0)
					{
						m_ParentGetProduct = (frmGetProduct)m_EmptyForm.Parent.Parent;
						Mantis.LVision.RFApi.RFControlText Control_C_PrdID = GetControlByName("ProductID");
						if (Control_C_PrdID != null)
						{
							/*set C_PrdID value equal ProductID entered in frmGetProduct*/
							Control_C_PrdID.Value = m_ParentGetProduct.txtProduct.ProductID.ToString();
							Control_C_PrdID.OriginalValue = m_ParentGetProduct.txtProduct.ProductID.ToString();
						}
					}
				}
			}
			/*if Set22IAProductSelect*/
			if (m_EmptyForm.FormFile != null && m_EmptyForm.FormFile.ToString().ToLower(invariantCulture).EndsWith("Set22IAProductSelect.xml".ToLower(invariantCulture)))
			{
				RFControlText Control_C_PrdID = GetControlByName("C_PrdID");
				if (Control_C_PrdID != null)
				{
					Control_C_PrdID.Validating += new OnValidationEventHandler(this.Control_C_PrdID_Validating);
				}
				//m_EmptyForm.Validating += new OnValidatingEventHandler(this.Forms_frmEmpty_Validating);
			}
		}
		public void Close(RFForm args)
		{
		}

		private RFControlText GetControlByName(string name)
		{
			foreach (Mantis.LVision.RFApi.RFControl ctrl in m_EmptyForm.Controls)
			{
				if (ctrl.Name != string.Empty && ctrl.Name != null && ctrl.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) == true)
					return (RFControlText)ctrl;
			}
			return null;
		}
	}

	/*frmReceipt & frmGetProduct handling*/

	public class RFProduct_frmGetProduct : IFormExit
	{
		public System.Data.IDbConnection con ;
		private frmGetProduct m_Form;
		private frmReceipt m_Parent;
		public int ProductID = 0;
		private string ReceiptCode = "";
		private bool m_bReadOnlyMultiplier;

		public RFProduct_frmGetProduct()
		{
		}

		public void txtProduct_Validating(object sender, OnValidationEventArgs e)
		{
			string IA_SetID, IA_SetValue, strSQL, RFFolder, XMLFormPath,ProductCode ;
			CMenuTools RFtools = new CMenuTools();
			Routines dbr = new Routines();
			try
			{
				/*check if there are parameters*/
				if (m_Form.Parameters.Count > 0 || m_Form.Parameters != null)
				{
					/*get RFFolder path*/
					RFFolder = m_Form.Parameters.get_ItemString("RFFolder");

					ProductID = m_Form.txtProduct.ProductID;
					ProductCode = m_Form.txtProduct.ProductCode;
					if (ProductID > 0 && ReceiptCode != "")
					{
						strSQL = "SELECT LPA.pat_ID FROM dbo.LV_ProductAttributes LPA WHERE LPA.pat_Code='SET';";
						object queryReturnValue = dbr.SelectSingleValue(strSQL, m_Form);
						if (queryReturnValue!=null)
						{ IA_SetID = queryReturnValue.ToString(); 
						strSQL = "Select LPAV.pav_Value FROM dbo.LV_ProductAttributesValues LPAV WHERE LPAV.pav_ProductID=" + ProductID + " And  LPAV.pav_attributeID=" + IA_SetID + " ;";
						object queryReturnedValue=dbr.SelectSingleValue(strSQL, m_Form);
							if (queryReturnedValue != null)
							{
								IA_SetValue = queryReturnedValue.ToString();
							}
							else { IA_SetValue = null; }
							/*only if IA_SetValue is equal ? then ask user for change
							 if is null then create IA then ask user */
							if (IA_SetValue == null)
							{
								/*Setup item hierarhy and IA*/
								strSQL = "exec usp_SetItemSetup  @prdCode='" + ProductCode + "';";
								int execReturnedValue = dbr.Execute(strSQL, con, null, m_Form);
							}

							if (IA_SetValue == null || IA_SetValue == "?")
							{
								/*set xml form path*/
								XMLFormPath = RFFolder + "\\SET\\Set26IASet.xml";
								/*call RF form from XML*/
								CMenuTools.ShowFormFile(m_Form, XMLFormPath);	
							}
							
						}
					}
				}
			}
		
			catch (InvalidCastException ex)
			{
				m_Form.Rf.DisplayError(ex);
			}
		}
	


		public void Initialize(RFForm args)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			if (!(args is frmGetProduct))
				return;
			this.m_Form = (frmGetProduct)args;
			this.m_Parent = (frmReceipt)args.Parent;
			this.con = DBConnection.Open(m_Form);
			if (this.m_Form.Parent != null && string.Compare(this.m_Form.Parent.GetType().ToString().ToLower(invariantCulture), "Mantis.LVision.rfReceipt.frmReceipt".ToLower(invariantCulture), false) == 0)
			{
				
				ReceiptCode = m_Parent.txtReceiptCode.Value.ToString();
				this.m_Form.txtProduct.Validating += new OnValidationEventHandler(this.txtProduct_Validating);

				this.m_bReadOnlyMultiplier = true;
				this.m_Form.txtMultiplier.Enter += new OnEnterEventHandler(this.txtMultiplier_Enter);
			}
		}

		/*NL oryginal dll content*/
		public void txtMultiplier_Enter(object sender, OnEnterEventArgs e)
		{
			if (!this.m_bReadOnlyMultiplier)
				return;
			this.m_Form.txtMultiplier.ReadOnly = true;
		}





		public void Close(RFForm args)
		{
		}
	}


	
}
