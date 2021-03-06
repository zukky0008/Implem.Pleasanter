﻿using Implem.SupportTools.SysLogViewer.Model;
using Implem.SupportTools.SysLogViewer.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Implem.SupportTools.SysLogViewer.View
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class SysLogViewerWindow : UserControl
    {
        private SysLogViewerViewModel VM { get => DataContext as SysLogViewerViewModel; }
        public SysLogViewerWindow()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await VM?.GetSysLogsAsync(listView.Dispatcher);
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var syslog = (e.Item as SysLogModel);
            e.Accepted = VM?.AcceptedSysLogTypes(syslog.SysLogType) == true;
            
        }
        
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (listView.ItemsSource == null) { return; }
            var view = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Refresh();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            VM?.Dispose();
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            VM?.GetSysLogsAsync(listView.Dispatcher);
        }

        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = ((sender as ListViewItem)?.DataContext as SysLogModel);
            if(item == null) { return; }

            var window = new DetailWindow() { DataContext = new DetailWindowViewModel(item) };
            window.ShowDialog();

        }
    }
}
