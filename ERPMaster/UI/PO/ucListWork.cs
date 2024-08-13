using CustomerDLL.DAO;
using CustomerDLL.DTO;
using DevExpress.Utils.DirectXPaint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarehouseDll.DTO;

namespace ERPMaster.UI.PO
{
    public partial class ucListWork : UserControl
    {
        public delegate void selectFunctionWork(int Action, WorkOrder workOrder, Customer cus, ModelChild modelChild );
        public event selectFunctionWork SelectFunctionWork;

        CustomerDAO _CustomerDAO = new CustomerDAO();
        readonly string _UserId;
        public ucListWork(string userId)
        {
            InitializeComponent();
            this.Load += UcListWork_Load;
            MyLoadData();
            _UserId = userId;
        }

        private void UcListWork_Load(object sender, EventArgs e)
        {
            
        }


        void MyLoadData()
        {
            var s = new CycleReport().Cycle();
            cboDate.DataSource = s;
            this.cboDate.SelectedIndexChanged += CboDate_SelectedIndexChanged; ;

            cboDate.SelectedItem = CycleReport.TAT_CA;

            var cus = _CustomerDAO.LoadCustomers();
            cboCus.DataSource = cus;
            cboCus.DisplayMember = "Name";
            cboCus.ValueMember = "Id";
            this.cboCus.SelectedIndexChanged += new System.EventHandler(this.cboCus_SelectedIndexChanged);
        }
        void LoadListWork(string model, DateTime dateStart, DateTime dateEnd)
        {
            var workOrders = _CustomerDAO.GetLsWorkByModelId(model, dateStart, dateEnd);
            dgvView.DataSource = workOrders;


        }
        private void CboDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            string model = string.Empty;
            var Model = (ModelChild)cboModel.SelectedItem;
            if(Model != null && !string.IsNullOrEmpty( Model.ModelID))
            {
                model = cboModel.SelectedValue.ToString();

            }
            var fillDate = new CycleReport().FillComboboxByCyle(cboDate);
            dtFrom.Value = fillDate.StartTime;
            dtTo.Value = fillDate.EndTime;


            LoadListWork(model, fillDate.StartTime, fillDate.EndTime);
        }

        private void cboCus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cusId = cboCus.SelectedValue.ToString();
            var model = _CustomerDAO.GetLsModelByCusId(cusId);
            var modelChild = model.SelectMany(x => x.ModelChilds).ToList();
            var modelChile2 = modelChild.SelectMany(x => x.ModelChilds).ToList();
            var allModel = modelChild.Union(modelChile2).OrderBy(x => x.ModelID).ToList();
            cboModel.DataSource = allModel;
            cboModel.DisplayMember = "ModelID";
            cboModel.ValueMember = "ModelID";

            cboModel.SelectedIndexChanged += CboModel_SelectedIndexChanged;
        }

        private void CboModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            string modelId = cboModel.SelectedValue.ToString();
            var fillDate = new CycleReport().FillComboboxByCyle(cboDate);
            dtFrom.Value = fillDate.StartTime;
            dtTo.Value = fillDate.EndTime;


            LoadListWork(modelId, fillDate.StartTime, fillDate.EndTime);
        }


        

        private void btnView_Click(object sender, EventArgs e)
        {
         
            var workorder = gridView1.GetRow(gridView1.FocusedRowHandle) as WorkOrder;
            var customer = (Customer)cboCus.SelectedItem;
            var model = (ModelChild)cboModel.SelectedItem;
            fmWork fm = new fmWork(LoadActionWorkOrder.UPDATE, workorder, model, customer, _UserId);
            fm.ShowDialog();
        }

        private void btnCreat_Click(object sender, EventArgs e)
        {
            var customer = (Customer)cboCus.SelectedItem;
            var model = (ModelChild)cboModel.SelectedItem;

            if (customer == null || string.IsNullOrEmpty(customer.Name) || model == null || string.IsNullOrEmpty(model.ModelID))
            {
                MessageBox.Show("Chọn khách hàng và model trước khi thêm mới");
                return;
            }
            fmWork fm = new fmWork(LoadActionWorkOrder.CREATE, null, model, customer, _UserId);
            fm.ShowDialog();
        }

        private void panelEx5_Click(object sender, EventArgs e)
        {

        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
        }
    }
}
