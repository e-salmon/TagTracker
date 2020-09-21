using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TagTracker.Adorners;
using TagTracker.Helpers;
using TagTracker.TagLibService.Interfaces;
using TagTracker.TagLibService.Services;
using TagTracker.Utility;
using TagTracker.ViewModel;
using ListViewItem = System.Windows.Controls.ListViewItem;

namespace TagTracker
{
    public sealed partial class MainWindow
    {
        private static readonly DependencyProperty PathHistoryProperty =
            DependencyProperty.Register(
                "PathHistory",
                typeof(ObservableCollection<string>),
                typeof(MainWindow));

        private static readonly DependencyProperty CurrentAlbumArtIndexProperty =
            DependencyProperty.Register(
                "CurrentAlbumArtIndex",
                typeof(int),
                typeof(MainWindow));

        private static readonly DependencyProperty CurrentAlbumArtDimensionProperty =
            DependencyProperty.Register(
                "CurrentAlbumArtDimension",
                typeof(string),
                typeof(MainWindow));

        private const string NoCoverImageFile = @"/Images/NoCover.png";

        private readonly BitmapImage _noCoverImage;

        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private SortAdorner _sortAdorner;

        public MainWindow()
        {
            InitializeComponent();

            PathHistory = new ObservableCollection<string>();
            PathHistoryComboBox.Items.SortDescriptions.Add(new SortDescription());

            _noCoverImage = new BitmapImage();
            InitializeNoCoverImage();
        }

        public ObservableCollection<string> PathHistory
        {
            get => (ObservableCollection<string>)GetValue(PathHistoryProperty);
            private set => SetValue(PathHistoryProperty, value);
        }

        public int CurrentAlbumArtIndex
        {
            get => (int)GetValue(CurrentAlbumArtIndexProperty);
            private set => SetValue(CurrentAlbumArtIndexProperty, value);
        }

        public string CurrentAlbumArtDimension
        {
            get => (string)GetValue(CurrentAlbumArtDimensionProperty);
            private set => SetValue(CurrentAlbumArtDimensionProperty, value);
        }

        private static bool IsItemPopulated(ItemsControl item)
        {
            return !(item.Items.Count == 1
                && item.Items[0] is string
                && (string)item.Items[0] == "*");
        }

