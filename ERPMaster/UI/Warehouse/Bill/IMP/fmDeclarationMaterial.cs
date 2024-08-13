using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace ERPMaster.UI.Warehouse.Bill.IMP
{
    public partial class fmDeclarationMaterial : ERPMaster.UI.Root.fmOriginal
    {

        string _PrinterName = string.Empty;
        public fmDeclarationMaterial()
        {
            InitializeComponent();
            MyInitialize();
            this.btnPrintSetup.Click += BtnPrintSetup_Click;
        }

        private void BtnPrintSetup_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            pd.PrinterSettings = new PrinterSettings();
            string currentPrintName = string.Empty;
            if (DialogResult.OK == pd.ShowDialog(this))
            {
                _PrinterName = pd.PrinterSettings.PrinterName;
            }
        }

        void MyInitialize()
        {
            btnPrintSetup.Enabled = true;
        }
    }
}
