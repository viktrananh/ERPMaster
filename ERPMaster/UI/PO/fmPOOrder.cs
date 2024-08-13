using CustomerDLL.BUS;
using CustomerDLL.DAO;
using CustomerDLL.DTO;
using DevExpress.XtraGrid.Views.Grid;
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
using static ERPMaster.UI.PO.ucPOOrder_1;

namespace ERPMaster.UI.PO
{
    public partial class fmPOOrder : fmOriginal
    {
        ProjectBUS _ProjectBUS = new ProjectBUS();
        ProjectDAO _ProjectDAO = new ProjectDAO();
        CustomerDAO _CustomerDAO = new CustomerDAO();

        POOrder _POOrder = new POOrder();
        private readonly int _ActionId;
        private string _UserId;
        public fmPOOrder(string userId, int actionId, POOrder pOOrder)
        {
            InitializeComponent();
            _UserId = userId;
            _ActionId = actionId;
            _POOrder = pOOrder;

            this.Load += FmPOOrder_Load;
           
            btnSave.Click += BtnSave_Click;
        }


        private void FmPOOrder_Load(object sender, EventArgs e)
        {
            LoadCustomer();
            MyInitialize();
        }
        void MyInitialize()
        {
            btnSave.Enabled = true;
            
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

        private void BtnSave_Click(object sender, EventArgs e)
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
               
            }
            else
            {
                MessageBox.Show("Cập nhật P.O Lỗi");

            }
            MyInitialize();
        }

        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            GridView view = sender as GridView;
            view.FocusedRowHandle = e.HitInfo.RowHandle;

            popupMenu.ShowPopup(dgvModels.PointToScreen(new Point(e.Point.X, e.Point.Y)));
        }
    }
}
