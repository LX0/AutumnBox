/* =============================================================================*\
*
* Filename: StartWindow.xaml.cs
* Description: 
*
* Version: 1.0
* Created: 10/6/2017 03:31:15(UTC+8:00)
* Compiler: Visual Studio 2017
* 
* Author: zsh2401
* Company: I am free man
*
\* =============================================================================*/
using AutumnBox.GUI.Helper;
using AutumnBox.GUI.Windows;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutumnBox.Basic.Util;
using AutumnBox.Basic.MultipleDevices;
using System.Windows.Threading;
using System.Media;
using MaterialDesignThemes.Wpf;
using AutumnBox.GUI.Depending;
using AutumnBox.GUI.ViewModel;
using AutumnBox.GUI.View.DialogContent;
using AutumnBox.GUI.Util.Net;
using AutumnBox.GUI.Util.UI;

namespace AutumnBox.GUI
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private SoundPlayer audioPlayer;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new VMMainWindow();
            
            audioPlayer = new SoundPlayer("Resources/Sound/ok.wav");
            RegisterEvent();
        }

        private void InjectChildProperty()
        {
            ListenerManager.Instance.RegisterEventSource(PanelDevices);
        }

        private void RegisterEvent()
        {
            AdbHelper.AdbServerStartsFailed += (s, e) =>
            {
                DevicesMonitor.Stop();
                bool _continue = true;
                Dispatcher.Invoke(() =>
                {
                    _continue = BoxHelper.ShowChoiceDialog("msgWarning",
                        "msgStartAdbServerFail",
                        "btnExit", "btnIHaveCloseOtherPhoneHelper")
                        .ToBool();
                });
                if (!_continue)
                {
                    Close();
                }
                else
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(3000);
                        App.Current.Dispatcher.Invoke(DevicesMonitor.Begin);
                    });
                }
            };
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs _e)
        {
            InjectChildProperty();
            PanelDevices.Work();
#if !DEBUG
            Util.Extensions.SuppressScriptErrors(WTF, true);
            WTF.Navigate(App.Current.Resources["urlApiStatistics"].ToString());
#endif

#if ENABLE_BLUR
            UIHelper.SetOwnerTransparency(Config.BackgroundA);
            //开启Blur透明效果
            BlurHelper.EnableBlur(this);
            AllowsTransparency = true;
#endif
            new UpdateChecker().RunAsync((r) =>
            {
                if (r.NeedUpdate)
                {
                    new UpdateNoticeWindow(r) { Owner = this }.ShowDialog();
                }
            });

            //哦,如果是第一次启动本软件,那么就显示一下提示吧!
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                Dispatcher.Invoke(() =>
                {
                    if (Properties.Settings.Default.IsFirstLaunch)
                    {
                        DialogHost.Show(new ContentAbout());
                    }
                });
            });
        }

        private void _MainWindow_Closed(object sender, EventArgs e)
        {
            foreach (Window window in App.Current.Windows)
            {
                window.Close();
            }
        }
    }
}
