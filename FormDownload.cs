using System.Windows.Forms;

namespace PriorityChatV3
{
    public partial class FormDownload : Form
    {
        public FormDownload()
        {
            InitializeComponent();
        }

        public void SetProgress(int percent)
        {
            progressBar1.Value = percent;
        }

        private void FormDownload_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
