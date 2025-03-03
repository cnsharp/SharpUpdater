using System;
using System.ComponentModel;
using System.Windows.Forms;
using CnSharp.Updater;
using CnSharp.Updater.Util;
using CnSharp.VisualStudio.Extensions.Projects;

namespace CnSharp.VisualStudio.SharpUpdater.Wizard
{
    public partial class ProductInfoControl : UserControl
    {
        public ProductInfoControl()
        {
            InitializeComponent();
        }

        private  Manifest _manifest;

        public Manifest Manifest
        {
            set
            {
                _manifest = value;
                if(_manifest != null)
                    BindManifest();
            }
            get
            {
                if(_manifest != null)
                    SyncManifest();
                return _manifest;
            }
        }

        private ProjectAssemblyInfo _projectAssemblyInfo;
        public ProjectAssemblyInfo ProjectAssemblyInfo
        {
            set
            {
                _projectAssemblyInfo = value;
                if (_projectAssemblyInfo != null)
                {
                    var ver = _projectAssemblyInfo.Version.ToSemanticVersion();
                    txtReleaseVer.Text = ver;
                    txtMinVer.Text = ver;
                }
                else
                {
                    txtReleaseVer.Clear();
                    txtMinVer.Clear();
                }
            }
        }


        private void SyncManifest()
        {
            _manifest.Id = txtId.Text.Trim();
            _manifest.AppName = txtAppName.Text.Trim();
            _manifest.Owner = txtOwner.Text.Trim();
            _manifest.WebSite = txtWebsite.Text.Trim();
            _manifest.Version = txtReleaseVer.Text.Trim();
            _manifest.MinVersion = txtMinVer.Text.Trim();
            _manifest.ReleaseNotes = txtNote.Text;
            _manifest.Language = txtLang.Text.Trim();
            _manifest.Copyright = txtCopyright.Text.Trim();
            _manifest.Tags = txtTags.Text.Trim();
        }


        private void BindManifest()
        {
            txtId.Text = _manifest.Id;
            txtAppName.Text = _manifest.AppName;
            txtOwner.Text = _manifest.Owner;
            txtWebsite.Text = _manifest.WebSite;
            txtReleaseVer.Text = _manifest.Version;
            txtMinVer.Text = _manifest.MinVersion;
            var desc = _manifest.ReleaseNotes;
            if (!string.IsNullOrEmpty(desc))
            {
                desc = desc.Replace("\n", Environment.NewLine);
            }
            txtNote.Text = desc;
            txtLang.Text = _manifest.Language;
            txtCopyright.Text = _manifest.Copyright;
            txtTags.Text = _manifest.Tags;
        }

        private void AddTextBoxEvents()
        {
            txtId.Validating += TextBoxRequiredValidating;
            txtId.Validated += TextBoxRequiredValidated;
            txtAppName.Validating += TextBoxRequiredValidating;
            txtAppName.Validated += TextBoxRequiredValidated;
            txtOwner.Validating += TextBoxRequiredValidating;
            txtOwner.Validated += TextBoxRequiredValidated;
            txtNote.Validating += TextBoxRequiredValidating;
            txtNote.Validated += TextBoxRequiredValidated;

            txtReleaseVer.Validating += TextBoxRequiredValidating;
            txtReleaseVer.Validating += VersionTextBoxValidating;
            txtReleaseVer.Validated += TextBoxRequiredValidated;

            txtMinVer.Validating += VersionTextBoxValidating;
            txtMinVer.Validating += TextBoxRequiredValidating;
            txtMinVer.Validated += TextBoxRequiredValidated;
        }

        private void VersionTextBoxValidating(object sender, CancelEventArgs e)
        {
            var box = sender as TextBox;
            if (box == null)
                return;
            if (!string.IsNullOrWhiteSpace(box.Text) && !box.Text.IsSemanticVersion())
            {
                errorProvider.SetError(box, "invalid version number");
                e.Cancel = true;
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtReleaseVer.Text) && !string.IsNullOrWhiteSpace(txtMinVer.Text) &&
                txtMinVer.Text.CompareVersion(txtReleaseVer.Text) > 0)
            {
                errorProvider.SetError(box, "Minimum Version > Release Version ?");
                e.Cancel = true;
            }
        }

        private void TextBoxRequiredValidating(object sender, CancelEventArgs e)
        {
            var box = sender as TextBox;
            if (box == null)
                return;
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                errorProvider.SetError(box, "*");
                e.Cancel = true;
            }
        }


        private void TextBoxRequiredValidated(object sender, EventArgs e)
        {
            var box = sender as TextBox;
            if (box == null)
                return;
            errorProvider.SetError(box, null);
        }

        private void ProductInfoForm_Load(object sender, EventArgs e)
        {
            ActiveControl = txtAppName;
            AddTextBoxEvents();
        }
    }
}
