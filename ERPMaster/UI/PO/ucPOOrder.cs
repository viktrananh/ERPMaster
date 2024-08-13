using CustomerDLL.DAO;
using CustomerDLL.DTO;
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
    public partial class ucPOOrder : UserControl
    {

        List<Customer> _Customers = new List<Customer>();

        CustomerDAO _CustomerDAO = new CustomerDAO();
        List<Model> _Models = new List<Model>();
        readonly string _UserId;
        public ucPOOrder(string userId)
        {
            InitializeComponent();
            _UserId = userId;
            MyInitialze();
            this.Load += UcProject_Load;
        }

        private void UcProject_Load(object sender, EventArgs e)
        {
          
        }

        void MyInitialze()
        {

            _Customers = _CustomerDAO.LoadCustomers();
            cboCustomer.DataSource = _Customers;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "Id";

            var s = new CycleReport().Cycle();
            cboDate.DataSource = s;
            cboDate.SelectedItem = CycleReport.TAT_CA;

        }

      

        List<POOrder> _POOrders = new List<POOrder>();
        ProjectDAO _ProjectDAO = new ProjectDAO();



        void LoadPOs(string cusId, DateTime dateStart, DateTime dateEnd)
        {
            _POOrders = new List<POOrder>();
            object dataSourcePO = null;
            object context = null;
            _POOrders = _ProjectDAO.GetPOs(cusId, dateStart, dateEnd);
            dataSourcePO = _POOrders;
            context = _POOrders;
            dgvPOs.DataSource = dataSourcePO;
            UpdateBindings(context);
        }
        private void btnGetDataPO_Click(object sender, EventArgs e)
        {
            var customer = (Customer)cboCustomer.SelectedItem;
            var dateStart = dtFrom.Value;
            var dateEnd = dtTo.Value;
            LoadPOs(customer.Id, dateStart, dateEnd);
        }
        private void UpdateBindings(object context)
        {

          
            
        }

        private void btnCreat_Click(object sender, EventArgs e)
        {
            fmPOOrder fm = new fmPOOrder(_UserId, LoadActionPOOrder.CREATE, new POOrder());
            fm.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var po = gridView2.GetRow(gridView2.FocusedRowHandle) as POOrder;
            fmPOOrder fm = new fmPOOrder(_UserId, LoadActionPOOrder.UPDATE, po);
            fm.ShowDialog();
        }

        private void cboDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            var fillDate = new CycleReport().FillComboboxByCyle(cboDate);
            dtFrom.Value = fillDate.StartTime;
            dtTo.Value = fillDate.EndTime;
        }
    }
}
