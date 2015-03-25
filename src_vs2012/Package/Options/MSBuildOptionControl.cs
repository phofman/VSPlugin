using System;
using System.Windows.Forms;
using BlackBerry.Package.Helpers;
using BlackBerry.Package.ViewModels;

namespace BlackBerry.Package.Options
{
    public partial class MSBuildOptionControl : UserControl
    {
        private readonly MSBuildOptionViewModel _vm = new MSBuildOptionViewModel();

        public MSBuildOptionControl()
        {
            InitializeComponent();
        }

        private void bttInstall_Click(object sender, EventArgs e)
        {
            _vm.InstallMSBuildPlatform();
        }

        private void bttRemove_Click(object sender, EventArgs e)
        {
            if (MessageBoxHelper.Show("Do you really want to remove the \"BlackBerry\" build platform?", 
                                      null, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _vm.RemoveMSBuildPlatform();
            }
        }
    }
}
