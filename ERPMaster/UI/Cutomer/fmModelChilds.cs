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

namespace ERPMaster.UI.Cutomer
{
    public partial class fmModelChilds : fmOriginal
    {
        CustomerDAO _CustomerDAO = new CustomerDAO();
        CustomerBUS _CustomerBUS = new CustomerBUS();
        Model _Model = new Model();
        readonly int _FunctionID = 0;
        readonly string _ModelParent;
        public fmModelChilds(Model model, string modelParent, int functionID)
        {
            InitializeComponent();
            _Model = model;
            _FunctionID = functionID;
            _ModelParent = modelParent;
            this.Load += FmModelChilds_Load;
        }
        void InitializeModelChild()
        {

            cboProcess.DataSource = LoadMyProcess.myProcesses;
            cboProcess.DisplayMember = "Name";
            cboProcess.ValueMember = "Name";


        }
        private void FmModelChilds_Load(object sender, EventArgs e)
        {
            InitializeModelChild();
            if (_FunctionID == LoadActionModel.CREATE)
            {
                txtModelParent.Text = _ModelParent;
                txtCusModel.Text = _Model.CusModel;
                txtModelChild.Text = _Model.ModelID;
            }
            else if (_FunctionID == LoadActionModel.UPDATE)
            {
                LoadInformationModel(_Model.ModelChilds, _ModelParent);
            }
        }

        void LoadInformationModel(List<ModelChild> modelChildren, string model)
        {
            foreach (var item in modelChildren)
            {
                if (item.ModelID == model)
                {
                    txtModelParent.Text = item.ModelParent;
                    txtModelChild.Text = model;
                    txtModelChild.ReadOnly = true;
                    cboProcess.SelectedValue = item.Process;
                    txtCusModel.Text = item.CusModel;
                    txtGerber.Text = item.Gerber;
                    txtVerBom.Text = item.VerBom;
                    nbPcsOnPanel.Value = item.PcbOnPanel;
                    break;
                }
                else
                {
                    LoadInformationModel(item.ModelChilds, model);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string productLine = _Model.ModelID;
            string modelChild = txtModelChild.Text.Trim();
            string cusModel = txtCusModel.Text.Trim();
            string process = cboProcess.Text.Trim().ToString();
            string gerber = txtGerber.Text.Trim();
            string bom = txtVerBom.Text.Trim();
            string cusid = _Model.CusID;
            int pcsOnPanel = (int)nbPcsOnPanel.Value;
            if (!modelChild.Contains(cusid))
            {
                MessageBox.Show("Thông tin Model mới phải bắt đầu bằng mã khách hàng");
                return;
            }
            if (modelChild.Count() != 10)
            {
                MessageBox.Show("Sai định dạng, mã Model phải có số ký tự bằng 10");
                return;
            }
            if (string.IsNullOrEmpty(modelChild) || string.IsNullOrEmpty(cusModel) || string.IsNullOrEmpty(process) ||
                string.IsNullOrEmpty(gerber) || string.IsNullOrEmpty(bom))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin");
                return;
            }
            if (_FunctionID == LoadActionModel.CREATE)
            {
                if (_CustomerDAO.IsModelChildExist(modelChild))
                {
                    MessageBox.Show("Lỗi ! Model này đã tồn tại ");
                    return;
                }
            }
            if (_CustomerBUS.SaveModelChild(modelChild, cusModel, process, cusid, pcsOnPanel, productLine, _ModelParent, gerber, bom))
            {
                MessageBox.Show("Pass");
                this.Close();

            }
            else
            {
                MessageBox.Show("Fail");
            }
        }
    }
}
