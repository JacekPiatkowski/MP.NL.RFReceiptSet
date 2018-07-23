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

		public RFTools_Forms_frmEmpty()
		{
		}

		public void Initialize(RFForm args)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			if (!(args is Forms.frmEmpty))
				return;
			m_EmptyForm = (Forms.frmEmpty)args;
			/*only if current form i Set27IAUpdate.xml*/
			if (m_EmptyForm.FormFile != null && string.Compare(m_EmptyForm.FormFile.ToString().ToLower(invariantCulture), "Set27IAUpdate.xml".ToLower(invariantCulture), false) == 0)
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


		public RFProduct_frmGetProduct()
		{
		}

		public void txtProduct_Validating(object sender, OnValidationEventArgs e)
		{
			string IA_SetID, IA_SetValue, strSQL, RFFolder, XMLFormPath,ProductCode ;
			CMenuTools RFtools = new CMenuTools();
				Mantis.LVision.DBAccess.DbRoutines.Routines dbr = new Mantis.LVision.DBAccess.DbRoutines.Routines();
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
							/*only if IA_SetValue is equal ? then ask user for change*/
							if (IA_SetValue == null)
							{
								/* RFmsg = "Produtk obecnie ma ustawiony atrybut SET na: " + IA_SetValue.ToString();
								RF_Message(RFmsg);*/
								/*Setup item hierarhy and IA*/
								strSQL = "exec usp_SetItemSetup  @prdCode='" + ProductCode + "';";
								int execReturnedValue = dbr.Execute(strSQL, con, null, m_Form);
							}
							
							/*set xml form path*/
							XMLFormPath = RFFolder + "\\SET\\Set26IASet.xml";
							/*call RF form from XML*/
							CMenuTools.ShowFormFile(m_Form, XMLFormPath);

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
				//this.m_bReadOnlyMultiplier = true;
				ReceiptCode = m_Parent.txtReceiptCode.Value.ToString();
				this.m_Form.txtProduct.Validating += new OnValidationEventHandler(this.txtProduct_Validating);
				//this.m_Form.txtMultiplier.Enter += new OnEnterEventHandler(this.txtMultiplier_Enter);
			}
		}

		/*NL oryginal dll content*/
		//public void txtMultiplier_Enter(object sender, OnEnterEventArgs e)
		//{
		//	if (!this.m_bReadOnlyMultiplier)
		//		return;
		//	this.m_Form.txtMultiplier.ReadOnly = true;
		//}





		public void Close(RFForm args)
		{
		}
	}
}
