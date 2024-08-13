using CustomerDLL.BUS;
using CustomerDLL.DAO;
using CustomerDLL.DTO;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.CodeDom;
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
    public partial class ucPOOrder_1 : UserControl
    {

        public delegate void SelectFunctionEditPO(POOrder POOrder);
        public event SelectFunctionEditPO _SelectFunctionEditPO;

        ProjectBUS _ProjectBUS = new ProjectBUS();
        ProjectDAO _ProjectDAO = new ProjectDAO();
        CustomerDAO _CustomerDAO = new CustomerDAO();

        POOrder _POOrder = new POOrder();
        private readonly int _ActionId;
        private string _UserId;

        public ucPOOrder_1(string userId, int actionId, POOrder pOOrder)
        {
            InitializeComponent();
            _UserId = userId;
            _ActionId = actionId;
            _POOrder = pOOrder;
            this.Load += UcPOOrder_Load;
        }

        private void UcPOOrder_Load(object sender, EventArgs e)
        {
            LoadCustomer();
            MyInitialize();
        }
        void MyInitialize()
        {
            if (_ActionId == 0)
            {
                LoadUICreat();
            }
            else
            {
                LoadUIUpdate();
            }
        }
        void LoadUICreat()
        {
            _POOrder = new POOrder();
            _POOrder.POOrderDetails = new List<POOrderDetail>();
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = _POOrder.POOrderDetails;

            dgvModels.DataSource = bindingSource;
            txtPO.Clear();
            txtPO.Focus();
            txtPO.Enabled = true;
            cboCustomer.Enabled = true;

            btnClose.Enabled = false;
            btnDelete.Enabled = false;

        }
        void LoadUIUpdate()
        {
            btnClose.Enabled = true;
            btnDelete.Enabled = true;
            txtPO.Text = _POOrder.PO;
            cboCustomer.SelectedValue = _POOrder.CusId;
            txtPO.Enabled = false;
            cboCustomer.Enabled = false;
            _POOrder = _ProjectDAO.GetPOOrderByPOName(_POOrder.PO);
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = _POOrder.POOrderDetails;
            dgvModels.DataSource = bindingSource;
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
            repositoryItemLookUpEdit1.DataSource = modelFormat;
            repositoryItemLookUpEdit1.ValueMember = "ModelID";
            repositoryItemLookUpEdit1.DisplayMember = "ModelID";


        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            string cusId = cboCustomer.SelectedValue.ToString().Trim();
            string po = txtPO.Text.Trim();

            string comment = txtComment.Text.Trim();


            var pOOrderDetails = _POOrder.POOrderDetails;
            if (txtPO.Enabled)
            {
                MessageBox.Show($"Vui lòng xác nhận P.O trước !");
                return;
            }
            if (_ActionId == 0)
            {
                if (_ProjectDAO.IsPOExist(po))
                {
                    MessageBox.Show($"PO {po} đã được tạo trước đó");
                    return;
                }

                bool checkMFG = pOOrderDetails.Any(x => x.MFGDate.Date <= DateTime.Now.Date);
                if (checkMFG)
                {
                    MessageBox.Show($"Ngày sản xuất không đúng tiêu chuẩn");
                    return;
                }
            }
            else
            {
                if (_ProjectDAO.IsPOApplyWork(po))
                {


                    var POOrderOld = _ProjectDAO.GetPOOrderByPOName(po);
                    var poDetailOld = POOrderOld.POOrderDetails;
                    bool checkQty = poDetailOld.Any(x => pOOrderDetails.Any(a => a.ModelId == x.ModelId && a.Count < x.Count));
                    if (checkQty)
                    {
                        var b = poDetailOld.Where(x => pOOrderDetails.Any(a => a.ModelId == x.ModelId && a.Count < x.Count)).ToList();
                        MessageBox.Show($"Không thể giảm số lượng PO đã khai báo công lệnh ");
                        return;


                    }
                  
                }

                int state = _POOrder.Status;
                if (state == 1)
                {
                    MessageBox.Show($"Không thể cập nhật PO đã đóng ");
                    return;
                }

            }
            if (_ProjectBUS.POUpdate(cusId, po, comment, _UserId, pOOrderDetails))
            {
                MessageBox.Show("Cập nhật P.O thành công");
                if (_SelectFunctionEditPO != null)
                {
                    var poOrder = _ProjectDAO.GetPOOrderByPOName(po);
                    _SelectFunctionEditPO(poOrder);
                }
            }
            else
            {
                MessageBox.Show("Cập nhật P.O Lỗi");

            }
            MyInitialize();

        }

        private void txtCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void txtPO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            string po = txtPO.Text.Trim();
            if (string.IsNullOrEmpty(po)) return;

            var POOrder = _ProjectDAO.GetPOOrderByPOName(po);
            if (POOrder.POOrderDetails != null && POOrder.POOrderDetails.Count() > 0)
            {
                MessageBox.Show("PO đã tồn tại!");
                return;

            }
            txtPO.Enabled = false;
        }

        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //if (e.HitInfo.InRow)
            //{
            GridView view = sender as GridView;
            view.FocusedRowHandle = e.HitInfo.RowHandle;

            popupMenu.ShowPopup(dgvModels.PointToScreen(new Point(e.Point.X, e.Point.Y)));
            //}

        }

        private void btnAddNewRow_ItemClick(object sender, ItemClickEventArgs e)
        {
            //(dgvModels.DataSource as BindingList<POOrderDetail>).AddNew();
            gridView1.AddNewRow();
        }

        private void btnRemoveRow_ItemClick(object sender, ItemClickEventArgs e)
        {
            gridView1.DeleteRow(gridView1.FocusedRowHandle);
        }

        private void gridView1_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            gridView1.SetRowCellValue(e.RowHandle, "MFGDate", DateTime.Now.AddDays(1));
        }
    }
}
