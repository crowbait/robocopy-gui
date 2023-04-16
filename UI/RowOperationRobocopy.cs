using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using System;

namespace robocopy_gui.UI {
  internal class RowOperationRobocopy {
    public int Index { get; set; }
    public Button SearchSourceButton { get; set; }
    public TextBox SourceText { get; set; }
    public Button SearchDestButton { get; set; }
    public TextBox DestText { get; set; }
    public Button ExclFilesButton { get; set; }
    public Button ExclFoldersButton { get; set; }
    public CheckBox Mirror { get; set; }
    public CheckBox Move { get; set; }
    public Button SettingsButton { get; set; }

    public RowOperationRobocopy(int operationIndex) {
      Index = operationIndex;

      PathIcon searchSourceIcon = new PathIcon{
        Data = Geometry.Parse(Icons.Search)
      };
      SearchSourceButton = new Button {
        Content = searchSourceIcon,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 60,
        Tag = operationIndex
      };
      Grid.SetColumn(SearchSourceButton, 1);

      SourceText = new TextBox {
        Name = "source",
        Watermark = "Source",
        Text = MainWindow.OperationsList[operationIndex].SourceFolder,
        VerticalAlignment = VerticalAlignment.Top,
        Margin = new Thickness(0, 5, 0, 0),
        TextWrapping = TextWrapping.NoWrap,
        Tag = operationIndex
      };
      Grid.SetColumn(SourceText, 2);

      PathIcon searchDestIcon = new PathIcon {
        Data = Geometry.Parse(Icons.Search)
      };
      SearchDestButton = new Button {
        Content = searchDestIcon,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 60,
        Tag = operationIndex
      };
      Grid.SetColumn(SearchDestButton, 3);

      DestText = new TextBox {
        Name = "dest",
        Watermark = "Destination",
        Text = MainWindow.OperationsList[operationIndex].DestinationFolder,
        VerticalAlignment = VerticalAlignment.Top,
        Margin = new Thickness(0, 5, 0, 0),
        TextWrapping = TextWrapping.NoWrap,
        Tag = operationIndex
      };
      Grid.SetColumn(DestText, 4);

      ExclFilesButton = new Button {
        Content = "Exclude\nFiles: " + MainWindow.OperationsList[operationIndex].ExcludeFiles.Count,
        FontSize = 10,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 100,
        Tag = operationIndex
      };
      ExclFilesButton.Click += async (s, e) => {
        Button sender = s as Button ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(sender.Tag);
        DialogExclusions dialog = new DialogExclusions(MainWindow.OperationsList[index].ExcludeFiles);
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
          await dialog.ShowDialog(desktop.MainWindow);
        }
        if (!dialog.IsCancel) {
          MainWindow.OperationsList[index].ExcludeFiles = dialog.ReturnExclusions;
          sender.Content = "Exclude\nFiles: " + dialog.ReturnExclusions.Count;
        }
      };
      Grid.SetColumn(ExclFilesButton, 5);

      ExclFoldersButton = new Button {
        Content = "Exclude\nFolders: " + MainWindow.OperationsList[operationIndex].ExcludeFolders.Count,
        FontSize = 10,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 100,
        Tag = operationIndex
      };
      ExclFoldersButton.Click += async (s, e) => {
        Button sender = s as Button ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(sender.Tag);
        DialogExclusions dialog = new DialogExclusions(MainWindow.OperationsList[index].ExcludeFolders);
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
          await dialog.ShowDialog(desktop.MainWindow);
        }
        if (!dialog.IsCancel) {
          MainWindow.OperationsList[index].ExcludeFolders = dialog.ReturnExclusions;
          sender.Content = "Exclude\nFolders: " + dialog.ReturnExclusions.Count;
        }
      };
      Grid.SetColumn(ExclFoldersButton, 6);

      Mirror = new CheckBox {
        Name = "mirror",
        Content = "Mirror",
        IsChecked = MainWindow.OperationsList[operationIndex].IsMirror,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Margin = new Thickness(0, 0, 5, 0),
        Tag = operationIndex
      };
      Mirror.SetValue(ToolTip.TipProperty,
        "Copy files to destination folder, removing files from the destination that are not present in the source folder.");
      Mirror.Unchecked += (sender, e) => {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        MainWindow.OperationsList[index].IsMirror = false;
      };
      Grid.SetColumn(Mirror, 7);

      Move = new CheckBox {
        Name = "move",
        Content = "Move",
        IsChecked = MainWindow.OperationsList[operationIndex].IsMove,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Margin = new Thickness(0,0,5,0),
        Tag = operationIndex
      };
      Move.SetValue(ToolTip.TipProperty,
        "Move files to the destination folder rather than copying them, removing them from the source folder.");
      Move.Unchecked += (sender, e) => {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        MainWindow.OperationsList[index].IsMove = false;
      };
      Grid.SetColumn(Move, 8);

      PathIcon settingsIcon = new PathIcon {
        Data = Geometry.Parse(Icons.Settings)
      };
      SettingsButton = new Button {
        Content = settingsIcon,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 60,
        Tag = operationIndex
      };
      SettingsButton.Click += async (s, e) => {
        Button sender = s as Button ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(sender.Tag);
        DialogRobocopySettings dialog = new DialogRobocopySettings(MainWindow.OperationsList[index]);
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
          await dialog.ShowDialog(desktop.MainWindow);
        }
        if (!dialog.IsCancel) {
          MainWindow.OperationsList[index].IsOnlyIfNewer = dialog.OnlyNewer;
          MainWindow.OperationsList[index].IsUseFATTime = dialog.FATTime;
          MainWindow.OperationsList[index].IsLoggingEnabled = dialog.LoggingEnabled;
          MainWindow.OperationsList[index].IsLoggingFiles = dialog.LogFiles;
          MainWindow.OperationsList[index].IsLoggingFolders = dialog.LogFolders;
          MainWindow.OperationsList[index].IsLoggingJobHeader = dialog.LogHeader;
          MainWindow.OperationsList[index].IsLoggingJobSummary = dialog.LogSummary;
          MainWindow.OperationsList[index].IsLoggingProgress = dialog.LogProgress;
          MainWindow.OperationsList[index].IsLoggingSize = dialog.LogSizes;
          MainWindow.OperationsList[index].RetryCount = dialog.RetryCount;
          MainWindow.OperationsList[index].MultiThreadCount = dialog.MultiThread;
        }
      };
      Grid.SetColumn(SettingsButton, 9);
    }
  }
}
