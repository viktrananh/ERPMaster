using CustomerDLL.BUS;
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

namespace ERPMaster.UI.PO
{
    public partial class fmWork : fmOriginal
    {
        ProjectDAO _ProjectDAO = new ProjectDAO();
        ProjectBUS _ProjectBUS = new ProjectBUS();

        int _ActionId = 0;
        WorkOrder _WorkOrder = new WorkOrder();
        ModelChild _ModelChild = new ModelChild();
        Customer _Customer = new Customer();
        readonly string _UserId;
        public fmWork(int actionId, WorkOrder workOrder, ModelChild modelChild, Customer customer, string userId)
        {
            InitializeComponent();
            _ActionId = actionId;
            _WorkOrder = workOrder;
            _ModelChild = modelChild;
            _Customer = customer;
            _UserId = userId;
            this.Load += FmWork_Load;
            this.btnSave.Click += BtnSave_Click;
        }

        private void FmWork_Load(object sender, EventArgs e)
        {
            MyLoadForm();
        }

        void MyLoadForm()
        {
            cboBOM.DataSource = _ProjectDAO.GetBomversion(_ModelChild.ModelID);
            cboBOM.DisplayMember = "VERSION";
            cboBOM.ValueMember = "VERSION";

            cboPO.DataSource = _ProjectDAO.GetPOByModelLine(_ModelChild.ModelLine);
            cboPO.DisplayMember = "PO";
            cboPO.ValueMember = "PO";
            txtCusName.Text = _Customer.Name;
            txtModelId.Text = _ModelChild.ModelID;

            btnSave.Enabled = true;
            if (_ActionId == LoadActionWorkOrder.CREATE)
            {
                LoadUICreat();
            }
            else
            {

                LoadUIUpdate();
            }
            BindingDataWork(_WorkOrder);
        }
        void LoadUICreat()
        {
            txtWorkID.Clear();
            txtWorkParent.Clear();
            chkRMA.Checked = chkSanple.Checked = chkXOut.Checked = false;
            nmrQty.Value = nmrTem.Value = 1;
            nmrXout.Value = 0;
            chkRMA.Enabled = true;
            chkSanple.Enabled = true;
            txtWorkID.Enabled = true;
            nmrQty.Enabled = true;
            nmrTem.Enabled = true;
            txtWorkParent.Enabled = true;
            cboPO.Enabled = true;
            chkXOut.Enabled = true;
            nmrXout.Enabled = true;
        }
        void LoadUIUpdate()
        {


            chkRMA.Enabled = false;
            chkSanple.Enabled = false;
            txtWorkID.Enabled = false;
            nmrQty.Enabled = false;
            nmrTem.Enabled = true;
            txtWorkParent.Enabled = true;
            cboPO.Enabled = false;
            chkXOut.Enabled = false;
            nmrXout.Enabled = _WorkOrder.IsXout == 1 ? true : false;

            btnDelete.Enabled = true;
            btnClose.Enabled = true;
        }

