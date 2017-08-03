using Assistant.DataProviders;
using Assistant.Manager;
using DevExpress.XtraTab;
using System;
using System.Windows.Data.Collection;
using System.Windows.Forms;
using WebBrowserUtils.HtmlUtils.History;
using Windows = System.Windows;

namespace Assistant
{
    public partial class MainRibbonForm
    {
        private static void OnCompareCarSelect(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            TreeModel tree = e.Parameter as TreeModel;
            RuleCompareNode node = tree.Children.Count > 0 ? tree.Children[0] as RuleCompareNode : null;
            if (node == null)
            {
                MessageBox.Show("车辆类型文件未正确加载！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (node.IsChecked == false)
            {
                MessageBox.Show("请选择车辆！", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                ((System.Windows.Window)sender).DialogResult = true;
            }
        }

        private void OnAddAppFile(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "选择文件";
            dialog.Filter = "(所有文件)|*.*";
            dialog.Multiselect = true;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Windows.DependencyObject d = sender as Windows.DependencyObject;
                AppFileManager manager = (d == null ? null : d.GetValue(Windows.FrameworkElement.DataContextProperty) as AppFileManager);
                try
                {
                    foreach (string name in dialog.FileNames)
                    {
                        AppFileInfo fileInfo = new AppFileInfo()
                        {
                            Version = FileHelper.GetCurrentVersion(),
                            FileName = name,
                            Status = FileStatus.New,
                            Enterprise = manager.SelectedEnterprise
                        };
                        int index = manager.Items.IndexOf(fileInfo);
                        if (index > -1)
                        {
                            AppFileInfo current = manager.Items[index] as AppFileInfo;
                            current.FileName = name;
                            continue;
                        }
                        manager.Items.Add(fileInfo);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnDeleteAppFile(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            AppFileInfo file = e.Parameter as AppFileInfo;
            DialogResult result = MessageBox.Show(string.Format("确认从版本{0}中移除文件{1}吗？", file.Version, file.OriginName), "确认", 
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == Windows.Forms.DialogResult.OK)
                file.Delete();
        }

        private void CanDeleteAppFiel(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            AppFileInfo file = e.Parameter as AppFileInfo;
            e.CanExecute = file != null && (file.Status == FileStatus.Uploaded || file.Status == FileStatus.Update);
        }

        private void OnRemoveAppFile(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            AppFileInfo file = e.Parameter as AppFileInfo;
            Windows.DependencyObject d = sender as Windows.DependencyObject;
            AppFileManager manager = null;
            while (d != null && manager == null)
            {
                manager = (d == null ? null : d.GetValue(Windows.FrameworkElement.DataContextProperty) as AppFileManager);
                d = System.Windows.Media.VisualTreeHelper.GetParent(d);
            }
            if (manager != null)
            {
                manager.Items.Remove(file);
            }
        }

        private void CanRemoveAppFiel(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            AppFileInfo file = e.Parameter as AppFileInfo;
            e.CanExecute = file != null && (file.Status == FileStatus.Deleted || file.Status == FileStatus.New || file.Status == FileStatus.None);
        }

        private static void OnOpenAppFile(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            AppFileInfo file = e.Parameter as AppFileInfo;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "选择文件";
            dialog.Filter = "(所有文件)|*.*";
            dialog.Multiselect = false;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
                file.FileName = dialog.FileName;
        }

        private static void CanOpenAppFile(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            Windows.DependencyObject d = sender as Windows.DependencyObject;
            AppFileManager manager = (d == null ? null : d.GetValue(Windows.FrameworkElement.DataContextProperty) as AppFileManager); 
            AppFileInfo file = e.Parameter as AppFileInfo;
            e.CanExecute = file != null && manager != null && manager.IsUploading == false;
        }

        private void OnOpenRuleFile(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            RuleFileInfo file = e.Parameter as RuleFileInfo;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "选择文件";
            dialog.Filter = "(所有文件)|*.*";
            dialog.Multiselect = false;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
                file.FileName = dialog.FileName;
        }

        private void CanOpenRuleFile(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            Windows.DependencyObject d = sender as Windows.DependencyObject;
            RuleFileManager manager = (d == null ? null : d.GetValue(Windows.FrameworkElement.DataContextProperty) as RuleFileManager);
            RuleFileInfo file = e.Parameter as RuleFileInfo;
            e.CanExecute = file != null && manager != null && manager.IsUploading == false;
        }

        private void OnUploadAppFile(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Windows.DependencyObject d = sender as Windows.DependencyObject;
            AppFileManager manager = (d == null ? null : d.GetValue(Windows.FrameworkElement.DataContextProperty) as AppFileManager);
            System.Threading.ThreadPool.QueueUserWorkItem(AppFileUploadWorker, manager);
        }

        private void CanUploadAppFile(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            Windows.DependencyObject d = sender as Windows.DependencyObject;
            AppFileManager manager = (d == null ? null : d.GetValue(Windows.FrameworkElement.DataContextProperty) as AppFileManager);
            e.CanExecute = manager != null && manager.IsUploading == false;
        }

        private void OnUploadFillRule(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Windows.DependencyObject d = sender as Windows.DependencyObject;
            RuleFileManager manager = (d == null ? null : d.GetValue(Windows.FrameworkElement.DataContextProperty) as RuleFileManager);
            System.Threading.ThreadPool.QueueUserWorkItem(RuleFileUploadWorker, manager);
        }

        private void CanUploadFillRule(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            Windows.DependencyObject d = sender as Windows.DependencyObject;
            RuleFileManager manager = (d == null ? null : d.GetValue(Windows.FrameworkElement.DataContextProperty) as RuleFileManager);
            e.CanExecute = manager != null && manager.IsUploading == false;
        }

        private void OnViewHistoryDetail(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            HistoryItem item = e.Parameter as HistoryItem;
            if (item != null)
            {
                XtraTabPage result = new XtraTabPage();
                System.Windows.Forms.Integration.ElementHost elementHost1 = new System.Windows.Forms.Integration.ElementHost();
                elementHost1.Dock = DockStyle.Fill;
                System.Windows.Controls.ContentControl contentCtrl = new System.Windows.Controls.ContentControl();
                System.Windows.ResourceDictionary res = new System.Windows.ResourceDictionary();
                res.BeginInit();
                res.Source = new Uri("pack://application:,,,/Assistant;component/Template/FillResult.xaml");
                res.EndInit();
                contentCtrl.ContentTemplate = res["fillResult"] as System.Windows.DataTemplate;
                try
                {
                    contentCtrl.DataContext = HistoryHelper.GetRecordList(item.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                contentCtrl.Content = contentCtrl.DataContext;
                elementHost1.Child = contentCtrl;
                result.Controls.Add(elementHost1);
                result.ShowCloseButton = DevExpress.Utils.DefaultBoolean.True;
                result.Text = "历史填报明细";
                this.tabPageControl.TabPages.Add(result);
                this.tabPageControl.SelectedTabPage = result;
            }
        }

        private static void AppFileUploadWorker(object state)
        {
            try
            {
                AppFileManager manager = state as AppFileManager;
                System.Threading.ThreadPool.QueueUserWorkItem((param) =>
                {
                    manager.SaveChange();
                }, null);
            }
            catch (Exception ex)
            {
                WebBrowserUtils.HtmlUtils.Fillers.WebFillManager.ShowMessageBox(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void RuleFileUploadWorker(object state)
        {
            try
            {
                RuleFileManager manager = state as RuleFileManager;
                System.Threading.ThreadPool.QueueUserWorkItem((param) =>
                {
                    manager.Upload();
                }, null);
            }
            catch (Exception ex)
            {
                WebBrowserUtils.HtmlUtils.Fillers.WebFillManager.ShowMessageBox(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
