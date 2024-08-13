using CustomerDLL.DAO;
using CustomerDLL.DTO;
using DevExpress.XtraEditors.Repository;
using ERPMaster.UI.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERPMaster.UI.Warehouse.FPBillGoods
{
    public partial class fmFPBillRequestExportCus : fmOriginal
    {
        CustomerDAO _CustomerDAO = new CustomerDAO();

        public fmFPBillRequestExportCus()
        {
            InitializeComponent();
        }



        void LoadCustomer()
        {
            var cus = _CustomerDAO.LoadCustomers();
            cboCustomer.DataSource = cus;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "Id";
            this.cboCustomer.SelectedIndexChanged += CboCustomer_SelectedIndexChanged;


        }


        private void CboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cus = (Customer)cboCustomer.SelectedItem;

            txtCompanyName.Text = cus.CompanyName;
            txtAddress.Text = cus.Address;
            txtOPContact.Text = cus.OpContact;


            string cusId = cboCustomer.SelectedValue.ToString();
            var model = _CustomerDAO.GetLsModelByCusId(cusId);

            var modelFormat = (from r in model
                               select new
                               {
                                   ModelID = r.ModelID,
                               }).ToList();
            //repositoryItemLookUpEdit1.DataSource = modelFormat;
            //repositoryItemLookUpEdit1.ValueMember = "ModelID";
            //repositoryItemLookUpEdit1.DisplayMember = "ModelID";
        }

        private void btnAddRow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView1.AddNewRow();

        }

        private void btnDeleteRow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView1.DeleteRow(gridView1.FocusedRowHandle);

        }
    }
}
