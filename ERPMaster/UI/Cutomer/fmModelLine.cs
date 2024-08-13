using CustomerDLL.BUS;
using CustomerDLL.DAO;
using CustomerDLL.DTO;
using DevComponents.DotNetBar;
using DevComponents.Tree;
using ERPMaster.MyStyte;
using ERPMaster.UI.Root;
using System;
using System.Collections;
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
    public partial class fmModelLine : fmOriginal
    {
        string[] _LeverPotential = new string[] { "S", "A", "B", "C", "D" };
        string[] _LeverPossibility = new string[] { "S", "A", "B", "C", "D" };
        string[] _LeverPiority = new string[] { "S", "A", "B", "C", "D" };
        int _FunctionID = 0;
        Model _Model = new Model();
        readonly Customer _Customer = new Customer();
        CustomerBUS _CustomerBUS = new CustomerBUS();
        CustomerDAO _CustomerDAO = new CustomerDAO();
        public fmModelLine(int functionId, Customer customer, Model model)
        {
            InitializeComponent();
            MyInitializeModel();
            _FunctionID = functionId;
            _Customer = customer;
            _Model = model;
            this.Load += FmModelLine_Load;
            this.btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string model = txtModel.Text.Trim();
            string cusModel = txtCusModel.Text.Trim();
            string cusID = _Customer.Id;
            string ger = txtGerber.Text.Trim();
            string bom = txtVerBom.Text.Trim();
            string potential = cboPotential.Text.Trim();
            string ppssi = cboPossibility.Text.Trim();
            string piority = cboPiority.Text.Trim();
            string opcontact = txtOpcontact.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (!model.Contains(cusID))
            {
                MessageBox.Show("Tên sản phẩm sai định dạng");
                txtModel.Focus();
                return;
            }
            if (_CustomerBUS.CreateModel(model, cusModel, cusID, ger, bom, opcontact, phone, potential, ppssi, piority, cbIATF.Checked))
            {
                MessageBox.Show("Pass");
                LoadUpdateView(model);

            }
            else
            {
                MessageBox.Show("Fail");

            }
        }

        private void FmModelLine_Load(object sender, EventArgs e)
        {
            txtCustomer.Text = _Customer.Name;
            txtCusID.Text = _Customer.Id;
            txtModel.Mask = $"AAA000";
            btnSave.Enabled = true;
            if (_FunctionID == LoadActionModel.CREATE)
            {

            }
            if (_FunctionID == LoadActionModel.UPDATE)
            {
                LoadUpdateView(_Model.ModelID);
            }
        }
        void LoadUpdateView(string model)
        {
            _Model = _CustomerDAO.GetModelById(model);
            txtModel.Text = _Model.ModelID;
            txtCusModel.Text = _Model.CusModel;
            txtGerber.Text = _Model.Gerber;
            txtVerBom.Text = _Model.VerBom;
            txtOpcontact.Text = _Model.OPContact;
            txtPhone.Text = _Model.Phone;
            cbIATF.Checked = _Model.isIATF == 1 ? true : false;
            try
            {
                cboPotential.SelectedValue = _Model.Potential;
                cboPossibility.SelectedValue = _Model.Possibility;
                cboPiority.SelectedValue = _Model.Pioirity;
            }
            catch (Exception)
            {


            }

            //_Model = ModelControl.LoadDetailModel("VLS004");
            LoadMainsModel(_Model);
        }
        void MyInitializeModel()
        {
            cboPotential.DataSource = _LeverPotential;
            cboPossibility.DataSource = _LeverPossibility;
            cboPiority.DataSource = _LeverPiority;

        }

        private Hashtable m_EnumeratedTypes = null;
        private void LoadMainsModel(Model model)
        {
            m_EnumeratedTypes = new Hashtable();
            treeGX1.BeginUpdate();
            treeGX1.Nodes.Clear();
            try
            {
                DevComponents.Tree.Node node = new DevComponents.Tree.Node();
                node.Text = model.ModelID;
                node.Expanded = true;
                //ctmnNode.Tag = model.ModelID;
                //node.ContextMenu = ctmnNode;

                node.Image = new Bitmap(@".\Image\Main.png");
                //m_EnumeratedTypes.Add(GetTypeName(rootType), "");
                treeGX1.Nodes.Add(node);
                List<ModelChild> modelChilds = model.ModelChilds;
                LoadModelChilds(modelChilds, node);
            }
            finally
            {
                treeGX1.EndUpdate();
            }
            m_EnumeratedTypes.Clear();
        }
        private void LoadModelChilds(List<ModelChild> modelChilds, DevComponents.Tree.Node parentNode)
        {
            if (_CustomerDAO.IsListEmty(modelChilds)) return;


            // Load Classes first
            foreach (ModelChild child in modelChilds)
            {
                string EOL = child.EOL;
                DevComponents.Tree.Node node = new DevComponents.Tree.Node();
                node.Text = child.ModelID;
                node.Expanded = true;
                if (EOL == "1")
                {
                    node.RenderMode = DevComponents.Tree.eNodeRenderMode.Custom;
                    // Assign renderer, renderers can be reused i.e. assigned to more than one node
                    RedNodeRenderer renderer = new RedNodeRenderer();
                    node.NodeRenderer = renderer;
                }
                //ctmnNode.Tag = child.ModelID;
                //node.ContextMenu = ctmnNode;
                parentNode.Nodes.Add(node);

                List<ModelChild> ModelChilds = child.ModelChilds;
                LoadModelChilds(ModelChilds, node);
            }
        }

        private void btnAddnewModelChild_Click(object sender, EventArgs e)
        {
            string model = InNodeContextMenu.Tag.ToString();
            if (model != _Model.ModelID)
            {

            }
            fmModelChilds f = new fmModelChilds(_Model, model, LoadActionModel.CREATE);
            f.ShowDialog();
            _Model = _CustomerDAO.GetModelById(_Model.ModelID);
            LoadMainsModel(_Model);
        }

        private void btnEditModelChild_Click(object sender, EventArgs e)
        {
            string model = InNodeContextMenu.Tag.ToString();
            if (model == _Model.ModelID) return;
            var ModelChilds = _Model.ModelChilds;

            fmModelChilds f = new fmModelChilds(_Model, model, LoadActionModel.UPDATE);
            f.ShowDialog();
            _Model = _CustomerDAO.GetModelById(_Model.ModelID);
            LoadMainsModel(_Model);
        }

        private void bntLockModelChild_Click(object sender, EventArgs e)
        {
            string model = InNodeContextMenu.Tag.ToString();
            if (model == _Model.ModelID) return;
            if (_CustomerBUS.LockModel(_Model.ModelChilds, model))
            {
                MessageBox.Show("Lock Pass");
            }
            else
            {
                MessageBox.Show("Lock Fail");
            }
            _Model = _CustomerDAO.GetModelById(_Model.ModelID);

            LoadUpdateView(_Model.ModelID);
        }

        private void btnUnblock_Click(object sender, EventArgs e)
        {
            string model = InNodeContextMenu.Tag.ToString();
            if (model == _Model.ModelID) return;
            if (_CustomerBUS.UnLockModel(_Model.ModelChilds, model))
            {
                MessageBox.Show("UnLock Pass");
            }
            else
            {
                MessageBox.Show("UnLock Fail");
            }
            _Model = _CustomerDAO.GetModelById(_Model.ModelID);
            LoadUpdateView(_Model.ModelID);
        }

        private void btnDeleteModelChilds_Click(object sender, EventArgs e)
        {
            string model = InNodeContextMenu.Tag.ToString();
            if (model == _Model.ModelID) return;
            if (_CustomerBUS.DeleteModel(_Model.ModelChilds, model))
            {
                MessageBox.Show("Delete Pass");

            }
            else
            {
                MessageBox.Show("Delete Fail");

            }
            _Model = _CustomerDAO.GetModelById(_Model.ModelID);
            LoadUpdateView(_Model.ModelID);
        }

        private void treeGX1_NodeMouseUp(object sender, DevComponents.Tree.TreeGXNodeMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeGX tree = sender as TreeGX;
                DevComponents.Tree.Node node = tree.SelectedNode;
                string model = node.Text;
                InNodeContextMenu.Tag = model;
                ShowContextMenu(InNodeContextMenu);
            }
        }
        private void ShowContextMenu(ButtonItem cm)
        {
            cm.Popup(MousePosition);
        }
    }
}
