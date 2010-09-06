using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GymGoldMemberCommands.Enums;

namespace GymGUI
{
    public partial class FormLevel : Form
    {
        public string guid;
        public string currentlevel;
        public ChangeAction action;
        public enum ChangeAction
        {
            Upgrade,
            Downgrade,
            Correct
        }
        public FormLevel()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentlevel = "";
            GymGoldMemberCommands.Enums.TrainngLevels newlevel;
            if (radioButton1.Checked)
            {
                currentlevel = "Beginner";
                newlevel = TrainngLevels.Beginner;
            }
            else if (radioButton2.Checked)
            {
                currentlevel = "Intermediate";
                newlevel = TrainngLevels.Intermediate;
            }
            else if (radioButton3.Checked)
            {
                currentlevel = "Expert";
                newlevel = TrainngLevels.Expert;
            }
            else
            {
                MessageBox.Show("Please select a new level");
                return;
            }
            if ( action == ChangeAction.Upgrade)
                Communications.Bus.Instance().ServiceBus.Send(new GymGoldMemberCommands.UpgradeTrainingLevel(Guid.Parse(guid),newlevel));
            if (action == ChangeAction.Downgrade)
                Communications.Bus.Instance().ServiceBus.Send(new GymGoldMemberCommands.DowngradeTrainingLevel(Guid.Parse(guid), newlevel));
            if (action == ChangeAction.Correct)
                Communications.Bus.Instance().ServiceBus.Send(new GymGoldMemberCommands.CorrectTrainingLevel(Guid.Parse(guid), newlevel));
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void FormLevel_Load(object sender, EventArgs e)
        {
            textBox2.Text = currentlevel;
            //Hardcoded but thats not the point
            if ( action == ChangeAction.Upgrade)
            {
                radioButton1.Enabled = false;
                radioButton2.Enabled = currentlevel == "Beginner";
                radioButton3.Enabled = true;
            }
            if (action == ChangeAction.Downgrade)
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = currentlevel == "Expert";
                radioButton3.Enabled = false;
            }
            if (action == ChangeAction.Downgrade)
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
            }

        }
    }
}
