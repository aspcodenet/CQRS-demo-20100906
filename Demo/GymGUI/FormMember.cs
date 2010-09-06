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
    public partial class FormMember : Form
    {
        public string guid;
        public FormMember()
        {
            InitializeComponent();
        }

        private void FormMember_Load(object sender, EventArgs e)
        {
            string sql = "select * from report_memberlist where id=@id";
            var p = new List<System.Data.SqlClient.SqlParameter>
                        {
                            new System.Data.SqlClient.SqlParameter("@id", Guid.Parse(guid) )
                        }.ToArray();

            DataRow row = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(System.Configuration.ConfigurationManager.ConnectionStrings["gym"].ConnectionString, CommandType.Text, sql, p).Tables[
                        0].
            Rows[0];

            textBox1.Text = row["membername"].ToString();
            textBox2.Text = row["traininglevel"].ToString();
            textBox3.Text = row["memberno"].ToString();
            checkBox1.Checked = row["isgold"].ToString().ToUpper() == "J";

            button1.Enabled = row["traininglevel"].ToString() != "Expert";
            button2.Enabled = row["traininglevel"].ToString() != "Beginner";
            button3.Enabled = true;


            //Old style
            textBox6.Text = row["membername"].ToString();
            comboBox1.SelectedItem = row["traininglevel"].ToString();
            textBox4.Text = row["memberno"].ToString();
            checkBox2.Checked = row["isgold"].ToString().ToUpper() == "J";

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormLevel oform = new FormLevel();
            oform.guid = guid;
            oform.currentlevel = textBox2.Text;
            oform.action = FormLevel.ChangeAction.Upgrade;
            if( oform.ShowDialog(this) == System.Windows.Forms.DialogResult.OK )
            {
                textBox2.Text = oform.currentlevel;
                textBox2.Font = new Font(textBox2.Font, FontStyle.Bold);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormLevel oform = new FormLevel();
            oform.guid = guid;
            oform.currentlevel = textBox2.Text;
            oform.action = FormLevel.ChangeAction.Downgrade;
            if (oform.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = oform.currentlevel;
                textBox2.Font = new Font(textBox2.Font, FontStyle.Italic);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormLevel oform = new FormLevel();
            oform.guid = guid;
            oform.currentlevel = textBox2.Text;
            oform.action = FormLevel.ChangeAction.Correct;
            if (oform.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = oform.currentlevel;
                textBox2.Font = new Font(textBox2.Font, FontStyle.Italic);
            }

        }
    }
}
