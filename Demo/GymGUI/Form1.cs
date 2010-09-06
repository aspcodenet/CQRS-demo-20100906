using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GymGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = "%";
            string mno = "%";
            if (txtNamn.Text.Length > 0)
                name = txtNamn.Text + "%";
            if (txtMemberNo.Text.Length > 0)
                mno = txtMemberNo.Text + "%";

            string sql = @"select * from report_memberlist where membername like @membername and memberno like @memberno";

            var p = new List<System.Data.SqlClient.SqlParameter>
                        {
                            new System.Data.SqlClient.SqlParameter("@membername", name ),
                            new System.Data.SqlClient.SqlParameter("@memberno", mno )
                        }.ToArray();
            dgVillkor.DataSource =
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(
                    System.Configuration.ConfigurationManager.ConnectionStrings["gym"].ConnectionString, CommandType.Text, sql, p).Tables[
                        0];
            dgVillkor.AutoResizeColumns();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormCreateNewMember oForm = new FormCreateNewMember();
            if ( oForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK )
            {
                //It as created...
                if (MessageBox.Show(this, "Do you want to search for that user now?", "User creation in progress", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    txtNamn.Text = oForm.textBox1.Text;
                    button1_Click(null, null);
                }
            }
        }

        private void dgVillkor_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Get selected
            if (e.RowIndex < 0)
                return;
            string guid = dgVillkor.Rows[e.RowIndex].Cells[0].Value.ToString();
            FormMember oForm = new FormMember();
            oForm.guid = guid;
            oForm.ShowDialog(this);

        }
    }
}
