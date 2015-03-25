using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace BlackBerry.Package.Options
{
    /// <summary>
    /// Option page to manage MSBuild settings.
    /// </summary>
    [Guid("6be58513-5765-423c-aa0a-64b9554294d4")]
    public sealed class MSBuildOptionPage : DialogPage
    {
        #region Control

        private MSBuildOptionControl _control;

        private MSBuildOptionControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new MSBuildOptionControl();
                    _control.Location = new Point(0, 0);
                }

                return _control;
            }
        }

        [Browsable(false)]
        protected override IWin32Window Window
        {
            get
            {
                return Control;
            }
        }

        #endregion

    }
}
