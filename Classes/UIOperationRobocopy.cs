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
      SearchSourceButton = new Button();
      SourceText = new TextBox();
      SearchDestButton = new Button();
      DestText = new TextBox();
      ExclFilesButton = new Button();
      ExclFoldersButton = new Button();
      Mirror = new CheckBox();
      Move = new CheckBox();
      OnlyNewer = new CheckBox();
      FATFileTime = new CheckBox();

      SearchSourceButton.Content = "Search...";
      SearchSourceButton.HorizontalAlignment = HorizontalAlignment.Center;
      SearchSourceButton.VerticalAlignment = VerticalAlignment.Center;
      SearchSourceButton.Width = 100;
      SearchSourceButton.Tag = operationIndex;

      SourceText.Name = "source" + operationIndex;
      SourceText.Text = MainWindow.OperationsList[operationIndex].SourceFolder;
      SourceText.VerticalAlignment = VerticalAlignment.Top;
      SourceText.Margin = new Thickness(10, 10, 0, 0);
      SourceText.TextWrapping = TextWrapping.NoWrap;
      SourceText.Tag = operationIndex;

      SearchDestButton.Content = "Search...";
      SearchDestButton.HorizontalAlignment = HorizontalAlignment.Center;
      SearchDestButton.VerticalAlignment = VerticalAlignment.Center;
      SearchDestButton.Width = 100;
      SearchDestButton.Tag = operationIndex;

      DestText.Name = "dest" + operationIndex;
      DestText.Text = MainWindow.OperationsList[operationIndex].DestinationFolder;
      DestText.VerticalAlignment = VerticalAlignment.Top;
      DestText.Margin = new Thickness(10, 10, 0, 0);
      DestText.TextWrapping = TextWrapping.NoWrap;
      DestText.Tag = operationIndex;

      ExclFilesButton.Content = "Exclude\nFiles: " + MainWindow.OperationsList[operationIndex].ExcludeFiles.Count;
      ExclFilesButton.FontSize = 10;
      ExclFilesButton.HorizontalAlignment = HorizontalAlignment.Center;
      ExclFilesButton.VerticalAlignment = VerticalAlignment.Center;
      ExclFilesButton.Width = 100;
      ExclFilesButton.Tag = operationIndex;
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

      ExclFoldersButton.Content = "Exclude\nFolders: " + MainWindow.OperationsList[operationIndex].ExcludeFolders.Count;
      ExclFoldersButton.FontSize = 10;
      ExclFoldersButton.HorizontalAlignment = HorizontalAlignment.Center;
      ExclFoldersButton.VerticalAlignment = VerticalAlignment.Center;
      ExclFoldersButton.Width = 100;
      ExclFoldersButton.Tag = operationIndex;
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

      Mirror.Name = "mirror" + operationIndex;
      Mirror.Content = "Mirror";
      Mirror.IsChecked = MainWindow.OperationsList[operationIndex].IsMirror;
      Mirror.ToolTip = "Copy files to destination folder, removing files from the destination that are not present in the source folder.";
      Mirror.HorizontalAlignment = HorizontalAlignment.Center;
      Mirror.VerticalAlignment = VerticalAlignment.Center;
      Mirror.Tag = operationIndex;
      Mirror.Unchecked += (sender, e) =>
      {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        MainWindow.OperationsList[index].IsMirror = false;
      };

      Move.Name = "move" + operationIndex;
      Move.Content = "Move";
      Move.IsChecked = MainWindow.OperationsList[operationIndex].IsMove;
      Move.ToolTip = "Move files to the destination folder rather than copying them, removing them from the source folder.";
      Move.HorizontalAlignment = HorizontalAlignment.Center;
      Move.VerticalAlignment = VerticalAlignment.Center;
      Move.Tag = operationIndex;
      Move.Unchecked += (sender, e) =>
      {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        MainWindow.OperationsList[index].IsMove = false;
      };

      OnlyNewer.Content = "Only newer";
      OnlyNewer.IsChecked = MainWindow.OperationsList[operationIndex].IsOnlyIfNewer;
      OnlyNewer.ToolTip = "Copy or move files to the destination folder only if the source file is newer than the target file.";
      OnlyNewer.HorizontalAlignment = HorizontalAlignment.Center;
      OnlyNewer.VerticalAlignment = VerticalAlignment.Center;
      OnlyNewer.Tag = operationIndex;
      OnlyNewer.Unchecked += (sender, e) =>
      {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        MainWindow.OperationsList[index].IsOnlyIfNewer = false;
      };

      FATFileTime.Name = "FATtime" + operationIndex;
      FATFileTime.Content = "FAT-Time";
      FATFileTime.IsChecked = MainWindow.OperationsList[operationIndex].IsOnlyIfNewer;
      FATFileTime.ToolTip = "Use FAT-style time format when writing file. Useful when copying to another file system and when using \"only newer files\".";
      FATFileTime.HorizontalAlignment = HorizontalAlignment.Center;
      FATFileTime.VerticalAlignment = VerticalAlignment.Center;
      FATFileTime.Tag = operationIndex;
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
