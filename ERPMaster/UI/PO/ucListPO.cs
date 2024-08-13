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

namespace ERPMaster.UI.PO
{
    public partial class ucListPO : UserControl
    {
        public delegate void SelectFunctionOnPO(int action, POOrder POOrder);
        public event SelectFunctionOnPO SelectFunctionPO;

        List<POOrder> _POOrders = new List<POOrder>();
        ProjectDAO _ProjectDAO = new ProjectDAO();
        public ucListPO()
        {
            InitializeComponent();
            this.Load += UcListPO_Load;
        }

        private void UcListPO_Load(object sender, EventArgs e)
        {
            LoadPOs();
        }
        void LoadPOs()
        {
            _POOrders = new List<POOrder>();
            object dataSourcePO = null;
            object context = null;
            //_POOrders = _ProjectDAO.GetPOs();
            dataSourcePO = _POOrders;
            context = _POOrders;
            dgvPOs.DataSource = dataSourcePO;
            UpdateBindings(context);
        }
        private void UpdateBindings(object context)
        {

            if (context != null)
            {
                lbPO.DataBindings.Clear();

                lbPO.DataBindings.Add(new Binding("Text", context, "PO"));

            }
            else
            {
                lbPO.Text = "";

            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPOs();
        }

        private void btncreate_Click(object sender, EventArgs e)
        {
            if (SelectFunctionPO != null)
            {
                var po = lbPO.Text.Trim();
                var POOrder = _POOrders.Where(x => x.PO == po).FirstOrDefault();
                SelectFunctionPO(LoadActionPOOrder.CREATE, null);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (SelectFunctionPO != null)
            {
                var POOrder = gridView1.GetRow(gridView1.FocusedRowHandle) as POOrder;

                SelectFunctionPO(LoadActionPOOrder.UPDATE, POOrder);
            }

        }
    }
}
