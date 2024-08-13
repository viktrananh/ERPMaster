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
using WarehouseDll.DAO;
using WarehouseDll.DAO.FinishedProduct;
using WarehouseDll.DTO;
using WarehouseDll.DTO.FinishedProduct;

namespace ERPMaster.UI.Warehouse.FPBillGoods
{
    public partial class ucWarehouse : UserControl
    {
        List<FPBill> _FPBills = new List<FPBill>();
        FPBillExportDAO _FBBillExportDAO = new FPBillExportDAO();
        FPBillType _FPBillType = new FPBillType();
        public ucWarehouse(FPBillType fPBillType)
        {
            InitializeComponent();
            _FPBillType = fPBillType;
            LoadComboBox();
        }

        void LoadComboBox()
        {
            var typeBill = new BaseDAO().GetFPBillTypes();
            cboTypeBill.DataSource = typeBill;
            cboTypeBill.DisplayMember = "Name";
            cboTypeBill.ValueMember = "Id";

            cboTypeBill.SelectedItem = _FPBillType;
            var s = new CycleReport().Cycle();
            cboDate.DataSource = s;
            this.cboDate.SelectedIndexChanged += CboDate_SelectedIndexChanged;

            cboDate.SelectedItem = CycleReport.THANG_NAY;
        }
        void LoadLsFPBill()
        {
            _FPBillType = (FPBillType)cboTypeBill.SelectedItem;

            _FPBills = _FBBillExportDAO.GetAllFBBill(_FPBillType.Id, dtFrom.Value, dtTo.Value);

            var dataGridView1 = new BindingList<FPBill>(_FPBills);
            dgvlsBill.DataSource = dataGridView1;
        }
        private void CboDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            var fillDate = new CycleReport().FillComboboxByCyle(cboDate);
            dtFrom.Value = fillDate.StartTime;
            dtTo.Value = fillDate.EndTime;

        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            if (e.Column.AppearanceHeader.BackColor == Color.Empty)
            {
                e.Column.AppearanceHeader.BackColor = Color.FromArgb(186, 216, 255);
                e.Info.AllowColoring = true;
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            LoadLsFPBill();
        }

        private void btnCreat_Click(object sender, EventArgs e)
        {

        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
           
        }

        private void dgvlsBill_Click(object sender, EventArgs e)
        {
            var bill = gridView1.GetRow(gridView1.FocusedRowHandle) as FPBill;

            var fPBill = _FBBillExportDAO.GetFBBillByBillId(bill.BillNumber);

            var DetailBill = fPBill.FPBillDetailS;

            dgvDetailBill.DataSource = DetailBill;

        }

        private void gridView2_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            if (e.Column.AppearanceHeader.BackColor == Color.Empty)
            {
                e.Column.AppearanceHeader.BackColor = Color.FromArgb(186, 216, 255);
                e.Info.AllowColoring = true;
            }
        }
    }
}
