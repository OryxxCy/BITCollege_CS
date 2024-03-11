using BITCollege_CS.Data;
using BITCollege_CS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITCollegeWindows
{
    public partial class BatchUpdate : Form
    {
        protected static BITCollege_CSContext db = new BITCollege_CSContext();
        private Batch batch = new Batch();

        public BatchUpdate()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Batch processing
        /// Further code to be added.
        /// </summary>
        private void lnkProcess_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (radSelect.Checked)
            {
                batch.ProcessTransmission(cbProgramAcronym.SelectedValue.ToString());
                string logData =  batch.WriteLogData();
                rtxtLog.AppendText(logData);
            }
            else if (radAll.Checked)
            {
                foreach (AcademicProgram program in cbProgramAcronym.Items)
                {
                    batch.ProcessTransmission(program.ProgramAcronym);
                    string logData = batch.WriteLogData();
                    rtxtLog.AppendText(logData);
                }
            }
        }

        /// <summary>
        /// given:  Always open this form in top right of frame.
        /// Further code to be added.
        /// </summary>
        private void BatchUpdate_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            IQueryable<AcademicProgram> academicPrograms = db.AcademicPrograms;
            academicProgramBindingSource.DataSource = academicPrograms.ToList();
        }

        /// <summary>
        /// Handles the change event of radAll radio button.
        /// </summary>
        private void radAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radSelect.Checked)
            {
                cbProgramAcronym.Enabled = true;
            }
            else
            {
                cbProgramAcronym.Enabled = false;
            }
        }

        /// <summary>
        /// Handles the click event of the test button.
        /// </summary>
        private void btnTest_Click(object sender, EventArgs e)
        {
            batch.ProcessTransmission(cbProgramAcronym.SelectedValue.ToString());
        }
    }
}
