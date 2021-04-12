using System;
using System.IO;
using System.Reflection;
using Eto.Forms;
using Eto.WinForms;
using NAPS2.Config;
using NAPS2.Lang.Resources;
using NAPS2.Logging;
using NAPS2.Update;
using NAPS2.Util;

namespace NAPS2.EtoForms.Ui
{
    public class AboutForm : EtoFormBase
    {
        private const string NAPS2_HOMEPAGE = "https://www.naps2.com";  
        private const string ICONS_HOMEPAGE = "https://www.fatcow.com/free-icons";

        private readonly UpdateChecker _updateChecker;
        private readonly ConfigScopes _configScopes;
        
        private readonly CheckBox _checkForUpdates;
        private readonly Panel _updatePanel;
        
        private bool _hasCheckedForUpdates;
        private UpdateInfo? _update;
    
        public AboutForm(UpdateChecker updateChecker, ConfigScopes configScopes)
        {
            _updateChecker = updateChecker;
            _configScopes = configScopes;
            
            _checkForUpdates = new CheckBox { Text = UiStrings.CheckForUpdates };
            _checkForUpdates.CheckedChanged += CheckForUpdatesChanged;
            _updatePanel = new Panel();
            
            BuildLayout();
            UpdateControls();
        }

        private void BuildLayout()
        {
            Title = UiStrings.AboutFormTitle;
            Icon = Icons.information_small.ToEtoIcon();

            // TODO: Default padding
            // TODO: Tune the padding and spacing for aesthetics
            // TODO: Re-add donate button
            Content = L.Row(
                L.Column(new ImageView { Image = Icons.scanner_large.ToEto() }),
                L.Column(
                    C.NoWrap(AssemblyProduct),
                    C.NoWrap(string.Format(MiscResources.Version, AssemblyVersion)),
                    C.Link(NAPS2_HOMEPAGE),
                    C.TextSpace(),
                    _checkForUpdates,
                    _updatePanel,
                    C.TextSpace(),
                    C.NoWrap(UiStrings.Copyright),
                    C.TextSpace(),
                    L.Row(
                        L.Column(
                            C.NoWrap(UiStrings.IconsFrom),
                            C.Link(ICONS_HOMEPAGE)
                        ).XScale(),
                        C.Button(UiStrings.OK, Close)
                    ),
                    C.ZeroSpace()
                )
            );
        }
        
        private void DoUpdateCheck()
        {
            if (_checkForUpdates.Checked == true)
            {
                _updateChecker.CheckForUpdates().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Log.ErrorException("Error checking for updates", task.Exception);
                    }
                    else
                    {
                        _configScopes.User.SetAll(new CommonConfig
                        {
                            HasCheckedForUpdates = true,
                            LastUpdateCheckDate = DateTime.Now
                        });
                    }
                    _update = task.Result;
                    _hasCheckedForUpdates = true;
                    Invoker.Current.SafeInvoke(UpdateControls);
                });
            }
        }

        private void UpdateControls()
        {
            _updatePanel.Content = GetUpdatePanelContent();
        }

        private Control GetUpdatePanelContent()
        {
            if (_checkForUpdates.Checked != true)
            {
                return C.NoWrap(MiscResources.UpdateCheckDisabled);
            }
            if (!_hasCheckedForUpdates)
            {
                return C.NoWrap(MiscResources.CheckingForUpdates);
            }
            if (_update == null)
            {
                return C.NoWrap(MiscResources.NoUpdates);
            }
            return C.Link(string.Format(MiscResources.Install, _update.Name),
                InstallLinkClicked);
        }

        private void InstallLinkClicked()
        {
            if (_update != null)
            {
                _updateChecker.StartUpdate(_update);
            }
        }

        private void CheckForUpdatesChanged(object sender, EventArgs e)
        {
            _configScopes.User.Set(c => c.CheckForUpdates = _checkForUpdates.Checked);
            UpdateControls();
            DoUpdateCheck();
        }

        // TODO: Move to a utility class
        #region Assembly Attribute Accessors

        private static string GetAssemblyAttributeValue<T>(Func<T, string> selector)
        {
            object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return selector((T)attributes[0]);
        }

        public string AssemblyTitle
        {
            get
            {
                string title = GetAssemblyAttributeValue<AssemblyTitleAttribute>(x => x.Title);
                if (string.IsNullOrEmpty(title))
                {
                    title = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
                }
                return title;
            }
        }

        public string AssemblyVersion => Assembly.GetEntryAssembly().GetName().Version.ToString();

        public string AssemblyDescription => GetAssemblyAttributeValue<AssemblyDescriptionAttribute>(x => x.Description);

        public string AssemblyProduct => GetAssemblyAttributeValue<AssemblyProductAttribute>(x => x.Product);

        public string AssemblyCopyright => GetAssemblyAttributeValue<AssemblyCopyrightAttribute>(x => x.Copyright);

        public string AssemblyCompany => GetAssemblyAttributeValue<AssemblyCompanyAttribute>(x => x.Company);

        #endregion
    }
}