        void BindingDataWork(object context)
        {
            if (context != null)
            {
                txtModelId.DataBindings.Clear();

                txtWorkID.DataBindings.Clear();
                txtWorkParent.DataBindings.Clear();

                cboPO.DataBindings.Clear();
                nmrQty.DataBindings.Clear();
                nmrTem.DataBindings.Clear();
                chkRMA.DataBindings.Clear();
                chkSanple.DataBindings.Clear();
                chkXOut.DataBindings.Clear();
                dtMfg.DataBindings.Clear();
                dtFirst.DataBindings.Clear();
                dtLast.DataBindings.Clear();
                nbMonthFinishPP.DataBindings.Clear();
                cboBOM.DataBindings.Clear();
                txtComment.DataBindings.Clear();
                nmrXout.DataBindings.Clear();

                txtModelId.DataBindings.Add(new Binding("Text", context, "ModelWork", true, DataSourceUpdateMode.OnPropertyChanged));

                txtWorkID.DataBindings.Add(new Binding("Text", context, "WorkID", true, DataSourceUpdateMode.OnPropertyChanged));
                txtWorkParent.DataBindings.Add(new Binding("Text", context, "WorkParent", true, DataSourceUpdateMode.OnPropertyChanged));
                cboPO.DataBindings.Add(new Binding("SelectedValue", context, "PO", true, DataSourceUpdateMode.OnPropertyChanged));
                nmrQty.DataBindings.Add(new Binding("Value", context, "TotalPCS", true, DataSourceUpdateMode.OnPropertyChanged));
                nmrTem.DataBindings.Add(new Binding("Value", context, "TempNumber", true, DataSourceUpdateMode.OnPropertyChanged));
                chkRMA.DataBindings.Add(new Binding("CheckValue", context, "IsRMA", true, DataSourceUpdateMode.OnPropertyChanged));
                chkSanple.DataBindings.Add(new Binding("CheckValue", context, "IsSample", true, DataSourceUpdateMode.OnPropertyChanged));
                chkXOut.DataBindings.Add(new Binding("CheckValue", context, "IsXout", true, DataSourceUpdateMode.OnPropertyChanged));

                dtMfg.DataBindings.Add(new Binding("Value", context, "MFGDate", true, DataSourceUpdateMode.OnPropertyChanged));
                dtFirst.DataBindings.Add(new Binding("Value", context, "FirstDate", true, DataSourceUpdateMode.OnPropertyChanged));
                dtLast.DataBindings.Add(new Binding("Value", context, "LastDate", true, DataSourceUpdateMode.OnPropertyChanged));
                nbMonthFinishPP.DataBindings.Add(new Binding("Value", context, "Month_Finish_PP", true, DataSourceUpdateMode.OnPropertyChanged));

                cboBOM.DataBindings.Add(new Binding("SelectedValue", context, "BomVersion", true, DataSourceUpdateMode.OnPropertyChanged));
                txtComment.DataBindings.Add(new Binding("Text", context, "Comment", true, DataSourceUpdateMode.OnPropertyChanged));
                nmrXout.DataBindings.Add(new Binding("Text", context, "PcbXO", true, DataSourceUpdateMode.OnPropertyChanged));



            }


        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_WorkOrder.TotalPCS % _ModelChild.PcbOnPanel != 0 && _WorkOrder.IsRMA != 1)
            {
                MessageBox.Show($" Vui lòng tạo số lượng sản xuất là Panel chẵn. Số dư pcb {_WorkOrder.TotalPCS} / {_ModelChild.PcbOnPanel} = {_WorkOrder.TotalPCS % _ModelChild.PcbOnPanel}");
                return;
            }
            if (_WorkOrder.IsXout == 1)
            {
                if (_WorkOrder.PcbXO < 1)
                {
                    MessageBox.Show($"Vui lòng nhập số lượng PCS XOut !");
                    return;
                }
            }
            if (_ActionId == LoadActionWorkOrder.CREATE)
            {
                if (_ModelChild.PTHOnly == 0 && _WorkOrder.IsRMA == 0)
                {
                    if (string.IsNullOrEmpty(_WorkOrder.PO))
                    {
                        MessageBox.Show("Vui lòng chọn PO");
                        return;
                    }
                    if (_ModelChild.Process != "SMT")
                    {
                        var workParrent = _ProjectDAO.GetWorkOrderById(_WorkOrder.WorkParent);

                        if (workParrent.Status == LoadStatusWorkOrder.CLOSE)
                        {
                            MessageBox.Show("Work mẹ đã đóng");
                            return;
                        }
                        if (workParrent.ModelWork != _ModelChild.ModelParent)
                        {
                            MessageBox.Show($"Model work mẹ {workParrent.ModelWork} khác không thể là model mẹ , model đúng {_ModelChild.ModelParent} ");
                            return;
                        }
                        var lsWorkChild = _ProjectDAO.GetLsWorkChildByWorkId(workParrent.WorkID);

                        var sumQtyWorkchild = 0;
                        string lswork = string.Empty;
                        if (!_ProjectDAO.IsListEmty(lsWorkChild))
                        {
                            sumQtyWorkchild = lsWorkChild.Sum(x => x.TotalPCS);
                            lswork = string.Join(",", lsWorkChild.Select(x => x.WorkID));
                        }
                        if (sumQtyWorkchild + _WorkOrder.TotalPCS > workParrent.TotalPCS)
                        {
                            MessageBox.Show($"Tổng số lượng Work con là {sumQtyWorkchild} lớn hơn số lượng work mẹ {workParrent.TotalPCS}, Work con {lswork} ");
                            return;
                        }
                    }
                }
            }
            else
            {
                if (_WorkOrder.Status == LoadStatusWorkOrder.CLOSE)
                {
                    MessageBox.Show("Work đã đóng");
                    return;
                }

                if (_ProjectBUS.UpdateWork(_WorkOrder, _UserId))
                {
                    MessageBox.Show("Work đã đóng");
                    return;
                }
                else
                {

                }
            }
        }
    }
}
