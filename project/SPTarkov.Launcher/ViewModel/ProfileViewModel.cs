﻿using SPTarkov.Common.Utils.App;
using SPTarkov.Launcher.Generics;
using SPTarkov.Launcher.Generics.AsyncCommand;
using SPTarkov.Launcher.Helpers;
using System.Threading.Tasks;
using System.Windows;
using SPTarkov.Launcher.Models.Launcher;

namespace SPTarkov.Launcher.ViewModel
{
    public class ProfileViewModel
    {
        public string CurrentEmail { get; set; }
        public string CurrentEdition { get; set; }
        public string CurrentID { get; set; }
        public GenericICommand LogoutCommand { get; set; }
        public GenericICommand EditProfileCommand { get; set; }
        public AwaitableDelegateCommand StartGameCommand { get; set; }
        private NavigationViewModel navigationViewModel { get; set; }
        private GameStarter gameStarter = new GameStarter();
        private System.Windows.Forms.NotifyIcon trayIcon = new System.Windows.Forms.NotifyIcon();

        private ProcessMonitor monitor { get; set; }
        public ProfileViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;
            LogoutCommand = new GenericICommand(OnLogoutCommand);
            EditProfileCommand = new GenericICommand(OnEditProfileCommand);
            StartGameCommand = new AwaitableDelegateCommand(OnStartGameCommand);

            monitor = new ProcessMonitor("EscapeFromTarkov", 1000, aliveCallback: null, exitCallback: GameExitCallback);

            CurrentEmail = AccountManager.SelectedAccount.email;
            CurrentEdition = AccountManager.SelectedAccount.edition;
            CurrentID = AccountManager.SelectedAccount.id;

            trayIcon.Icon = Properties.Resources.icon;
            trayIcon.Text = "SPTarkov Launcher";
            trayIcon.Visible = false;
            trayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
            Application.Current.Exit += Current_Exit;
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
        }

        public void OnLogoutCommand(object parameter)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();

            navigationViewModel.SelectedViewModel = new LoginViewModel(navigationViewModel);
        }
        public void OnEditProfileCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new EditProfileViewModel(navigationViewModel);
        }
        public async Task OnStartGameCommand()
        {

            LauncherSettingsProvider.Instance.AllowSettings = false;

            int status = await AccountManager.LoginAsync(AccountManager.SelectedAccount.email, AccountManager.SelectedAccount.password);

            LauncherSettingsProvider.Instance.AllowSettings = true;

            switch (status)
            {
                case -1:
                    Current_Exit(null, null);

                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.incorrect_login);
                    return;

                case -2:
                    Current_Exit(null, null);

                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.login_failed);
                    navigationViewModel.SelectedViewModel = new ConnectServerViewModel(navigationViewModel);
                    return;
            }


            LauncherSettingsProvider.Instance.GameRunning = true;

            int gameStatus = gameStarter.LaunchGame(ServerManager.SelectedServer, AccountManager.SelectedAccount);

            switch (gameStatus)
            {
                case 1:
                    monitor.Start();

                    switch(LauncherSettingsProvider.Instance.LauncherStartGameAction)
                    {
                        case LauncherAction.MinimizeAction:
                            {
                                Application.Current.MainWindow.WindowState = WindowState.Minimized;
                                break;
                            }
                        case LauncherAction.MinimizeToSystemTrayAction:
                            {
                                trayIcon.Visible = true;
                                Application.Current.MainWindow.Hide();
                                break;
                            }
                        case LauncherAction.ExitAction:
                            {
                                Application.Current.Shutdown(0);
                                break;
                            }
                    }

                    break;

                case -1:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.installed_in_live_game_warning);
                    break;

                case -2:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.no_official_game_warning);
                    break;

                case -3:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.eft_exe_not_found_warning);
                    return;
            }
        }

        private void GameExitCallback(ProcessMonitor monitor)
        {
            monitor.Stop();

            LauncherSettingsProvider.Instance.GameRunning = false;

            //Make sure the call to MainWindow happens on the UI thread.
            switch(LauncherSettingsProvider.Instance.LauncherStartGameAction)
            {
                case LauncherAction.MinimizeToSystemTrayAction:
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Application.Current.MainWindow.Show();
                        });

                        break;
                    }
                case LauncherAction.MinimizeAction:
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Application.Current.MainWindow.WindowState = WindowState.Normal;
                        });

                        break;
                    }
            }
        }

        private void TrayIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }
    }
}
