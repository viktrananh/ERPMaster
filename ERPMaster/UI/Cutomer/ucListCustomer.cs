using CustomerDLL.DAO;
using CustomerDLL.DTO;
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

namespace ERPMaster.UI.Cutomer
{
    public partial class ucListCustomer : UserControl
    {
        public delegate void SelectFunction(int ActionId, Customer customer);
        public event SelectFunction SelectedFunctionListCus;


        List<Customer> _Customers = new List<Customer>();
        CustomerDAO _CustomerDAO = new CustomerDAO();
        readonly string _UserId;

        public ucListCustomer()
        {
            InitializeComponent();
            InitializeCus();
        }

     

        void InitializeCus()
        {
            object dataSourceBomDetail = null;
            object context = null;
            _Customers = _CustomerDAO.LoadCustomers();
            dataSourceBomDetail = _Customers;
            dgvCustomer.DataSource = dataSourceBomDetail;

            context = _Customers;
            Binding(context);
        }

        void Binding(object context)
        {
            if (context != null)
            {
                lbCusId.DataBindings.Clear();
                lbCusName.DataBindings.Clear();
                lbAddress.DataBindings.Clear();
                lbTaxCode.DataBindings.Clear();

                lbCusId.DataBindings.Add(new Binding("Text", context, "Id"));
                lbCusName.DataBindings.Add(new Binding("Text", context, "Name"));
                lbAddress.DataBindings.Add(new Binding("Text", context, "Address"));
                lbTaxCode.DataBindings.Add(new Binding("Text", context, "TaxCode"));

            }
            else
            {
                //lbModel.Text = "";
            }
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            InitializeCus();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var customer = gridView1.GetRow(gridView1.FocusedRowHandle) as Customer;

            fmCustomer fm = new fmCustomer(LoadActionDefineCustomer.UPDATE, customer, _UserId);
            fm.ShowDialog();
        }

        private void btncreate_Click(object sender, EventArgs e)
        {
           

            if (SelectedFunctionListCus != null)
            {
                SelectedFunctionListCus(LoadActionDefineCustomer.CREATE, null);
            }
        }

        private void btnListModel_Click(object sender, EventArgs e)
        {
            if (SelectedFunctionListCus != null)
            {
                var customer = gridView1.GetRow(gridView1.FocusedRowHandle) as Customer;

                SelectedFunctionListCus(LoadActionDefineCustomer.LOAD_LIST_MODEL, customer);
            }
        }

        private void btnNewModel_Click(object sender, EventArgs e)
        {
            if (SelectedFunctionListCus != null)
            {
                var customer = gridView1.GetRow(gridView1.FocusedRowHandle) as Customer;

                SelectedFunctionListCus(LoadActionDefineCustomer.CREATE_NEW_MODEL, customer);
            }
        }

        private void btnCreat_Click(object sender, EventArgs e)
        {
            fmCustomer fm = new fmCustomer(LoadActionDefineCustomer.CREATE, new Customer(), _UserId);
            fm.ShowDialog();

        }

       
    }
}
