using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using GraphControlDemo.ViewModels;
using Microsoft.Graph;
using Microsoft.Toolkit.Graph.Providers;
using Windows.UI.Xaml.Controls;

namespace GraphControlDemo.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel
        {
            get { return ViewModelLocator.Current.MainViewModel; }
        }
        public ObservableCollection<DriveItem> DriveItems { get; set; } = new ObservableCollection<DriveItem>();

        

        private string folderId { get; set; }
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DriveItems.Clear();
            var provider = ProviderManager.Instance.GlobalProvider;

            if (provider != null && provider.State == ProviderState.SignedIn)
            {
                ResultTb.Text = "开始请求数据...";
                var driveItems = await provider.Graph.Me.Drive.Root.Children.Request().GetAsync();
                foreach (var item in driveItems.CurrentPage)
                {
                    DriveItems.Add(item);
                }
                ResultTb.Text = "";
            }
            else
            {
                ResultTb.Text = "未登录，请先登录。";
            }
        }

        private async void AddFolder_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var provider = ProviderManager.Instance.GlobalProvider;

            if (provider != null && provider.State == ProviderState.SignedIn)
            {
                ResultTb.Text = "开始请求数据...";
                var driveItem = new DriveItem
                {
                    Name = folderTbox.Text,
                    Folder = new Folder
                    {
                    },
                    AdditionalData = new Dictionary<string, object>()
                    {
                        {"@microsoft.graph.conflictBehavior", "rename"}
                    }
                };
                var folderitem = await provider.Graph.Me.Drive.Root.Children.Request().AddAsync(driveItem);
                folderId = folderitem.Id;

                DriveItems.Clear();
                var driveItems = await provider.Graph.Me.Drive.Root.Children.Request().GetAsync();
                foreach (var item in driveItems.CurrentPage)
                {
                    DriveItems.Add(item);
                }
                ResultTb.Text = "";
            }
            else
            {
                ResultTb.Text = "未登录，请先登录。";
            }
        }

        private async void UpFile_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
            var provider = ProviderManager.Instance.GlobalProvider;
            if (provider != null && provider.State == ProviderState.SignedIn)
            {
                ResultTb.Text = "开始请求数据...";
                var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(@"The contents of the file goes here."));

                DriveItemUploadableProperties properties = new DriveItemUploadableProperties
                {
                    Name = "new text.txt"
                };

                // 上传到指定文件夹
                //await provider.Graph.Me.Drive.Items[folderId].ItemWithPath("test.txt").Content.Request().PutAsync<DriveItem>(stream);

                // 上传到根目录
                //await provider.Graph.Me.Drive.Root.ItemWithPath("test.txt").Content.Request().PutAsync<DriveItem>(stream);

                var path = "AAA/test.txt";
                // 按照路径上传,如果路径中文件夹不存在会自动新建
                await provider.Graph.Me.Drive.Root.ItemWithPath(path).Content.Request().PutAsync<DriveItem>(stream);

                DriveItems.Clear();
                var driveItems = await provider.Graph.Me.Drive.Root.ItemWithPath(path).Children.Request().GetAsync();
                foreach (var item in driveItems.CurrentPage)
                {
                    DriveItems.Add(item);
                }
                ResultTb.Text = "";
            }
            else
            {
                ResultTb.Text = "未登录，请先登录。";
            }
        }

       
        private async void GetRoot_CLick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            if (provider != null && provider.State == ProviderState.SignedIn)
            {
                ResultTb.Text = "开始请求数据...";

                var driveItems = await provider.Graph.Me.Drive.Root.ItemWithPath(folderTbox.Text).Children.Request().GetAsync();

                DriveItems.Clear();
                foreach (var item in driveItems.CurrentPage)
                {
                    DriveItems.Add(item);
                }

                ResultTb.Text = "";
            }
            else
            {
                ResultTb.Text = "未登录，请先登录。";
            }
        }

        private async void GetPathFile_CLick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var provider = ProviderManager.Instance.GlobalProvider;
            if (provider != null && provider.State == ProviderState.SignedIn)
            {
                ResultTb.Text = "开始请求数据...";

                var file = await provider.Graph.Me.Drive.Root.ItemWithPath("A/test.txt").Request().GetAsync();
                
                ResultTb.Text = $"{file.Name} {file.Id} {file.LastModifiedDateTime}";
            }
            else
            {
                ResultTb.Text = "未登录，请先登录。";
            }
        }


    }
}
