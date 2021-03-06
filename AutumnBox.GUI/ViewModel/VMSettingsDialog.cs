﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/8/21 20:19:06 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.GUI.MVVM;
using AutumnBox.GUI.Properties;
using AutumnBox.GUI.Util.Bus;
using AutumnBox.GUI.Util.Custom;
using AutumnBox.GUI.Util.Debugging;
using AutumnBox.GUI.Util.I18N;
using AutumnBox.GUI.Util.Net;
using AutumnBox.GUI.Util.OS;
using AutumnBox.GUI.View.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

namespace AutumnBox.GUI.ViewModel
{
    class VMSettingsDialog : ViewModelBase
    {
        #region MVVM
        public bool DisplayCmdWindow
        {
            get
            {
                return Settings.Default.DisplayCmdWindow;
            }
            set
            {
                Settings.Default.DisplayCmdWindow = value;
                Basic.Util.Settings.CreateNewWindow = value;
                RaisePropertyChanged();
            }
        }

        public bool DeveloperMode
        {
            get
            {
                return Settings.Default.DeveloperMode;
            }
            set
            {
                Settings.Default.DeveloperMode = value;
                MainWindowBus.ReloadExtensionList();
                RaisePropertyChanged();
            }
        }

        public ICommand UpdateCheck
        {
            get => _updateCheck; set
            {
                _updateCheck = value;
                RaisePropertyChanged();
            }
        }
        private ICommand _updateCheck;

        public ICommand OpenLogFloder
        {
            get => _openLogFloder; set
            {
                _openLogFloder = value;
                RaisePropertyChanged();
            }
        }
        private ICommand _openLogFloder;


        public bool UseRandomTheme
        {
            get
            {
                return Settings.Default.RandomTheme;
            }
            set
            {
                Settings.Default.RandomTheme = value;
                Settings.Default.Save();
                if (value)
                {
                    ThemeManager.Instance.ApplyBySetting();
                }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(SelectedTheme));
            }
        }


        public IEnumerable<ILanguage> Languages
        {
            get => LanguageManager.Instance.Languages;
        }
        public ILanguage SelectedLanguage
        {
            get => LanguageManager.Instance.Current;
            set
            {
                LanguageManager.Instance.Current = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<ITheme> Themes { get => ThemeManager.Instance.Themes; }

        public ITheme SelectedTheme
        {
            get => ThemeManager.Instance.Current; set
            {
                Settings.Default.Theme = value.Name;
                Settings.Default.Save();
                ThemeManager.Instance.ApplyBySetting();
                RaisePropertyChanged();
            }
        }

        public bool DebugOnNext
        {
            get
            {
                return Settings.Default.ShowDebuggingWindowNextLaunch;
            }
            set
            {
                Settings.Default.ShowDebuggingWindowNextLaunch = value;
                RaisePropertyChanged();
            }
        }

        public bool SoundEffectEnable
        {
            get => Settings.Default.NotifyOnFinish; set
            {
                Settings.Default.NotifyOnFinish = value;
            }
        }

        public bool StartCmdAtDesktop
        {
            get
            {
                return Settings.Default.StartCmdAtDesktop;
            }
            set
            {
                Settings.Default.StartCmdAtDesktop = value;
                RaisePropertyChanged();
            }
        }

        public bool UseEnvVarCmd
        {
            get => Settings.Default.EnvVarCmdWindow;
            set
            {
                Settings.Default.EnvVarCmdWindow = value;
                RaisePropertyChanged();
                if (!value)
                {
                    StartCmdAtDesktop = false;
                }
            }
        }

        public bool DoubleClickToStartExtension
        {
            get => Settings.Default.DoubleClickRunExt;
            set
            {
                Settings.Default.DoubleClickRunExt = value;
                RaisePropertyChanged();
            }
        }

        public ICommand SendToDesktop { get; private set; }

        public ICommand ShowDebugWindow { get; private set; }

        public ICommand ResetSettings { get; private set; }

        public string ThemeDisplayMemberPath { get; set; } = nameof(ITheme.Name);
        public string LanguageDisplayMemberPath { get; set; } = nameof(ILanguage.LangName);

        #endregion
        public VMSettingsDialog()
        {
            RaisePropertyChangedOnDispatcher = true;
            ResetSettings = new MVVMCommand(ResetSettingsMethod);
            SendToDesktop = new MVVMCommand((_) =>
            {
                ShortcutHelper.CreateShortcutOnDesktop("AutumnBox", System.Environment.CurrentDirectory + "/AutumnBox.GUI.exe", "The AutumnBox-Dream of us");
            });
            ShowDebugWindow = new MVVMCommand((_) =>
            {
                new LogWindow().Show();
            });
            UpdateCheck = new FlexiableCommand(() =>
            {
                Updater.Do();
            });
            OpenLogFloder = new MVVMCommand((p) =>
            {
                try
                {
                    Process.Start(LoggingStation.LOG_FLODER);
                }
                catch { }
            });
        }

        private void ResetSettingsMethod(object para)
        {
            Settings.Default.Reset();
            Settings.Default.Save();
            foreach (var prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                RaisePropertyChanged(prop.Name);
            }
        }
    }
}
