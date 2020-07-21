﻿using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Windows.ApplicationModel;

namespace FluentTerminal.SystemTray
{
    public class SystemTrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;

        public SystemTrayApplicationContext()
        {
            var openMenuItem = new ToolStripMenuItem("Show", null, new EventHandler(OpenAppAsync));
            var newWindowItem = new ToolStripMenuItem("New terminal", null, new EventHandler(NewWindow));
            var settingsMenuItem = new ToolStripMenuItem("Show settings", null, new EventHandler(ShowSettings));
            var exitMenuItem = new ToolStripMenuItem("Exit", null, new EventHandler(Exit));

            //openMenuItem.DefaultItem = true;

            _notifyIcon = new NotifyIcon();
            _notifyIcon.DoubleClick += OpenAppAsync;
            _notifyIcon.Text = "Fluent Terminal";

            if (SystemUsesLightTheme())
            {
                _notifyIcon.Icon = Properties.Resources.Icon_mono_light;
            }
            else
            {
                _notifyIcon.Icon = Properties.Resources.Icon_mono_dark;
            }

            Container container = new Container();
            container.Add(openMenuItem);
            container.Add(newWindowItem);
            container.Add(settingsMenuItem);
            container.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = new ContextMenuStrip(container);
            _notifyIcon.Visible = true;
        }

        private void Exit(object sender, EventArgs e)
        {
            _notifyIcon.Dispose(); // cleans up the tray icon
            Application.Exit();
        }

        private void NewWindow(object sender, EventArgs e)
        {
            Process.Start("flute.exe", "new");
        }

        private async void OpenAppAsync(object sender, EventArgs e)
        {
            var appListEntries = await Package.Current.GetAppListEntriesAsync();
            await appListEntries.First().LaunchAsync();
        }

        private void ShowSettings(object sender, EventArgs e)
        {
            Process.Start("flute.exe", "settings");
        }

        /// <summary>
        /// Checks whether the new light system theme in Windows 10 1903+ is used
        /// </summary>
        private bool SystemUsesLightTheme()
        {
            try
            {
                using (var regKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, string.Empty).OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (regKey.GetValueNames().Contains("SystemUsesLightTheme"))
                    {
                        var value = regKey.GetValue("SystemUsesLightTheme");
                        return value is int intValue && intValue == 1;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}