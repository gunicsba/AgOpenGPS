using System;
using System.Windows.Forms;
using AgOpenGPS.Logging;

namespace AgOpenGPS
{
    public partial class FormEventViewer : Form
    {
        public FormEventViewer()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormEventViewer_Load(object sender, EventArgs e)
        {
            rtbAutoSteerStopEvents.HideSelection = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (rtbAutoSteerStopEvents.TextLength != Log.System.RawLogs.Length)
            {
                rtbAutoSteerStopEvents.Clear();
                rtbAutoSteerStopEvents.AppendText(Log.System.RawLogs);
            }
        }
    }
}