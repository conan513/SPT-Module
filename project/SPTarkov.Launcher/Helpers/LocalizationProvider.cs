﻿using SPTarkov.Common.Utils.App;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SPTarkov.Launcher.Helpers
{
    public static class LocalizationProvider
    {
        public static string DefaultLocaleFolderPath = $"{Environment.CurrentDirectory}\\Launcher_Data\\Locales";

        public static void LoadLocaleFromFile(string localeName)
        {
            LocaleData newLocale = Json.LoadClassWithoutSaving<LocaleData>($"{DefaultLocaleFolderPath}\\{localeName}.json");

            if(newLocale != null)
            {
                foreach(var prop in Instance.GetType().GetProperties())
                {
                    prop.SetValue(Instance, newLocale.GetType().GetProperty(prop.Name).GetValue(newLocale));
                }
                
                LauncherSettingsProvider.Instance.DefaultLocale = localeName;
                LauncherSettingsProvider.Instance.SaveSettings();
            }

            //could possibly raise an event here to say why the local wasn't changed.
        }

        public static LocaleData GenerateEnglishLocale()
        {
            //Create default english locale data and save if the default locale data file dosen't exist.
            //This is to (hopefully) prevent the launcher from becoming 100% broken if no locale files exist or the locale files are outdated (missing data).
            LocaleData englishLocale = new LocaleData();
            #region Set All English Defaults
            englishLocale.retry = "Retry";
            englishLocale.server_connecting = "Connecting";
            englishLocale.server_unavailable_format_1 = "Default server '{0}' is not available.";
            englishLocale.no_servers_available = "No Servers found. Check server list in settings.";
            englishLocale.settings_cog_tooltip = "Settings Menu";
            englishLocale.back = "Back";
            englishLocale.wipe = "Wipe";
            englishLocale.wipe_profile = "Wipe Profile";
            englishLocale.email = "Email";
            englishLocale.password = "Password";
            englishLocale.update = "Update";
            englishLocale.edit_profile_update_error = "Some Issues occurred while updating your profile.";
            englishLocale.register = "Register";
            englishLocale.login = "Login";
            englishLocale.login_automatically = "Login Automatically";
            englishLocale.incorrect_login = "Email or password are incorrect";
            englishLocale.login_failed = "Login Failed";
            englishLocale.edition = "Edition";
            englishLocale.id = "ID";
            englishLocale.logout = "Logout";
            englishLocale.edit_profile = "Edit Profile";
            englishLocale.start_game = "Start Game";
            englishLocale.installed_in_live_game_warning = "SPTarkov shouldn't be installed into live game. Please make a copy of the gamefiles and install SPTarkov there.";
            englishLocale.no_official_game_warning = "Escape From Tarkov isn't installed on your computer. Please buy a copy of the game and support the developers!";
            englishLocale.launcher_not_running_in_gamedir_warning = "The launcher is not running from the game directory";
            englishLocale.account_exist = "Account already exists";
            englishLocale.local_server_remove_warning = "You are about to remove the local server.";
            englishLocale.remove_server_question = "Remove Server?";
            englishLocale.hide_to_tray = "Hide to system tray on game start";
            englishLocale.exit_settings_tooltip = "Exit Settings";
            englishLocale.name = "Name";
            englishLocale.save = "Save";
            englishLocale.url = "Url";
            englishLocale.new_server = "New Server";
            englishLocale.server_list_back_arrow_tooltip = "Back to server list";
            englishLocale.make_default = "Make Default";
            englishLocale.@default = "Default";
            englishLocale.server_list = "Server List";
            englishLocale.add = "Add";
            englishLocale.remove_server_tooltip = "Remove Server";
            englishLocale.default_language = "Default Language";
            #endregion

            Directory.CreateDirectory(LocalizationProvider.DefaultLocaleFolderPath);
            LauncherSettingsProvider.Instance.DefaultLocale = "English";
            LauncherSettingsProvider.Instance.SaveSettings();
            Json.SaveWithFormatting($"{LocalizationProvider.DefaultLocaleFolderPath}\\English.json", englishLocale, Newtonsoft.Json.Formatting.Indented);

            return englishLocale;
        }

        public static ObservableCollection<string> GetAvailableLocales()
        {
            return new ObservableCollection<string>(Directory.GetFiles(DefaultLocaleFolderPath).Select(x => new FileInfo(x).Name.Replace(".json", "")).ToList());
        }

        public static LocaleData Instance { get; private set; } = Json.LoadClassWithoutSaving<LocaleData>($"{DefaultLocaleFolderPath}\\{LauncherSettingsProvider.Instance.DefaultLocale}.json") ?? GenerateEnglishLocale();
    }

    public class LocaleData : INotifyPropertyChanged
    {
        //this is going to be some pretty long boiler plate code. So I'm putting everything into regions.

        #region retry
        private string _retry;
        public string retry
        {
            get => _retry;
            set
            {
                if(_retry != value)
                {
                    _retry = value;
                    RaisePropertyChanged(nameof(retry));
                }
            }
        }
        #endregion

        #region server_connecting
        private string _server_connecting;
        public string server_connecting
        {
            get => _server_connecting;
            set
            {
                if(_server_connecting != value)
                {
                    _server_connecting = value;
                    RaisePropertyChanged(nameof(server_connecting));
                }
            }
        }
        #endregion

        #region server_unavailable_format_1
        private string _server_unavailable_format_1;
        public string server_unavailable_format_1
        {
            get => _server_unavailable_format_1;
            set
            {
                if(_server_unavailable_format_1 != value)
                {
                    _server_unavailable_format_1 = value;
                    RaisePropertyChanged(nameof(server_unavailable_format_1));
                }
            }
        }
        #endregion

        #region no_servers_available
        private string _no_servers_available;
        public string no_servers_available
        {
            get => _no_servers_available;
            set
            {
                if(_no_servers_available != value)
                {
                    _no_servers_available = value;
                    RaisePropertyChanged(nameof(no_servers_available));
                }
            }
        }
        #endregion

        #region settings_cog_tooltip
        private string _settings_cog_tooltip;
        public string settings_cog_tooltip
        {
            get => _settings_cog_tooltip;
            set
            {
                if(_settings_cog_tooltip != value)
                {
                    _settings_cog_tooltip = value;
                    RaisePropertyChanged(nameof(_settings_cog_tooltip));
                }
            }
        }
        #endregion

        #region back
        private string _back;
        public string back
        {
            get => _back;
            set
            {
                if(_back != value)
                {
                    _back = value;
                    RaisePropertyChanged(nameof(back));
                }
            }
        }
        #endregion

        #region wipe
        private string _wipe;
        public string wipe
        {
            get => _wipe;
            set
            {
                if(_wipe != value)
                {
                    _wipe = value;
                    RaisePropertyChanged(nameof(wipe));
                }
            }
        }
        #endregion

        #region wipe_profile
        private string _wipe_profile;
        public string wipe_profile
        {
            get => _wipe_profile;
            set
            {
                if(_wipe_profile != value)
                {
                    _wipe_profile = value;
                    RaisePropertyChanged(nameof(wipe_profile));
                }
            }
        }
        #endregion

        #region email
        private string _email;
        public string email
        {
            get => _email;
            set
            {
                if(_email != value)
                {
                    _email = value;
                    RaisePropertyChanged(nameof(email));
                }
            }
        }
        #endregion

        #region password
        private string _password;
        public string password
        {
            get => _password;
            set
            {
                if(_password != value)
                {
                    _password = value;
                    RaisePropertyChanged(nameof(password));
                }
            }
        }
        #endregion

        #region update
        private string _update;
        public string update
        {
            get => _update;
            set
            {
                if(_update != value)
                {
                    _update = value;
                    RaisePropertyChanged(nameof(update));
                }
            }
        }
        #endregion

        #region edit_profile_update_error
        private string _edit_profile_update_error;
        public string edit_profile_update_error
        {
            get => _edit_profile_update_error;
            set
            {
                if(_edit_profile_update_error != value)
                {
                    _edit_profile_update_error = value;
                    RaisePropertyChanged(nameof(edit_profile_update_error));
                }
            }
        }
        #endregion

        #region register
        private string _register;
        public string register
        {
            get => _register;
            set
            {
                if(_register != value)
                {
                    _register = value;
                    RaisePropertyChanged(nameof(register));
                }
            }
        }
        #endregion

        #region login
        private string _login;
        public string login
        {
            get => _login;
            set
            {
                if(_login != value)
                {
                    _login = value;
                    RaisePropertyChanged(nameof(login));
                }
            }
        }
        #endregion

        #region login_automatically
        private string _login_automatically;
        public string login_automatically
        {
            get => _login_automatically;
            set
            {
                if(_login_automatically != value)
                {
                    _login_automatically = value;
                    RaisePropertyChanged(nameof(login_automatically));
                }
            }
        }
        #endregion

        #region incorrect_login
        private string _incorrect_login;
        public string incorrect_login
        {
            get => _incorrect_login;
            set
            {
                if(_incorrect_login != value)
                {
                    _incorrect_login = value;
                    RaisePropertyChanged(nameof(incorrect_login));
                }
            }
        }
        #endregion

        #region login_failed
        private string _login_failed;
        public string login_failed
        {
            get => _login_failed;
            set
            {
                if(_login_failed != value)
                {
                    _login_failed = value;
                    RaisePropertyChanged(nameof(login_failed));
                }
            }
        }
        #endregion

        #region edition
        private string _edition;
        public string edition
        {
            get => _edition;
            set
            {
                if(_edition != value)
                {
                    _edition = value;
                    RaisePropertyChanged(nameof(edition));
                }
            }
        }
        #endregion

        #region id
        private string _id;
        public string id
        {
            get => _id;
            set
            {
                if(_id != value)
                {
                    _id = value;
                    RaisePropertyChanged(nameof(id));
                }
            }
        }
        #endregion

        #region logout
        private string _logout;
        public string logout
        {
            get => _logout;
            set
            {
                if(_logout != value)
                {
                    _logout = value;
                    RaisePropertyChanged(nameof(logout));
                }
            }
        }
        #endregion

        #region edit_profile
        private string _edit_profile;
        public string edit_profile
        {
            get => _edit_profile;
            set
            {
                if(_edit_profile != value)
                {
                    _edit_profile = value;
                    RaisePropertyChanged(nameof(edit_profile));
                }
            }
        }
        #endregion

        #region start_game
        private string _start_game;
        public string start_game
        {
            get => _start_game;
            set
            {
                if (_start_game != value)
                {
                    _start_game = value;
                    RaisePropertyChanged(nameof(start_game));
                }
            }
        }
        #endregion

        #region installed_in_live_game_warning
        private string _installed_in_live_game_warning;
        public string installed_in_live_game_warning
        {
            get => _installed_in_live_game_warning;
            set
            {
                if(_installed_in_live_game_warning != value)
                {
                    _installed_in_live_game_warning = value;
                    RaisePropertyChanged(nameof(installed_in_live_game_warning));
                }
            }
        }
        #endregion

        #region no_official_game_warning
        private string _no_official_game_warning;
        public string no_official_game_warning
        {
            get => _no_official_game_warning;
            set
            {
                if(_no_official_game_warning != value)
                {
                    _no_official_game_warning = value;
                    RaisePropertyChanged(nameof(no_official_game_warning));
                }
            }
        }
        #endregion

        #region launcher_not_running_in_gamedir_warning
        private string _launcher_not_running_in_gamedir_warning;
        public string launcher_not_running_in_gamedir_warning
        {
            get => _launcher_not_running_in_gamedir_warning;
            set
            {
                if(_launcher_not_running_in_gamedir_warning != value)
                {
                    _launcher_not_running_in_gamedir_warning = value;
                    RaisePropertyChanged(nameof(launcher_not_running_in_gamedir_warning));
                }
            }
        }
        #endregion

        #region account_exist
        private string _account_exist;
        public string account_exist
        {
            get => _account_exist;
            set
            {
                if(_account_exist != value)
                {
                    _account_exist = value;
                    RaisePropertyChanged(nameof(account_exist));
                }
            }
        }
        #endregion

        #region local_server_remove_warning
        private string _local_server_remove_warning;
        public string local_server_remove_warning
        {
            get => _local_server_remove_warning;
            set
            {
                if(_local_server_remove_warning != value)
                {
                    _local_server_remove_warning = value;
                    RaisePropertyChanged(nameof(local_server_remove_warning));
                }
            }
        }
        #endregion

        #region remove_server_question
        private string _remove_server_question;
        public string remove_server_question
        {
            get => _remove_server_question;
            set
            {
                if(_remove_server_question != value)
                {
                    _remove_server_question = value;
                    RaisePropertyChanged(nameof(remove_server_question));
                }
            }
        }
        #endregion

        #region hide_to_tray
        private string _hide_to_tray;
        public string hide_to_tray
        {
            get => _hide_to_tray;
            set
            {
                if(_hide_to_tray != value)
                {
                    _hide_to_tray = value;
                    RaisePropertyChanged(nameof(hide_to_tray));
                }
            }
        }
        #endregion

        #region exit_settings_tooltip
        private string _exit_setttings_tooltip;
        public string exit_settings_tooltip
        {
            get => _exit_setttings_tooltip;
            set
            {
                if(_exit_setttings_tooltip != value)
                {
                    _exit_setttings_tooltip = value;
                    RaisePropertyChanged(nameof(exit_settings_tooltip));
                }
            }
        }
        #endregion

        #region name
        private string _name;
        public string name
        {
            get => _name;
            set
            {
                if(_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(nameof(name));
                }
            }
        }
        #endregion

        #region save
        private string _save;
        public string save
        {
            get => _save;
            set
            {
                if(_save != value)
                {
                    _save = value;
                    RaisePropertyChanged(nameof(save));
                }
            }
        }
        #endregion

        #region url
        private string _url;
        public string url
        {
            get => _url;
            set
            {
                if(_url != value)
                {
                    _url = value;
                    RaisePropertyChanged(nameof(_url));
                }
            }
        }
        #endregion

        #region new_server
        private string _new_server;
        public string new_server
        {
            get => _new_server;
            set
            {
                if(_new_server != value)
                {
                    _new_server = value;
                    RaisePropertyChanged(nameof(new_server));
                }
            }
        }
        #endregion

        #region server_list_back_arrow_tooltip
        private string _server_list_back_arrow_tooltip;
        public string server_list_back_arrow_tooltip
        {
            get => _server_list_back_arrow_tooltip;
            set
            {
                if(_server_list_back_arrow_tooltip != value)
                {
                    _server_list_back_arrow_tooltip = value;
                    RaisePropertyChanged(nameof(server_list_back_arrow_tooltip));
                }
            }
        }
        #endregion

        #region make_default
        private string _make_default;
        public string make_default
        {
            get => _make_default;
            set
            {
                if(_make_default != value)
                {
                    _make_default = value;
                    RaisePropertyChanged(nameof(make_default));
                }
            }
        }
        #endregion

        #region default
        private string _default;
        //@ symbol is because 'default' is a reserved word
        public string @default
        {
            get => _default;
            set
            {
                if(_default != value)
                {
                    _default = value;
                    RaisePropertyChanged(nameof(@default));
                }
            }
        }
        #endregion

        #region server_list
        private string _server_list;
        public string server_list
        {
            get => _server_list;
            set
            {
                if(_server_list != value)
                {
                    _server_list = value;
                    RaisePropertyChanged(nameof(server_list));
                }
            }
        }
        #endregion

        #region add
        private string _add;
        public string add
        {
            get => _add;
            set
            {
                if(_add != value)
                {
                    _add = value;
                    RaisePropertyChanged(nameof(add));
                }
            }
        }
        #endregion

        #region remove_server_tooltip
        private string _remove_server_tooltip;
        public string remove_server_tooltip
        {
            get => _remove_server_tooltip;
            set
            {
                if(_remove_server_tooltip != value)
                {
                    _remove_server_tooltip = value;
                    RaisePropertyChanged(nameof(remove_server_tooltip));
                }
            }
        }
        #endregion

        #region default_language
        private string _default_language;
        public string default_language
        {
            get => _default_language;
            set
            {
                if(_default_language != value)
                {
                    _default_language = value;
                    RaisePropertyChanged(nameof(default_language));
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
