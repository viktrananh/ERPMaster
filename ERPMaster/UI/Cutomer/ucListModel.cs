using CustomerDLL.DAO;
using CustomerDLL.DTO;
using DevComponents.DotNetBar.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarehouseDll.DTO;

namespace ERPMaster.UI.Cutomer
{
    public partial class ucListModel : UserControl
    {
        public delegate void SelectFunctionLsModel(int ActionId, Customer customer, Model Model);
        public event SelectFunctionLsModel SelectedFunctionListModel;

        List<Customer> _Customers = new List<Customer>();

        CustomerDAO _CustomerDAO = new CustomerDAO();
        List<Model> _Models = new List<Model>();

        string _UserId;
        public ucListModel(string userId)
        {
            InitializeComponent();
            _UserId = userId;
            LoadModels();
            this.btnGetData.Click += BtnGetData_Click;
        }

        private void BtnGetData_Click(object sender, EventArgs e)
        {

            LoadData();
        }
        void LoadData()
        {
            object context = null;
            var Customer = (Customer)cboCustomer.SelectedItem;
            _Models = _CustomerDAO.GetLsModelByCusId(Customer.Id);
            context = _Models;
            Binding(context);
            dgvModels.DataSource = _Models;
        }
        private void UcListModel_Load(object sender, EventArgs e)
        {

           
        }
        void LoadModels()
        {

            _Customers = _CustomerDAO.LoadCustomers();
            cboCustomer.DataSource = _Customers;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "Id";
            var s = new CycleReport().Cycle();
            cboDate.DataSource = s;
            cboDate.SelectedItem = CycleReport.TAT_CA;
        }
     

        void Binding(object context)
        {
            if (context != null)
            {
                lbCusId.DataBindings.Clear();
                lbModelId.DataBindings.Clear();
                lbCusModel.DataBindings.Clear();

                lbModelId.DataBindings.Add(new Binding("Text", context, "ModelID"));
                lbCusModel.DataBindings.Add(new Binding("Text", context, "CusModel"));
                lbCusId.DataBindings.Add(new Binding("Text", context, "CusId"));

            }
            else
            {
                //lbModel.Text = "";
            }
        }

        private void btnCreat_Click(object sender, EventArgs e)
        {
            var Customer = (Customer)cboCustomer.SelectedItem;

            fmModelLine fm = new fmModelLine(LoadActionModel.CREATE, Customer, new Model());
            fm.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var Customer = (Customer)cboCustomer.SelectedItem;
            var Model = gridView1.GetRow(gridView1.FocusedRowHandle) as Model;
            if (Model.CusID != Customer.Id) return;

            fmModelLine fm = new fmModelLine(LoadActionModel.UPDATE, Customer, Model);
            fm.ShowDialog();
        }
    }
}
