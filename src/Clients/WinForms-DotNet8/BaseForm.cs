using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace CnSharp.Windows.Updater
{
    public class BaseForm : Form
    {
        #region Constants and Fields

        protected bool isEnableCloseButton;

        #endregion

        public BaseForm()
        {
            InitializeComponent();
        }

        #region Properties

        protected override CreateParams CreateParams
        {
            get
            {
                if (isEnableCloseButton)
                {
                    CreateParams parameters = base.CreateParams;
                    return parameters;
                }
                else
                {
                    int CS_NOCLOSE = 0x200;
                    CreateParams parameters = base.CreateParams;
                    parameters.ClassStyle |= CS_NOCLOSE;
                    return parameters;
                }
            }
        }

        #endregion

        #region Public Methods

        public DialogResult CheckProcessing(string exeName)
        {
            if (Process.GetProcessesByName(exeName).Length > 0)
            {
                DialogResult rs = MessageBox.Show(
                    string.Format(Common.GetLocalText("CloseRunning"), exeName),
                    Common.GetLocalText("Warning"),
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                if (rs == DialogResult.Retry)
                {
                    return CheckProcessing(exeName);
                }
                return rs;
            }
            return DialogResult.OK;
        }

        #endregion


        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(BaseForm));
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "BaseForm";
            this.ResumeLayout(false);

        }
    }
}