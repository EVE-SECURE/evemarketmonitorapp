using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AutoUpdater
{
    public partial class Details : Form
    {
        private DataTable _data = new DataTable("Data");
        private Form1 _parent;

        public Details(List<ComponentData> components, Form1 parent)
        {
            _parent = parent;
            InitializeComponent();

            _data.Columns.Add(new DataColumn("Name", typeof(String)));
            _data.Columns.Add(new DataColumn("CurrentVersion", typeof(String)));
            _data.Columns.Add(new DataColumn("LatestVersion", typeof(String)));
            _data.Columns.Add(new DataColumn("Description", typeof(String)));

            foreach (ComponentData component in components)
            {
                DataRow newRow = _data.NewRow();
                newRow["Name"] = component.Name;
                newRow["CurrentVersion"] = (component.Exists ? (component.currentVersion.Major == 0 ? 
                    "Unknown" : component.currentVersion.ToString()) : "None");
                newRow["LatestVersion"] = component.latestVersion.ToString();
                newRow["Description"] = component.Description;
                _data.Rows.Add(newRow);
            }

            try
            {
                string tmpFile = parent.DownloadFile("VersionHistory.rtf");
                richTextBox1.LoadFile(tmpFile);

                try
                {
                    // Try and copy the latest version file into our work directory.
                    // Someone might want to look at it or somthing.
                    string versionFile = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar +
                        "VersionHistory.rtf";

                    File.Copy(tmpFile, versionFile, true);
                }
                catch { /* don't care */ }
            }
            catch (Exception ex)
            {
                richTextBox1.Text = "Error loading version history: " + ex.Message + "\r\n" + ex.StackTrace;
            }
        }

        private void Details_Load(object sender, EventArgs e)
        {
            updatesGrid.AutoGenerateColumns = false;
            updatesGrid.DataSource = _data;
            NameColumn.DataPropertyName = "Name";
            YourVersionColumn.DataPropertyName = "CurrentVersion";
            LatestVersionColumn.DataPropertyName = "LatestVersion";
            DescriptionColumn.DataPropertyName = "Description";
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}