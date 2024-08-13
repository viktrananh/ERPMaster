using CustomerDLL.DAO;
using CustomerDLL.DTO;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
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
using WarehouseDll.DAO.FinishedProduct;
using WarehouseDll.DTO;
using WarehouseDll.DTO.FinishedProduct;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ERPMaster.UI.Warehouse.FPBillGoods
{
    public partial class ucFPBillExportCus : UserControl
    {
        ProjectDAO _ProjectDAO = new ProjectDAO();
        readonly string _UserID = string.Empty;
        int _FunctionID = -1;

        FPBill _FPBill = new FPBill();
        FPBillExportDAO _FPBillExportDAO = new FPBillExportDAO();
        public ucFPBillExportCus(int functionId, string userId, FPBill fPBill)
        {
            InitializeComponent();
            _FunctionID = functionId;
            _UserID = userId;
            _FPBill = fPBill;

            this.Load += UcFPBillExportCus_Load;
        }

        private void UcFPBillExportCus_Load(object sender, EventArgs e)
        {
            InitializeBill();
        }

        void InitializeBill()
        {
            cbbShipping.SelectedIndex = 0;



            switch (_FunctionID)
            {
                case LoadFunctionBillExportGoodsToCus.CREATE:
                    LoadUICreate();
                    break;
                case LoadFunctionBillExportGoodsToCus.UPDATE:
                    LoadUIUpdate(_FPBill);
                    break;
            }
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = _FPBill.FPBillDetailS;
            dgvDetailBill.DataSource = bindingSource;
        }
        void LoadUICreate()
        {
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            txtWorkPlan.ReadOnly = false;
            txtRequests.ReadOnly = false;
            txtBillNumber.Clear();
            txtWorkPlan.Clear();
            txtCusCode.Clear();
            txtCusModel.Clear();
            txtRequests.Clear();
            txtNote.Clear();
            txtWorkPlan.Focus();
            txtModel.Clear();
            _FPBill = new FPBill();
            _FPBill.FPBillDetailS = new List<FPBillDetail>();

        }
        void LoadUIUpdate(FPBill Bill)
        {
            btnCancel.Enabled = true;
            btnPrint.Enabled = true;

        }
        private void btnRefesh_Click(object sender, EventArgs e)
        {
            InitializeBill();
        }
        private void txtWorkPlan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            string work = txtWorkPlan.Text;
            if (work == string.Empty) return;

            var workOrder = _ProjectDAO.GetWorkOrderById(work);
            if (workOrder == null || string.IsNullOrEmpty(workOrder.WorkID))
            {
                MessageBox.Show("Lỗi ! Không tìm thấy thông tin work !");
                return;
            }
            if (workOrder.Status == LoadStatusWorkOrder.CLOSE)
            {
                MessageBox.Show("Lỗi !Work đã đóng !");
                return;

            }
            string mes = "";
            if (!_ProjectDAO.CheckEcnRelate(work, out mes))
            {
                MessageBox.Show(mes);
                return;
            }

            if (_ProjectDAO.IsListEmty(_FPBill.FPBillDetailS))
            {
                _FPBill.CusId = workOrder.CusId;
            }
            else
            {
                if (_FPBill.CusId != workOrder.CusId)
                {
                    txtWorkPlan.Clear();
                    txtModel.Clear();
                    txtCusModel.Clear();
                    txtCusCode.Clear();
                    MessageBox.Show("Phiếu chỉ chứa thông tin một khách hàng !");
                    return;
                }
            }

            txtModel.Text = workOrder.ModelWork;
            txtCusModel.Text = workOrder.CusModel;
            txtCusCode.Text = workOrder.CusCode;
            txtRequests.Focus();
        }

        private void txtRequests_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            string work = txtWorkPlan.Text;
            string model = txtModel.Text;
            string cusModel = txtCusModel.Text;
            string cusCode = txtCusCode.Text;
            int request = int.Parse(txtRequests.Text);
            string note = txtNote.Text;

            var workOrder = _ProjectDAO.GetWorkOrderById(work);

            if (workOrder == null || string.IsNullOrEmpty(workOrder.WorkID))
            {
                MessageBox.Show("Work không đúng");
                return;

            }
            if (request > workOrder.PCS - workOrder.Exported)
            {
                MessageBox.Show($"Số lượng yêu cầu vượt mức, tổng work {workOrder.PCS}, đã xuất  {workOrder.Exported}, số lượng tối đa có thể xuất tiếp {workOrder.PCS - workOrder.Exported}");
                return;
            }
            string unit = "PCS";
            if (string.IsNullOrEmpty(work) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(txtRequests.Text) || string.IsNullOrEmpty(unit))
            {
                MessageBox.Show(" Thông tin work chưa chưa được nhập đủ");
                return;
            }


            if (_FPBill != null && !string.IsNullOrEmpty(_FPBill.CusId))
            {

                if (_FPBill.CusId != workOrder.CusId)
                {
                    MessageBox.Show("Không thể tạo phiếu cho 2 khách hàng");
                    return;
                }


            }
            var existWork = _FPBill.FPBillDetailS.Any(x => x.WorkId == workOrder.WorkID);

            if (existWork)
            {
                if (request == 0)
                {
                    _FPBill.FPBillDetailS.RemoveAll(x => x.WorkId == work);
                }
                else
                {
                    _FPBill.FPBillDetailS.Where(x => x.WorkId == work).FirstOrDefault().Request = request;
                }
            }
            else
            {

                _FPBill.FPBillDetailS.Add(new FPBillDetail()
                {
                    WorkId = workOrder.WorkID,
                    PO = workOrder.PO,
                    ModelId = workOrder.ModelWork,
                    CusCode = cusCode,
                    CusModel = cusModel,
                    Request = request,
                    Note = note,
                });


            }
            var dataGridView1 = new BindingList<FPBillDetail>(_FPBill.FPBillDetailS);
            dgvDetailBill.DataSource = dataGridView1;
            ClearDataDetail();
        }

        private void txtRequests_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }


        void ClearDataDetail()
        {
            foreach (System.Windows.Forms.TextBox item in groupPanel2.Controls.OfType<System.Windows.Forms.TextBox>())
            {
                item.Clear();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string timeExport = dtExport.Value.ToString("yyyy/MM/dd HH:mm:ss");
            string numberBill = _FunctionID == LoadFunctionBillExportGoodsToCus.CREATE ? _FPBillExportDAO.CreateFPBillExportCus(dtExport.Value) : txtBillNumber.Text;
        }
    }
}
