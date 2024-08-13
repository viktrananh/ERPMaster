using CustomerDLL.BUS;
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
using static DevExpress.XtraEditors.TextEdit;

namespace ERPMaster.UI.Cutomer
{
    public partial class fmCustomer : fmOriginal
    {
        readonly string _UserId;
        int _FuctionID = 0;
        Customer _Customer = new Customer();
        CustomerBUS _CustomerBUS = new CustomerBUS();

        public fmCustomer(int FuctionID, Customer customer, string userId ) 
        {
            InitializeComponent();
            _Customer = customer;
            _FuctionID = FuctionID; 
            this.Load += FmCustomer_Load;
            this.btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string cusName = txtCustomerName.Text.Trim();
            string cusID = txtCustomerID.Text.Trim();
            string address = string.IsNullOrEmpty(txtAdrress.Text.Trim()) ? "N/A" : txtAdrress.Text.Trim();

            string companyName = string.IsNullOrEmpty(txtCompanyName.Text.Trim()) ? "N/A" : txtCompanyName.Text.Trim();
            string information = string.IsNullOrEmpty(txtInformation.Text.Trim()) ? "N/A" : txtInformation.Text.Trim();
            string OpName = string.IsNullOrEmpty(txtNameOP.Text.Trim()) ? "N/A" : txtNameOP.Text.Trim();
            string position = string.IsNullOrEmpty(txtPosition.Text.Trim()) ? "N/A" : txtPosition.Text.Trim();
            string email = string.IsNullOrEmpty(txtEmail.Text.Trim()) ? "N/A" : txtEmail.Text.Trim();
            string phone = string.IsNullOrEmpty(txtPhone.Text.Trim()) ? "N/A" : txtPhone.Text.Trim();
            if (string.IsNullOrEmpty(cusName) || string.IsNullOrEmpty(cusID))
            {
                MessageBox.Show("Lỗi ! Không để trống thông tin khách hàng");
            }
            if (cusID.Length != 3)
            {
                MessageBox.Show("Lỗi ! Mã khách hàng phải là 3 kí tự");
                return;
            }
            if (_CustomerBUS.CreateCustomer(cusName, cusID, companyName, address, phone, OpName, email, information))
            {
                MessageBox.Show("Cập nhật dữ liệu khách hàng thành công  ");
            }
            else
            {
                MessageBox.Show("Fail ");

            }
        }

        private void FmCustomer_Load(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            btnRefresh.Enabled = true;

            switch (_FuctionID)
            {
                case LoadActionDefineCustomer.CREATE:
                    LoadCreatUI();
                    break;
                case LoadActionDefineCustomer.UPDATE:
                    txtCustomerName.Text = _Customer.Name;
                    txtCustomerID.Text = _Customer.Id;
                    txtAdrress.Text = string.IsNullOrEmpty(_Customer.Address) ? "NA" : _Customer.Address;
                    txtCompanyName.Text = string.IsNullOrEmpty(_Customer.CompanyName) ? "NA" : _Customer.CompanyName;
                    txtInformation.Text = string.IsNullOrEmpty(_Customer.Information) ? "NA" : _Customer.Information;
                    txtNameOP.Text = string.IsNullOrEmpty(_Customer.OpContact) ? "NA" : _Customer.OpContact;
                    txtPosition.Text = "NA";
                    txtEmail.Text = string.IsNullOrEmpty(_Customer.Email) ? "NA" : _Customer.Email;
                    txtPhone.Text = string.IsNullOrEmpty(_Customer.Phone) ? "NA" : _Customer.Phone;
                    LoadEditUI();
                    break;
            }
        }
        void LoadEditUI()
        {
            btnDelete.Enabled = true;
            btnLock.Enabled = true;
        }
        void LoadCreatUI()
        {
            btnDelete.Enabled = false;
            btnLock.Enabled = false;

          
        }

    }
}
