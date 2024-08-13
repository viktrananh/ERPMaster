using CustomerDLL.DTO;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevExpress.Data.Filtering;
using ERPMaster.UI.Cutomer;
using ERPMaster.UI.PO;
using ERPMaster.UI.Warehouse;
using ERPMaster.UI.Warehouse.FPBillGoods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarehouseDll.DTO.FinishedProduct;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ERPMaster.UI
{
    public partial class fmMain : Form
    {
        string _UserId;

        public fmMain()
        {
            InitializeComponent();
            this.Load += FmMain_Load;
        }

        private async void FmMain_Load(object sender, EventArgs e)
        {
            ucDesk uc = new ucDesk();
            await AddUserControl(uc, pnlMain);
        }

        private async void Uc_SelectedFunctionListCus(int ActionId, CustomerDLL.DTO.Customer customer)
        {

            if (ActionId == LoadActionDefineCustomer.CREATE)
            {
                //ucDefineCustomer uc = new ucDefineCustomer(LoadActionDefineCustomer.CREATE, null, _UserId);
                //await addControlTool(uc, $"CreatNewCus", $"Tạo mới khách hàng");
                //fmCustomer fm = new fmCustomer();
                //fm.ShowDialog();

            }
            if (ActionId == LoadActionDefineCustomer.LOAD_LIST_MODEL)
            {
                //ucListModel uc = new ucListModel(_UserId, customer);
                //await addControlTool(uc, $"ListModel-{customer.Id}", $"Danh sách sản phẩm - {customer.Id}");
                //uc.SelectedFunctionListModel += Uc_SelectedFunctionListModel;

            }
            else if (ActionId == LoadActionDefineCustomer.CREATE_NEW_MODEL)
            {
                //ucDefineModel uc = new ucDefineModel(LoadStatusDefineModel.CREATE, customer, "");
                //await addControlTool(uc, "CreatModel", "Tạo mới sản phẩm");
            }
        }


        //public async Task<bool> addControlTool(UserControl uc, string tabpageName, string tabpageText)
        //{
        //    Func<object, bool> myfunc = (object any) =>
        //    {
        //        tabFormControlMain.Invoke(new MethodInvoker(delegate
        //        {
        //            //sideNav1.SelectedItem = btnPOT;
        //            //bool TabpageExisted = false;
        //            foreach (TabFormItem tp in tabFormControlMain.Items)
        //            {
        //                if (tp.Name == tabpageName)
        //                {
        //                    //TabpageExisted = true;
        //                    //tabFormControlMain.SelectedTab = tp;
        //                    tabFormControlMain.Items.Remove(tp);
        //                    break;
        //                }
        //            }
        //            //if (!TabpageExisted)
        //            {
        //                TabFormItem tabpage = tabFormControlMain.CreateTab(tabpageText, tabpageName);
        //                uc.Dock = DockStyle.Fill;
        //                tabpage.Panel.Controls.Add(uc);
        //                tabFormControlMain.SelectedTab = tabpage;
        //            }
        //        }));
        //        return true;

        //    };
        //    Task<bool> task1 = new Task<bool>(myfunc, "");
        //    task1.Start();
        //    await task1;
        //    return task1.Result;
        //}
        public async Task<bool> AddUserControl(UserControl uc, Panel pnl)
        {
            Func<object, bool> myfunc = (object any) =>
            {
                pnl.Invoke(new MethodInvoker(delegate
                {

                    if (pnl.Controls.Contains(uc))
                    {
                        pnl.Controls.Remove(uc);
                    }
                    pnl.Controls.Clear();
                    pnl.Controls.Add(uc);
                    uc.Dock = DockStyle.Fill;
                    uc.BringToFront();

                }));
                return true;
            };
            Task<bool> task1 = new Task<bool>(myfunc, "");
            task1.Start();
            await task1;
            return task1.Result;

        }
        private async void btnPO_Click(object sender, EventArgs e)
        {
            ucListPO uc = new ucListPO();
            //await addControlTool(uc, "ListPOs", "Danh sách P.O");
            //uc.SelectFunctionPO += Uc_SelectFunctionPO;
        }

    
        private async void btnWork_Click(object sender, EventArgs e)
        {
            ucListWork uc = new ucListWork(_UserId);
            //await addControlTool(uc, "ListWork", "Danh sách công lệnh - Work");
            //uc.SelectFunctionWork += Uc_SelectFunctionWork;
        }

    

    
        private async void btnCustomer_Click(object sender, EventArgs e)
        {

            ucListCustomer uc = new ucListCustomer();
            await AddUserControl(uc, pnlMain);
            //await addControlTool(uc, $"ListCustomer", $"Danh sách khách hàng");
            uc.SelectedFunctionListCus += Uc_SelectedFunctionListCus;
        }

        private async void btnProject_Click(object sender, EventArgs e)
        {
            ucPOOrder uc = new ucPOOrder(_UserId);
            await AddUserControl(uc, pnlMain);
        }

        private async void buttonItem37_Click(object sender, EventArgs e)
        {
            ucListWork uc = new ucListWork(_UserId);
            await AddUserControl(uc, pnlMain);

        }

        private async void btnModelLine_Click(object sender, EventArgs e)
        {
            ucListModel uc = new ucListModel(_UserId);
            await AddUserControl(uc, pnlMain);

        }
        private async void btnWarehouse_Click(object sender, EventArgs e)
        {
            FPBillType FPBillType = new FPBillType()
            {
                Id = 6,
                Name = "Nhập hàng thành phẩm"

            };
            ucWarehouse uc = new ucWarehouse(FPBillType);
            await AddUserControl(uc, pnlMain);
        }

        private void buttonItem34_Click(object sender, EventArgs e)
        {

        }

        private void metroTileItem1_Click(object sender, EventArgs e)
        {

        }
    }
}
