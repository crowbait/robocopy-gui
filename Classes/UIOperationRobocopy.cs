using System;
using System.Windows;
using System.Windows.Controls;

namespace robocopy_gui.Classes
{
  internal class UIOperationRobocopy
  {
    public int Index { get; set;}
    public Button SearchSourceButton { get; set;}
    public TextBox SourceText { get; set;}
    public Button SearchDestButton { get; set;}
    public TextBox DestText { get; set;}
    public Button ExclFilesButton { get; set;}
    public Button ExclFoldersButton { get; set;}
    public CheckBox Mirror { get; set;}
    public CheckBox Move { get; set;}
    public CheckBox OnlyNewer { get; set;}
    public CheckBox FATFileTime { get; set;}

    public UIOperationRobocopy(int operationIndex)
    {
      Index = operationIndex;

      SearchSourceButton = new Button
      {
        Content = "Search...",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 100,
        Tag = operationIndex
      };

      SourceText = new TextBox
      {
        Name = "source" + operationIndex,
        Text = MainWindow.OperationsList[operationIndex].SourceFolder,
        VerticalAlignment = VerticalAlignment.Top,
        Margin = new Thickness(10, 10, 0, 0),
        TextWrapping = TextWrapping.NoWrap,
        Tag = operationIndex
      };

      SearchDestButton = new Button
      {
        Content = "Search...",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 100,
        Tag = operationIndex
      };

      DestText = new TextBox
      {
        Name = "dest" + operationIndex,
        Text = MainWindow.OperationsList[operationIndex].DestinationFolder,
        VerticalAlignment = VerticalAlignment.Top,
        Margin = new Thickness(10, 10, 0, 0),
        TextWrapping = TextWrapping.NoWrap,
        Tag = operationIndex
      };

      ExclFilesButton = new Button
      {
        Content = "Exclude\nFiles: " + MainWindow.OperationsList[operationIndex].ExcludeFiles.Count,
        FontSize = 10,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 100,
        Tag = operationIndex
      };
      ExclFilesButton.Click += (s, e) =>
      {
        Button sender = s as Button ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(sender.Tag);
        DialogExclusions dialog = new DialogExclusions(MainWindow.OperationsList[index].ExcludeFiles);
        dialog.ShowDialog();

        //handle return
        if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
        {
          MainWindow.OperationsList[index].ExcludeFiles = dialog.returnExclusions;
          sender.Content = "Exclude\nFiles: " + dialog.returnExclusions.Count;
        }
      };

      ExclFoldersButton = new Button
      {
        Content = "Exclude\nFolders: " + MainWindow.OperationsList[operationIndex].ExcludeFolders.Count,
        FontSize = 10,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 100,
        Tag = operationIndex
      };
      ExclFoldersButton.Click += (s, e) =>
      {
        Button sender = s as Button ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(sender.Tag);
        DialogExclusions dialog = new DialogExclusions(MainWindow.OperationsList[index].ExcludeFolders);
        dialog.ShowDialog();

        //handle return
        if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
        {
          MainWindow.OperationsList[index].ExcludeFolders = dialog.returnExclusions;
          sender.Content = "Exclude\nFolders: " + dialog.returnExclusions.Count;
        }
      };

      Mirror = new CheckBox
      {
        Name = "mirror" + operationIndex,
        Content = "Mirror",
        IsChecked = MainWindow.OperationsList[operationIndex].IsMirror,
        ToolTip = "Copy files to destination folder, removing files from the destination that are not present in the source folder.",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Tag = operationIndex
      };
      Mirror.Unchecked += (sender, e) =>
      {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        MainWindow.OperationsList[index].IsMirror = false;
      };

      Move = new CheckBox
      {
        Name = "move" + operationIndex,
        Content = "Move",
        IsChecked = MainWindow.OperationsList[operationIndex].IsMove,
        ToolTip = "Move files to the destination folder rather than copying them, removing them from the source folder.",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Tag = operationIndex
      };
      Move.Unchecked += (sender, e) =>
      {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        MainWindow.OperationsList[index].IsMove = false;
      };

      OnlyNewer = new CheckBox
      {
        Content = "Only newer",
        IsChecked = MainWindow.OperationsList[operationIndex].IsOnlyIfNewer,
        ToolTip = "Copy or move files to the destination folder only if the source file is newer than the target file.",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Tag = operationIndex
      };
      OnlyNewer.Unchecked += (sender, e) =>
      {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        MainWindow.OperationsList[index].IsOnlyIfNewer = false;
      };

      FATFileTime = new CheckBox
      {
        Name = "FATtime" + operationIndex,
        Content = "FAT-Time",
        IsChecked = MainWindow.OperationsList[operationIndex].IsOnlyIfNewer,
        ToolTip = "Use FAT-style time format when writing file. Useful when copying to another file system and when using \"only newer files\".",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Tag = operationIndex
      };
      FATFileTime.Checked += (sender, e) =>
      {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        MainWindow.OperationsList[index].IsUseFATTime = true;
      };
      FATFileTime.Unchecked += (sender, e) =>
      {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        MainWindow.OperationsList[index].IsUseFATTime = false;
      };
    }
  }
}