        private static void PopulateTreeLevel(ItemsControl item)
        {
            if (item == null) return;

            item.Items.Clear();

            IEnumerable<string> folders = DirectoryService.SafeFolders(item.Tag as string);

            string path = (string)item.Tag;
            TreeViewItem parent = VisualTreeHelper.GetParent(item) as TreeViewItem;
            if (folders == null)
            {
                return;
            }

            while (parent != null)
            {
                path = (parent.Tag as string) + @"\" + path;
                parent = VisualTreeHelper.GetParent(parent) as TreeViewItem;
            }

            foreach (string folder in folders)
            {
                TreeViewItem newItem = new TreeViewItem
                {
                    Tag = folder,
                    Header = folder
                        .Replace(path, string.Empty, StringComparison.OrdinalIgnoreCase)
                        .Replace(@"\", string.Empty, StringComparison.OrdinalIgnoreCase)
                };
                newItem.Items.Add("*");
                item.Items.Add(newItem);
            }
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderDialog.SelectedPath = PathHistoryComboBox.Text;
                folderDialog.ShowNewFolderButton = false;
                folderDialog.Description = "Select a folder:";
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ExpandTreeToPath(folderDialog.SelectedPath);
                    AddPathToHistory(folderDialog.SelectedPath);
                }
            }
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RefreshCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is ListViewItem sourceItem)
            {
                if (sourceItem.Content is TrackInfoViewModel model)
                {
                    model.Refresh();
                    UpdateAlbumArt(model);
                }
            }
            else
            {
                TreeViewItem selectedItem = FoldersTreeView.SelectedItem as TreeViewItem;
                PopulateTreeLevel(selectedItem);
                if (selectedItem != null)
                {
                    PopulateContentList(selectedItem.Tag as string);
                }
            }
        }

        private void RefreshCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true; // FoldersTreeView != null && FoldersTreeView.SelectedItem != null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateTree();
        }

        private void IncludeSubFoldersCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (FoldersTreeView.SelectedItem is TreeViewItem item)
            {
                PopulateContentList(item.Tag as string);
            }
        }

        private void FoldersTreeView_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)e.OriginalSource;

            if (item != null && !IsItemPopulated(item))
            {
                PopulateTreeLevel(item);
            }
        }

        private void FoldersTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem item)
            {
                PopulateContentList(item.Tag as string);
            }
        }

        private void ContentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                UpdateAlbumArt(null);
                return;
            }

            CurrentAlbumArtIndex = 1;
            UpdateAlbumArt((TrackInfoViewModel)e.AddedItems[0]);
        }

        private void PathHistoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FoldersTreeView.SelectedItem is TreeViewItem item)
            {
                string selectedPath = e.AddedItems[0].ToString();
                string selectedNode = item.Tag as string;

                if (selectedPath != selectedNode)
                {
                    ExpandTreeToPath(selectedPath);
                    PopulateContentList(selectedPath);
                }
            }
        }

        private void PreviousAlbumArtButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPreviousAlbumArt();
        }

        private void NextAlbumArtButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNextAlbumArt();
        }

        private void AlbumCoverImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ShowNextAlbumArt();
            }
            else
            {
                ShowPreviousAlbumArt();
            }
        }

        private void InitializeNoCoverImage()
        {
            _noCoverImage.BeginInit();
            _noCoverImage.UriSource = new Uri(NoCoverImageFile, UriKind.RelativeOrAbsolute);
            _noCoverImage.EndInit();
        }

        private void AddPathToHistory(string path)
        {
            if (!PathHistory.Contains(path))
            {
                PathHistory.Add(path);
                PathHistoryComboBox.SelectedValue = path;
            }
        }

        private void PopulateContentList(string path)
        {
            if (!Directory.Exists(path))
            {
                ContentListView.ItemsSource = null;
                return;
            }
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                AddPathToHistory(path);

                ContentListView.ItemsSource =
                    (from t in TagReaderService.ReadTracksInfo(
                        path,
                        IncludeSubFoldersCheckBox.IsChecked == true)
                     select new TrackInfoViewModel { TrackInfo = t }).ToList();

                if (!ContentListView.HasItems)
                {
                    return;
                }

                _lastHeaderClicked ??= (GridViewColumnHeader)((GridView)ContentListView.View).Columns[0].Header;

                Sort(_lastHeaderClicked.Content as string, _lastDirection == ListSortDirection.Ascending);
                ContentListView.SelectedIndex = 0;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void ShowPreviousAlbumArt()
        {
            CurrentAlbumArtIndex--;
            UpdateAlbumArt((TrackInfoViewModel)ContentListView.SelectedItem);
        }

        private void ShowNextAlbumArt()
        {
            CurrentAlbumArtIndex++;
            UpdateAlbumArt((TrackInfoViewModel)ContentListView.SelectedItem);
        }

        private void UpdateAlbumArt(TrackInfoViewModel trackInfoViewModel)
        {
            if (trackInfoViewModel?.TrackInfo == null
                || trackInfoViewModel.TrackInfo.AlbumArtCount == 0
                || !File.Exists(trackInfoViewModel.TrackInfo.FileName))
            {
                AlbumCoverImage.Source = _noCoverImage;
                CurrentAlbumArtIndex = 0;
                CurrentAlbumArtDimension = null;
                PreviousAlbumArtButton.IsEnabled = false;
                NextAlbumArtButton.IsEnabled = false;
            }
            else
            {
                ITrackInfo info = trackInfoViewModel.TrackInfo;
                if (CurrentAlbumArtIndex < 1)
                {
                    CurrentAlbumArtIndex = 1;
                }

                if (CurrentAlbumArtIndex > info.AlbumArtCount)
                {
                    CurrentAlbumArtIndex = info.AlbumArtCount;
                }

                try
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = new MemoryStream(info.AlbumArt(CurrentAlbumArtIndex - 1));
                    bmp.EndInit();
                    AlbumCoverImage.Source = bmp;
                    CurrentAlbumArtDimension = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}x{1}",
                        bmp.PixelWidth,
                        bmp.PixelHeight);
                    PreviousAlbumArtButton.IsEnabled = CurrentAlbumArtIndex > 1;
                    NextAlbumArtButton.IsEnabled = CurrentAlbumArtIndex < info.AlbumArtCount;
                }
                catch (Exception)
                {
                    AlbumCoverImage.Source = _noCoverImage;
                    CurrentAlbumArtIndex = 0;
                    CurrentAlbumArtDimension = null;
                    PreviousAlbumArtButton.IsEnabled = false;
                    NextAlbumArtButton.IsEnabled = false;
                }
            }
        }

        private void PopulateTree()
        {
            FoldersTreeView.Items.Clear();

            foreach (string drive in DirectoryService.DriveNames())
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = drive,
                    Tag = drive
                };
                item.Items.Add("*");
                TreeViewItemProps.SetIsRootLevel(item, true);
                FoldersTreeView.Items.Add(item);
            }
        }

        private void ExpandTreeToPath(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);

                TreeViewItem folder = ExpandLevel(dirInfo);
                if (folder != null)
                {
                    folder.BringIntoView();
                    folder.IsSelected = true;
                    FoldersTreeView.Focus();
                }
            }
        }

        private TreeViewItem ExpandLevel(DirectoryInfo dirInfo)
        {
            TreeViewItem folder;

            if (dirInfo.Parent == null)
            {
                folder = (from TreeViewItem item in FoldersTreeView.Items
                          where dirInfo.Root.Name == item.Header.ToString()
                          select item).FirstOrDefault();
            }
            else
            {
                folder = (from TreeViewItem item in ExpandLevel(dirInfo.Parent).Items
                          where dirInfo.Name == item.Header.ToString()
                          select item).FirstOrDefault();
            }

            if (folder != null)
            {
                // Triggers a call to event FoldersTreeView_Expanded.
                folder.IsExpanded = true;
            }

            return folder;
        }

        private void ContentListView_ColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader headerClicked)
            {
                if (_lastHeaderClicked != null && _sortAdorner != null)
                {
                    AdornerLayer.GetAdornerLayer(_lastHeaderClicked)?.Remove(_sortAdorner);
                }

                ListSortDirection direction;
                if (!headerClicked.Equals(_lastHeaderClicked))
                {
                    direction = ListSortDirection.Ascending;
                }
                else
                {
                    direction = _lastDirection == ListSortDirection.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;
                }

                Sort(headerClicked.Content as string, direction == ListSortDirection.Ascending);

                _lastHeaderClicked = headerClicked;
                _lastDirection = direction;
                _sortAdorner = new SortAdorner(_lastHeaderClicked, direction, null);
                AdornerLayer.GetAdornerLayer(_lastHeaderClicked)?.Add(_sortAdorner);
            }
        }

        private void Sort(string sortBy, bool isAscending)
        {
            if (!ContentListView.HasItems) return;

            ListCollectionView view =
                (ListCollectionView)CollectionViewSource.GetDefaultView(ContentListView.ItemsSource);

            TrackInfoComparer comp;
            switch (sortBy)
            {
                case "#":
                    comp = new TrackInfoComparer(x => x.HasTrackNumber, isAscending);
                    break;

                case "Title":
                    comp = new TrackInfoComparer(x => x.HasTrackTitle, isAscending);
                    break;

                case "Artist":
                    comp = new TrackInfoComparer(x => x.HasTrackArtist, isAscending);
                    break;

                case "Album":
                    comp = new TrackInfoComparer(x => x.HasAlbumTitle, isAscending);
                    break;

                case "Album Artist":
                    comp = new TrackInfoComparer(x => x.HasAlbumArtist, isAscending);
                    break;

                case "Cover Art":
                    comp = new TrackInfoComparer(x => x.HasAlbumArt, isAscending);
                    break;

                case "Year":
                    comp = new TrackInfoComparer(x => x.HasAlbumYear, isAscending);
                    break;

                case "Bitrate":
                    comp = new TrackInfoComparer(x => x.HasDecentBitrate, isAscending);
                    break;

                default:
                    comp = new TrackInfoComparer(x => x.TrackInfo.FileName, isAscending);
                    break;
            }

            view.CustomSort = comp;
            ContentListView.Items.Refresh();
        }
    }
}
