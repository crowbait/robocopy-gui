using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public MainWindow()
        {
            InitializeComponent();
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
                if (File.Exists(fileName))
                {
                    operations.Clear();

                    //read lines in file
                    InputFilePath.Text = fileName;
                    List<string> operationStrings = new List<string>();
                    using (StreamReader reader = File.OpenText(fileName))
                    {
                        while (!reader.EndOfStream)
                        {
                            var readLine = reader.ReadLine();
                            //only read lines that are actually robocopy
                            if (readLine != null && readLine.StartsWith("robocopy")) 
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
                    GroupOperations.Visibility = Visibility.Visible;
                } else
                {
                    File.CreateText(fileName).Dispose();

                    //show operations group with empty list to allow adding of new operations
                }            
            }
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

                Button searchSource = new Button();
                searchSource.Content = "Search...";
                searchSource.HorizontalAlignment = HorizontalAlignment.Center;
                searchSource.VerticalAlignment = VerticalAlignment.Center;
                searchSource.Width = 100;
                searchSource.Tag = operationIndex;
                searchSource.Click += (s, e) =>
                {
                    Button sender = s as Button ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(sender.Tag);
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                    dialog.InitialDirectory = Path.GetDirectoryName(operations[index].SourceFolder);
                    dialog.IsFolderPicker = true;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        operations[index].SourceFolder = dialog.FileName;
                        TextBox source = GridOperations.FindName("source" + index) as TextBox ?? throw new Exception("Couldn't find appropriate TextBox");
                        source.Text = dialog.FileName;
                    }
                };
                Grid.SetColumn(searchSource, 0);
                Grid.SetRow(searchSource, operationIndex);

                TextBox source = new TextBox();
                source.Name = "source" + operationIndex;
                source.Text = operation.SourceFolder;
                source.VerticalAlignment = VerticalAlignment.Top;
                source.Margin = new Thickness(10, 10, 0, 0);
                source.TextWrapping = TextWrapping.NoWrap;
                Grid.SetColumn(source, 1);
                Grid.SetRow(source, operationIndex);

                Button searchDest = new Button();
                searchDest.Content = "Search...";
                searchDest.HorizontalAlignment = HorizontalAlignment.Center;
                searchDest.VerticalAlignment = VerticalAlignment.Center;
                searchDest.Width = 100;
                searchDest.Tag = operationIndex;
                searchDest.Click += (s, e) =>
                {
                    Button sender = s as Button ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(sender.Tag);
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                    dialog.InitialDirectory = Path.GetDirectoryName(operations[index].DestinationFolder);
                    dialog.IsFolderPicker = true;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        operations[index].DestinationFolder = dialog.FileName;
                        TextBox dest = GridOperations.FindName("dest" + index) as TextBox ?? throw new Exception("Couldn't find appropriate TextBox");
                        dest.Text = dialog.FileName;
                    }
                };
                Grid.SetColumn(searchDest, 2);
                Grid.SetRow(searchDest, operationIndex);

                TextBox dest = new TextBox();
                dest.Name = "dest" + operationIndex;
                dest.Text = operation.DestinationFolder;
                dest.VerticalAlignment = VerticalAlignment.Top;
                dest.Margin = new Thickness(10, 10, 0, 0);
                dest.TextWrapping = TextWrapping.NoWrap;
                Grid.SetColumn(dest, 3);
                Grid.SetRow(dest, operationIndex);

                Button exclFiles = new Button();
                exclFiles.Content = "Exclude\nFiles...";
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
                    }
                };
                Grid.SetColumn(exclFiles, 4);
                Grid.SetRow(exclFiles, operationIndex);

                Button exclFolders = new Button();
                exclFolders.Content = "Exclude\nFolders...";
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
                    }
                };
                Grid.SetColumn(exclFolders, 5);
                Grid.SetRow(exclFolders, operationIndex);

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
                Grid.SetColumn(remove, 6);
                Grid.SetRow(remove, operationIndex);

                GridOperations.Children.Add(searchSource);
                GridOperations.Children.Add(source);
                GridOperations.Children.Add(searchDest);
                GridOperations.Children.Add(dest);
                GridOperations.Children.Add(exclFiles);
                GridOperations.Children.Add(exclFolders);
                GridOperations.Children.Add(remove);


                GridOperations.RegisterName(source.Name, source);
                GridOperations.RegisterName(dest.Name, dest);
                registeredNames.Add(source.Name);
                registeredNames.Add(dest.Name);

                operationIndex++;
            }
        }
    }
}
