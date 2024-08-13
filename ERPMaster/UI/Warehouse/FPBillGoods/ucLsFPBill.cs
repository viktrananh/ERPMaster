using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
    public partial class ucLsFPBill : UserControl
    {
        List<FPBill> _FPBills = new List<FPBill>();
        FPBillExportDAO _FBBillExportDAO = new FPBillExportDAO();
        FPBillType _FPBillType = new FPBillType();

        public ucLsFPBill(FPBillType fPBillType)
        {
            InitializeComponent();
            _FPBillType =   fPBillType;
            this.Load += UcFPBill_Load;
        }

        private void UcFPBill_Load(object sender, EventArgs e)
        {
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

        private void CboDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            var fillDate = new CycleReport().FillComboboxByCyle(cboDate);
            dtFrom.Value = fillDate.StartTime;
            dtTo.Value = fillDate.EndTime;

        }

        void LoadLsFPBill()
        {
            _FPBillType = (FPBillType)cboTypeBill.SelectedItem;

            _FPBills = _FBBillExportDAO.GetAllFBBill(_FPBillType.Id, dtFrom.Value, dtTo.Value);

            var dataGridView1 = new BindingList<FPBill>(_FPBills);
            dgvlsBill.DataSource = dataGridView1;
        }
        private void btnGetData_Click(object sender, EventArgs e)
        {
            LoadLsFPBill();
        }

        private void btnCreat_Click(object sender, EventArgs e)
        {
            _FPBillType = (FPBillType)cboTypeBill.SelectedItem;
            if(_FPBillType.Id == LoadTypeFPBill.EXP_CUS)
            {

            }
            else if(_FPBillType.Id == LoadTypeFPBill.IMP_LOCAL)
            {

            }
            else if(_FPBillType.Id == LoadTypeFPBill.EXP_LOCAL)
            {

            }
            else
            {

            }
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
    }
}
