using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using robocopy_gui.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;

namespace robocopy_gui
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
    {
        private List<Operation> operations = new List<Operation>();
        private List<string> registeredNames = new List<string>();
        private string currentFile = "";
        private string scriptTitle = "Backup";

        public MainWindow()
        {
            InitializeComponent();
            MainWindow1.Width = Properties.Settings.Default.MainWindowWidth;
            MainWindow1.Height = Properties.Settings.Default.MainWindowHeight;
            string lastFile = Properties.Settings.Default.LastFile;
            if( !string.IsNullOrWhiteSpace(lastFile) )
            {
                if(File.Exists(lastFile))
                {
                    InputFilePath.Text = lastFile;
                    currentFile = lastFile;
                    ButtonCommit.IsEnabled = true;
                    readFile();
                }
            }
        }

        private void ButtonPickFile_Click(object sender, RoutedEventArgs e)
        {
            //pick batch file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = false;
            openFileDialog.Filter = "Batch Files (*.bat)|*.bat";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                currentFile = fileName;
                InputFilePath.Text = currentFile;
                if (File.Exists(fileName))
                {
                    readFile();
                } else
                {
                    File.CreateText(fileName).Dispose();
                    //show operations group with empty list to allow adding of new operations
                    clearOperationsList();
                    renderList();
                }
                ButtonCommit.IsEnabled = true;
            }
        }

        private void InputFilePath_LostFocus(object sender, RoutedEventArgs e)
        {
            if(currentFile != InputFilePath.Text)
            {
                string fileName = InputFilePath.Text;
                if (File.Exists(fileName))
                {
                    currentFile = fileName;
                    readFile();
                    ButtonCommit.IsEnabled = true;
                }
                else
                {
                    if (MessageBox.Show(
                            "The file / path you've entered does not exist.\nDo you want to create it?",
                            "File doesn't exist",
                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        File.CreateText(fileName).Dispose();
                        currentFile = fileName;
                        //show operations group with empty list to allow adding of new operations
                        clearOperationsList();
                        renderList();
                        ButtonCommit.IsEnabled = true;
                    }
                    else
                    {
                        InputFilePath.Text = currentFile;
                    }
                }
            }
        }
        private void InputFilePath_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                InputFilePath_LostFocus (sender, e );
            }
        }
        private void InputFile_Drop(object sender, DragEventArgs e)
        {
            string fileName = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            currentFile = fileName;
            InputFilePath.Text = currentFile;
            if (File.Exists(fileName))
            {
                readFile();
                ButtonCommit.IsEnabled = true;
            }
        }

        private void OperationTextBoxSource_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox s = sender as TextBox ?? throw new Exception("Sender is null");
            int index = Convert.ToInt32(s.Tag);
            operations[index].SourceFolder = s.Text;
            operations[index].Name = operations[index].CreateName();
        }
        private void OperationTextBoxDest_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox s = sender as TextBox ?? throw new Exception("Sender is null");
            int index = Convert.ToInt32(s.Tag);
            operations[index].DestinationFolder = s.Text;
            operations[index].Name = operations[index].CreateName();
        }
        private void clearOperationsList()
        {
            operations = new List<Operation>();
            foreach (string item in registeredNames)
            {
                GridOperations.UnregisterName(item);
            }
            registeredNames.Clear();
            foreach (UIElement control in GridOperations.Children)
            {
                if(control is TextBox)
                {
                    control.LostFocus -= OperationTextBoxSource_LostFocus;
                    control.LostFocus -= OperationTextBoxDest_LostFocus;
                }
            }
        }

        private void readFile()
        {
            operations.Clear();

            //read lines in file
            List<string> operationStrings = new List<string>();
            using (StreamReader reader = File.OpenText(currentFile))
            {
                while (!reader.EndOfStream)
                {
                    var readLine = reader.ReadLine();
                    //only read lines that are actually robocopy
                    if (readLine != null && (
                        readLine.ToLower().StartsWith("robocopy") ||
                        readLine.ToLower().StartsWith("rem robocopy")
                        ))
                    {
                        operationStrings.Add(readLine);
                    }
                }
                reader.Close();
            }

            //interpret all read lines as operations
            foreach (string operation in operationStrings)
            {
                operations.Add(new Operation(operation));
            }

            //display rows of operations
            renderList();
        }
        private void renderList()
        {
            GridOperations.RowDefinitions.Clear();
            GridOperations.Children.Clear();

            foreach (string item in registeredNames)
            {
                GridOperations.UnregisterName(item);
            }
            registeredNames.Clear();

            int operationIndex = 0;
            foreach (Operation operation in operations)
            {
                RowDefinition newRow = new RowDefinition();
                newRow.Height = new GridLength(42);
                GridOperations.RowDefinitions.Add(newRow);

                CheckBox enabled = new CheckBox();
                enabled.Content = "Enabled";
                enabled.IsChecked = operation.enabled;
                enabled.HorizontalAlignment = HorizontalAlignment.Center;
                enabled.VerticalAlignment = VerticalAlignment.Center;
                enabled.Tag = operationIndex;
                enabled.Checked += (sender, e) =>
                {
                    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    operations[index].enabled = true;
                };
                enabled.Unchecked += (sender, e) =>
                {
                    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    operations[index].enabled = false;
                };
                Grid.SetColumn(enabled, 0);
                Grid.SetRow(enabled, operationIndex);

                Button searchSource = new Button();
                searchSource.Content = "Search...";
                searchSource.HorizontalAlignment = HorizontalAlignment.Center;
                searchSource.VerticalAlignment = VerticalAlignment.Center;
                searchSource.Width = 100;
                searchSource.Tag = operationIndex;
                searchSource.Click += (sender, e) =>
                {
                    Button s = sender as Button ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                    dialog.InitialDirectory = Path.GetDirectoryName(operations[index].SourceFolder);
                    dialog.IsFolderPicker = true;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        operations[index].SourceFolder = dialog.FileName;
                        operations[index].Name = operations[index].CreateName();
                        TextBox source = GridOperations.FindName("source" + index) as TextBox ?? throw new Exception("Couldn't find appropriate TextBox");
                        source.Text = dialog.FileName;
                    }
                };
                Grid.SetColumn(searchSource, 1);
                Grid.SetRow(searchSource, operationIndex);

                TextBox source = new TextBox();
                source.Name = "source" + operationIndex;
                source.Text = operation.SourceFolder;
                source.VerticalAlignment = VerticalAlignment.Top;
                source.Margin = new Thickness(10, 10, 0, 0);
                source.TextWrapping = TextWrapping.NoWrap;
                source.Tag = operationIndex;
                source.LostFocus += OperationTextBoxSource_LostFocus;
                Grid.SetColumn(source, 2);
                Grid.SetRow(source, operationIndex);

                Button searchDest = new Button();
                searchDest.Content = "Search...";
                searchDest.HorizontalAlignment = HorizontalAlignment.Center;
                searchDest.VerticalAlignment = VerticalAlignment.Center;
                searchDest.Width = 100;
                searchDest.Tag = operationIndex;
                searchDest.Click += (sender, e) =>
                {
                    Button s = sender as Button ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                    dialog.InitialDirectory = Path.GetDirectoryName(operations[index].DestinationFolder);
                    dialog.IsFolderPicker = true;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        operations[index].DestinationFolder = dialog.FileName;
                        operations[index].Name = operations[index].CreateName();
                        TextBox dest = GridOperations.FindName("dest" + index) as TextBox ?? throw new Exception("Couldn't find appropriate TextBox");
                        dest.Text = dialog.FileName;
                    }
                };
                Grid.SetColumn(searchDest, 3);
                Grid.SetRow(searchDest, operationIndex);

                TextBox dest = new TextBox();
                dest.Name = "dest" + operationIndex;
                dest.Text = operation.DestinationFolder;
                dest.VerticalAlignment = VerticalAlignment.Top;
                dest.Margin = new Thickness(10, 10, 0, 0);
                dest.TextWrapping = TextWrapping.NoWrap;
                dest.Tag = operationIndex;
                dest.LostFocus += OperationTextBoxDest_LostFocus;
                Grid.SetColumn(dest, 4);
                Grid.SetRow(dest, operationIndex);

                Button exclFiles = new Button();
                exclFiles.Content = "Exclude\nFiles: " + operation.ExcludeFiles.Count;
                exclFiles.FontSize = 10;
                exclFiles.HorizontalAlignment = HorizontalAlignment.Center;
                exclFiles.VerticalAlignment = VerticalAlignment.Center;
                exclFiles.Width = 100;
                exclFiles.Tag = operationIndex;
                exclFiles.Click += (s, e) =>
                {
                    Button sender = s as Button ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(sender.Tag);
                    DialogExclusions dialog = new DialogExclusions(operations[index].ExcludeFiles);
                    dialog.ShowDialog();

                    //handle return
                    if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                    {
                        operations[index].ExcludeFiles = dialog.returnExclusions;
                        sender.Content = "Exclude\nFiles: " + dialog.returnExclusions.Count;
                    }
                };
                Grid.SetColumn(exclFiles, 5);
                Grid.SetRow(exclFiles, operationIndex);

                Button exclFolders = new Button();
                exclFolders.Content = "Exclude\nFolders: " + operation.ExcludeFolders.Count;
                exclFolders.FontSize = 10;
                exclFolders.HorizontalAlignment = HorizontalAlignment.Center;
                exclFolders.VerticalAlignment = VerticalAlignment.Center;
                exclFolders.Width = 100;
                exclFolders.Tag = operationIndex;
                exclFolders.Click += (s, e) =>
                {
                    Button sender = s as Button ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(sender.Tag);
                    DialogExclusions dialog = new DialogExclusions(operations[index].ExcludeFolders);
                    dialog.ShowDialog();

                    //handle return
                    if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                    {
                        operations[index].ExcludeFolders = dialog.returnExclusions;
                        sender.Content = "Exclude\nFolders: " + dialog.returnExclusions.Count;
                    }
                };
                Grid.SetColumn(exclFolders, 6);
                Grid.SetRow(exclFolders, operationIndex);

                CheckBox mirror = new CheckBox();
                mirror.Name = "mirror" + operationIndex;
                mirror.Content = "Mirror";
                mirror.IsChecked = operation.mirror;
                mirror.ToolTip = "Copy files to destination folder, removing files from the destination that are not present in the source folder.";
                mirror.HorizontalAlignment = HorizontalAlignment.Center;
                mirror.VerticalAlignment = VerticalAlignment.Center;
                mirror.Tag = operationIndex;
                mirror.Checked += (sender, e) =>
                {
                    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    operations[index].mirror = true;
                    CheckBox move = GridOperations.FindName("move" + index) as CheckBox ?? throw new Exception("Couldn't find appropriate CheckBox");
                    move.IsChecked = false;
                };
                mirror.Unchecked += (sender, e) =>
                {
                    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    operations[index].mirror = false;
                };
                Grid.SetColumn(mirror, 7);
                Grid.SetRow(mirror, operationIndex);

                CheckBox move = new CheckBox();
                move.Name = "move" + operationIndex;
                move.Content = "Move";
                move.IsChecked = operation.move;
                move.ToolTip = "Move files to the destination folder rather than copying them, removing them from the source folder.";
                move.HorizontalAlignment = HorizontalAlignment.Center;
                move.VerticalAlignment = VerticalAlignment.Center;
                move.Tag = operationIndex;
                move.Checked += (sender, e) =>
                {
                    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    operations[index].move = true;
                    CheckBox mirror = GridOperations.FindName("mirror" + index) as CheckBox ?? throw new Exception("Couldn't find appropriate CheckBox");
                    mirror.IsChecked = false;
                };
                move.Unchecked += (sender, e) =>
                {
                    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    operations[index].move = false;
                };
                Grid.SetColumn(move, 8);
                Grid.SetRow(move, operationIndex);

                CheckBox onlyNewer = new CheckBox();
                onlyNewer.Content = "Only newer";
                onlyNewer.IsChecked = operation.onlyIfNewer;
                onlyNewer.ToolTip = "Copy or move files to the destination folder only if the source file is newer than the target file.";
                onlyNewer.HorizontalAlignment = HorizontalAlignment.Center;
                onlyNewer.VerticalAlignment = VerticalAlignment.Center;
                onlyNewer.Tag = operationIndex;
                onlyNewer.Checked += (sender, e) =>
                {
                    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    operations[index].onlyIfNewer = true;
                    CheckBox fatFileTime = GridOperations.FindName("FATtime" + index) as CheckBox ?? throw new Exception("Couldn't find appropriate CheckBox");
                    fatFileTime.IsChecked = true;
                };
                onlyNewer.Unchecked += (sender, e) =>
                {
                    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    operations[index].onlyIfNewer = false;
                };
                Grid.SetColumn(onlyNewer, 9);
                Grid.SetRow(onlyNewer, operationIndex);

                CheckBox fatFileTime = new CheckBox();
                fatFileTime.Name = "FATtime" + operationIndex;
                fatFileTime.Content = "FAT-Time";
                fatFileTime.IsChecked = operation.onlyIfNewer;
                fatFileTime.ToolTip = "Use FAT-style time format when writing file. Useful when copying to another file system and when using \"only newer files\".";
                fatFileTime.HorizontalAlignment = HorizontalAlignment.Center;
                fatFileTime.VerticalAlignment = VerticalAlignment.Center;
                fatFileTime.Tag = operationIndex;
                fatFileTime.Checked += (sender, e) =>
                {
                    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    operations[index].useFATTime = true;
                };
                fatFileTime.Unchecked += (sender, e) =>
                {
                    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(s.Tag);
                    operations[index].useFATTime = false;
                };
                Grid.SetColumn(fatFileTime, 10);
                Grid.SetRow(fatFileTime, operationIndex);

                Button remove = new Button();
                remove.Content = "-";
                remove.HorizontalAlignment = HorizontalAlignment.Center;
                remove.VerticalAlignment = VerticalAlignment.Center;
                remove.Width = 60;
                remove.Tag = operationIndex;
                remove.Click += (s, e) =>
                {

                    Button sender = s as Button ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(sender.Tag);
                    operations.RemoveAt(index);
                    renderList();
                };
                Grid.SetColumn(remove, 11);
                Grid.SetRow(remove, operationIndex);

                GridOperations.Children.Add(enabled);
                GridOperations.Children.Add(searchSource);
                GridOperations.Children.Add(source);
                GridOperations.Children.Add(searchDest);
                GridOperations.Children.Add(dest);
                GridOperations.Children.Add(exclFiles);
                GridOperations.Children.Add(exclFolders);
                GridOperations.Children.Add(mirror);
                GridOperations.Children.Add(move);
                GridOperations.Children.Add(onlyNewer);
                GridOperations.Children.Add(fatFileTime);
                GridOperations.Children.Add(remove);


                GridOperations.RegisterName(source.Name, source);
                GridOperations.RegisterName(dest.Name, dest);
                GridOperations.RegisterName(mirror.Name, mirror);
                GridOperations.RegisterName(move.Name, move);
                GridOperations.RegisterName(fatFileTime.Name, fatFileTime);
                registeredNames.Add(source.Name);
                registeredNames.Add(dest.Name);
                registeredNames.Add(mirror.Name);
                registeredNames.Add(move.Name);
                registeredNames.Add(fatFileTime.Name);

                operationIndex++;
            }

            RowDefinition addRow = new RowDefinition();
            addRow.Height = new GridLength(42);
            GridOperations.RowDefinitions.Add(addRow);

            Button add = new Button();
            add.Content = "+";
            add.HorizontalAlignment = HorizontalAlignment.Center;
            add.VerticalAlignment = VerticalAlignment.Center;
            add.Width = 60;
            add.Click += (s, e) =>
            {
                operations.Add(new Operation(string.Empty, string.Empty));
                renderList();
            };
            Grid.SetColumn(add, 11);
            Grid.SetRow(add, operationIndex);

            GridOperations.Children.Add(add);
            GroupOperations.Visibility = Visibility.Visible;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.MainWindowWidth = MainWindow1.Width;
            Properties.Settings.Default.MainWindowHeight = MainWindow1.Height;
            Properties.Settings.Default.LastFile = currentFile;
            Properties.Settings.Default.Save();
        }

        private void ButtonCommit_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter file;
            if (!File.Exists(currentFile))
            {
                file = File.CreateText(currentFile);
            } else
            {
                file = new StreamWriter(currentFile);
            }
            file.WriteLine("echo off");
            file.WriteLine("title " + scriptTitle);

            foreach (Operation item in operations)
            {
                if( !string.IsNullOrWhiteSpace(item.SourceFolder) && !string.IsNullOrWhiteSpace(item.DestinationFolder) )
                {
                    file.WriteLine();
                    if(!item.enabled)
                    {
                        file.Write("REM ");
                    }
                    file.WriteLine("echo " + item.Name);
                    file.WriteLine(item.Command());
                }
            }
            file.Close();
        }
    }
}
