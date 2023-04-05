using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using robocopy_gui.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace robocopy_gui
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public static List<Operation> OperationsList = new List<Operation>();
    private List<string> registeredNames = new List<string>();
    private string currentFile = "";
    private string scriptTitle = "Backup";

    public MainWindow()
    {
      InitializeComponent();
      MainWindow1.Width = Properties.Settings.Default.MainWindowWidth;
      MainWindow1.Height = Properties.Settings.Default.MainWindowHeight;
      string lastFile = Properties.Settings.Default.LastFile;
      if (!string.IsNullOrWhiteSpace(lastFile))
      {
        if (File.Exists(lastFile))
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
        }
        else
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
      if (currentFile != InputFilePath.Text)
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
        InputFilePath_LostFocus(sender, e);
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

    /*                      ROUTINES FOR GENERATED OPERATION ROW UI ELEMENTS
     * ===============================================================================
     * ===============================================================================
     */

    private void OperationButtonSearchSource_Click(object sender, RoutedEventArgs e)
    {
      Button s = sender as Button ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      CommonOpenFileDialog dialog = new CommonOpenFileDialog();
      dialog.InitialDirectory = Path.GetDirectoryName(MainWindow.OperationsList[index].SourceFolder);
      dialog.IsFolderPicker = true;
      if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
      {
        OperationsList[index].SourceFolder = dialog.FileName;
        OperationsList[index].Name = OperationsList[index].CreateName();
        TextBox source = GridOperations.FindName("source" + index) as TextBox ?? throw new Exception("Couldn't find appropriate TextBox");
        source.Text = dialog.FileName;
      }
    }
    private void OperationButtonSearchDest_Click(object sender, RoutedEventArgs args)
    {
      Button s = sender as Button ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      CommonOpenFileDialog dialog = new CommonOpenFileDialog();
      dialog.InitialDirectory = Path.GetDirectoryName(OperationsList[index].DestinationFolder);
      dialog.IsFolderPicker = true;
      if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
      {
        OperationsList[index].DestinationFolder = dialog.FileName;
        OperationsList[index].Name = OperationsList[index].CreateName();
        TextBox dest = GridOperations.FindName("dest" + index) as TextBox ?? throw new Exception("Couldn't find appropriate TextBox");
        dest.Text = dialog.FileName;
      }
    }
    private void OperationTextBoxSource_LostFocus(object sender, RoutedEventArgs e)
    {
      TextBox s = sender as TextBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].SourceFolder = s.Text;
      OperationsList[index].Name = OperationsList[index].CreateName();
    }
    private void OperationTextBoxDest_LostFocus(object sender, RoutedEventArgs e)
    {
      TextBox s = sender as TextBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].DestinationFolder = s.Text;
      OperationsList[index].Name = OperationsList[index].CreateName();
    }
    private void OperationTextBoxCommand_LostFocus(object sender, RoutedEventArgs e)
    {
      TextBox s = sender as TextBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].arbitraryCommand = s.Text;
    }
    private void OperationCheckMirror_enable(object sender, RoutedEventArgs e)
    {
      CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].mirror = true;
      CheckBox move = GridOperations.FindName("move" + index) as CheckBox ?? throw new Exception("Couldn't find appropriate CheckBox");
      move.IsChecked = false;
    }
    private void OperationCheckMove_enable(object sender, RoutedEventArgs e)
    {
      CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].move = true;
      CheckBox mirror = GridOperations.FindName("mirror" + index) as CheckBox ?? throw new Exception("Couldn't find appropriate CheckBox");
      mirror.IsChecked = false;
    }
    private void OperationCheckOnlyNewer_enable(object sender, RoutedEventArgs e)
    {
      CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].onlyIfNewer = true;
      CheckBox fatFileTime = GridOperations.FindName("FATtime" + index) as CheckBox ?? throw new Exception("Couldn't find appropriate CheckBox");
      fatFileTime.IsChecked = true;
    }


    /*                      ROUTINES FOR READING AND PARSING FILE
     * ===============================================================================
     * ===============================================================================
     */


    private void clearOperationsList()
    {
      OperationsList.Clear();
      foreach (string item in registeredNames)
      {
        GridOperations.UnregisterName(item);
      }
      registeredNames.Clear();
      foreach (UIElement control in GridOperations.Children)
      {
        if (control is TextBox) // this prevents LostFocus events from firing for non-existing elements after the list is cleared or re-populated
        {
          control.LostFocus -= OperationTextBoxSource_LostFocus;
          control.LostFocus -= OperationTextBoxDest_LostFocus;
        }
      }
    }

    private void readFile()
    {
      OperationsList.Clear();

      //read lines in file
      List<string> operationStrings = new List<string>();
      using (StreamReader reader = File.OpenText(currentFile))
      {
        int skipFirstLines = 2; // 0 to disable - designed to skip echo off and title XYZ at the beginning of file
        for (int i = 0; i < skipFirstLines; i++)
        {
          reader.ReadLine();
        }
        while (!reader.EndOfStream)
        { 
          var readLine = reader.ReadLine();
          if (readLine != null && !string.IsNullOrWhiteSpace(readLine))
          {
            operationStrings.Add(readLine);
          }
        }
        reader.Close();
      }

      //interpret all read lines as operations
      foreach (string operation in operationStrings)
      {
        if(operation.ToLower().StartsWith("robocopy") || operation.ToLower().StartsWith("rem robocopy") ) {
          OperationsList.Add(new Operation(operation));
        } else if(operation.ToLower().StartsWith("rem ") && !operation.ToLower().StartsWith("rem echo") )
        {
          OperationsList.Add(new Operation(true, false, operation.Substring(3)));
        } else if (!operation.ToLower().StartsWith("echo"))
        {
          OperationsList.Add(new Operation(true, true, operation));
        }
      }

      //display rows of operations
      renderList();
    }
    public void renderList()
    {
      GridOperations.RowDefinitions.Clear();
      GridOperations.Children.Clear();

      foreach (string item in registeredNames)
      {
        GridOperations.UnregisterName(item);
      }
      registeredNames.Clear();

      int operationIndex = 0;
      foreach (Operation operation in OperationsList)
      {
        RowDefinition newRow = new RowDefinition();
        newRow.Height = new GridLength(42);
        GridOperations.RowDefinitions.Add(newRow);

        if(!operation.isArbitrary)
        {
          UIOperationRobocopy row = new UIOperationRobocopy(operationIndex);
          row.SearchSourceButton.Click += OperationButtonSearchSource_Click;
          row.SearchDestButton.Click += OperationButtonSearchDest_Click;
          row.SourceText.LostFocus += OperationTextBoxSource_LostFocus;
          row.DestText.LostFocus += OperationTextBoxDest_LostFocus;
          row.Mirror.Checked += OperationCheckMirror_enable;
          row.Move.Checked += OperationCheckMove_enable;
          row.OnlyNewer.Checked += OperationCheckOnlyNewer_enable;

          Grid.SetColumn(row.SearchSourceButton, 1);
          Grid.SetRow(row.SearchSourceButton, operationIndex);
          Grid.SetColumn(row.SourceText, 2);
          Grid.SetRow(row.SourceText, operationIndex);
          Grid.SetColumn(row.SearchDestButton, 3);
          Grid.SetRow(row.SearchDestButton, operationIndex);
          Grid.SetColumn(row.DestText, 4);
          Grid.SetRow(row.DestText, operationIndex);
          Grid.SetColumn(row.ExclFilesButton, 5);
          Grid.SetRow(row.ExclFilesButton, operationIndex);
          Grid.SetColumn(row.ExclFoldersButton, 6);
          Grid.SetRow(row.ExclFoldersButton, operationIndex);
          Grid.SetColumn(row.Mirror, 7);
          Grid.SetRow(row.Mirror, operationIndex);
          Grid.SetColumn(row.Move, 8);
          Grid.SetRow(row.Move, operationIndex);
          Grid.SetColumn(row.OnlyNewer, 9);
          Grid.SetRow(row.OnlyNewer, operationIndex);
          Grid.SetColumn(row.FATFileTime, 10);
          Grid.SetRow(row.FATFileTime, operationIndex);


          GridOperations.Children.Add(row.SearchSourceButton);
          GridOperations.Children.Add(row.SourceText);
          GridOperations.Children.Add(row.SearchDestButton);
          GridOperations.Children.Add(row.DestText);
          GridOperations.Children.Add(row.ExclFilesButton);
          GridOperations.Children.Add(row.ExclFoldersButton);
          GridOperations.Children.Add(row.Mirror);
          GridOperations.Children.Add(row.Move);
          GridOperations.Children.Add(row.OnlyNewer);
          GridOperations.Children.Add(row.FATFileTime);

          GridOperations.RegisterName(row.SourceText.Name, row.SourceText);
          GridOperations.RegisterName(row.DestText.Name, row.DestText);
          GridOperations.RegisterName(row.Mirror.Name, row.Mirror);
          GridOperations.RegisterName(row.Move.Name, row.Move);
          GridOperations.RegisterName(row.FATFileTime.Name, row.FATFileTime);
          registeredNames.Add(row.SourceText.Name);
          registeredNames.Add(row.DestText.Name);
          registeredNames.Add(row.Mirror.Name);
          registeredNames.Add(row.Move.Name);
          registeredNames.Add(row.FATFileTime.Name);
        } else
        {
          UIOperationArbitrary row = new UIOperationArbitrary(operationIndex);
          row.Command.LostFocus += OperationTextBoxCommand_LostFocus;
          Grid.SetColumn(row.label, 1);
          Grid.SetRow(row.label, operationIndex);
          Grid.SetColumn(row.Command, 2);
          Grid.SetColumnSpan(row.Command, 9);
          Grid.SetRow(row.Command, operationIndex);
          GridOperations.Children.Add(row.label);
          GridOperations.Children.Add(row.Command);
        }

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
          OperationsList[index].enabled = true;
        };
        enabled.Unchecked += (sender, e) =>
        {
          CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
          int index = Convert.ToInt32(s.Tag);
          OperationsList[index].enabled = false;
        };
        Grid.SetColumn(enabled, 0);
        Grid.SetRow(enabled, operationIndex);

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
          OperationsList.RemoveAt(index);
          List<Control> toDelete = new List<Control>();
          foreach (Control control in GridOperations.Children)
          {
            if((int)control.GetValue(Grid.RowProperty) == index)
            {
              toDelete.Add(control);
            }
            if ((int)control.GetValue(Grid.RowProperty) > index)
            {
              Grid.SetRow(control, (int)control.GetValue(Grid.RowProperty) - 1);
              control.Tag = Convert.ToInt32(control.Tag) - 1;
            }
          }
          foreach (UIElement control in toDelete)
          {
            GridOperations.Children.Remove(control);
          }
          GridOperations.RowDefinitions.RemoveAt(index);
        };
        Grid.SetColumn(remove, 11);
        Grid.SetRow(remove, operationIndex);

        GridOperations.Children.Add(enabled);
        GridOperations.Children.Add(remove);

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
        OperationsList.Add(new Operation(string.Empty, string.Empty));
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
      MessageBox.Show("Inspect!");  // set breakpoint here for convenient variable inspection
      
      StreamWriter file;
      if (!File.Exists(currentFile))
      {
        file = File.CreateText(currentFile);
      }
      else
      {
        file = new StreamWriter(currentFile);
      }
      file.WriteLine("echo off");
      file.WriteLine("title " + scriptTitle);

      foreach (Operation item in OperationsList)
      {
        if (item.isArbitrary || (!string.IsNullOrWhiteSpace(item.SourceFolder) && !string.IsNullOrWhiteSpace(item.DestinationFolder)))
        {
          file.WriteLine();
          if (!item.enabled)
          {
            file.Write("REM ");
          }
          if(!item.isArbitrary)
          {
            file.WriteLine("echo " + item.Name);
          }          
          file.WriteLine(item.Command());
        }
      }
      file.Close();
    }
  }
}
