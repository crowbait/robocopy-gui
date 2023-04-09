using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using robocopy_gui.Classes;
using System;
using System.Collections.Generic;
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
    public static List<Operation> OperationsList { get; set; } = new List<Operation>();
    private readonly List<string> registeredNames = new List<string>();
    private string currentFile = "";
    private string scriptTitle = "Backup";

    public MainWindow()
    {
      InitializeComponent();
      try
      {
        MainWindow1.Width = Properties.Settings.Default.MainWindowWidth;
        MainWindow1.Height = Properties.Settings.Default.MainWindowHeight;
        string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        this.Title = "Robocopy GUI " + version;
        string lastFile = Properties.Settings.Default.LastFile;
        if (!string.IsNullOrWhiteSpace(lastFile))
        {
          if (File.Exists(lastFile))
          {
            InputFilePath.Text = lastFile;
            currentFile = lastFile;
            ButtonCommit.IsEnabled = true;
            ReadFile();
          }
        }
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message + e.StackTrace);
      }
    }

    private void ButtonPickFile_Click(object sender, RoutedEventArgs e)
    {
      //pick batch file
      OpenFileDialog openFileDialog = new OpenFileDialog
      {
        CheckFileExists = false,
        Filter = "Batch Files (*.bat)|*.bat"
      };
      if (openFileDialog.ShowDialog() == true)
      {
        string fileName = openFileDialog.FileName;
        currentFile = fileName;
        InputFilePath.Text = currentFile;
        if (File.Exists(fileName))
        {
          ReadFile();
        }
        else
        {
          File.CreateText(fileName).Dispose();
          //show operations group with empty list to allow adding of new operations
          ClearOperationsList();
          RenderList();
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
          ReadFile();
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
            ClearOperationsList();
            RenderList();
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
        ReadFile();
        ButtonCommit.IsEnabled = true;
      }
    }
    private void InputScriptTitle_LostFocus(object sender, RoutedEventArgs e)
    {
      scriptTitle = InputScriptTitle.Text;
    }
    private void InputScriptTitle_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == System.Windows.Input.Key.Enter)
      {
        InputScriptTitle_LostFocus(sender, e);
      }
    }
    private void CheckStartup_Checked(object sender, RoutedEventArgs e)
    {
      new StartupTask(currentFile, scriptTitle).Register();
    }

    private void CheckStartup_Unchecked(object sender, RoutedEventArgs e)
    {
      new StartupTask(currentFile, scriptTitle).Unregister();
    }

    /*                      ROUTINES FOR GENERATED OPERATION ROW UI ELEMENTS
     * ===============================================================================
     * ===============================================================================
     */

    private void OperationButtonSearchSource_Click(object sender, RoutedEventArgs e)
    {
      Button s = sender as Button ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      CommonOpenFileDialog dialog = new CommonOpenFileDialog
      {
        InitialDirectory = Path.GetDirectoryName(MainWindow.OperationsList[index].SourceFolder),
        IsFolderPicker = true
      };
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
      CommonOpenFileDialog dialog = new CommonOpenFileDialog
      {
        InitialDirectory = Path.GetDirectoryName(OperationsList[index].DestinationFolder),
        IsFolderPicker = true
      };
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
      OperationsList[index].Command = s.Text;
    }
    private void OperationCheckMirror_enable(object sender, RoutedEventArgs e)
    {
      CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].IsMirror = true;
      CheckBox move = GridOperations.FindName("move" + index) as CheckBox ?? throw new Exception("Couldn't find appropriate CheckBox");
      move.IsChecked = false;
    }
    private void OperationCheckMove_enable(object sender, RoutedEventArgs e)
    {
      CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].IsMove = true;
      CheckBox mirror = GridOperations.FindName("mirror" + index) as CheckBox ?? throw new Exception("Couldn't find appropriate CheckBox");
      mirror.IsChecked = false;
    }
    private void OperationCheckOnlyNewer_enable(object sender, RoutedEventArgs e)
    {
      CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].IsOnlyIfNewer = true;
      CheckBox fatFileTime = GridOperations.FindName("FATtime" + index) as CheckBox ?? throw new Exception("Couldn't find appropriate CheckBox");
      fatFileTime.IsChecked = true;
    }


    /*                      ROUTINES FOR READING AND PARSING FILE
     * ===============================================================================
     * ===============================================================================
     */


    private void ClearOperationsList()
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
      GridOperations.Children.Clear();
      GridOperations.RowDefinitions.Clear();
    }

    private void ReadFile()
    {
      ClearOperationsList();

      //read lines in file
      List<string> operationStrings = new List<string>();
      using (StreamReader reader = File.OpenText(currentFile))
      {
        int skipFirstLines = 1; // 0 to disable - 1 is designed to skip "echo off" at the beginning of file
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
        if (operation.ToLower().StartsWith("robocopy") || operation.ToLower().StartsWith("rem robocopy"))
        {
          OperationsList.Add(new Operation(operation));
        }
        else if (operation.ToLower().StartsWith("title "))
        {
          scriptTitle = operation.Substring(6);
        }
        else if (operation.ToLower().StartsWith("rem ") && !operation.ToLower().StartsWith("rem echo"))
        {
          OperationsList.Add(new Operation(true, false, operation.Substring(3)));
        }
        else if (!operation.ToLower().StartsWith("echo"))
        {
          OperationsList.Add(new Operation(true, true, operation));
        }
      }
      InputScriptTitle.Text = scriptTitle;

      if (new StartupTask(currentFile, scriptTitle).CheckRegistration())
      {
        CheckStartup.IsChecked = true;
      }
      else
      {
        CheckStartup.IsChecked = false;
      }
      RenderList();
    }
    private void AddOperationRow(Operation operation, int operationIndex)
    {
      RowDefinition newRow = new RowDefinition
      {
        Height = new GridLength(42)
      };
      GridOperations.RowDefinitions.Add(newRow);

      if (!operation.IsArbitrary)
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
      }
      else
      {
        UIOperationArbitrary row = new UIOperationArbitrary(operationIndex);
        row.Command.LostFocus += OperationTextBoxCommand_LostFocus;
        Grid.SetColumn(row.Label, 1);
        Grid.SetRow(row.Label, operationIndex);
        Grid.SetColumn(row.Command, 2);
        Grid.SetColumnSpan(row.Command, 9);
        Grid.SetRow(row.Command, operationIndex);
        GridOperations.Children.Add(row.Label);
        GridOperations.Children.Add(row.Command);
      }

      CheckBox enabled = new CheckBox
      {
        Content = "Enabled",
        IsChecked = operation.IsEnabled,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Tag = operationIndex
      };
      enabled.Checked += (sender, e) =>
      {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        OperationsList[index].IsEnabled = true;
      };
      enabled.Unchecked += (sender, e) =>
      {
        CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(s.Tag);
        OperationsList[index].IsEnabled = false;
      };
      Grid.SetColumn(enabled, 0);
      Grid.SetRow(enabled, operationIndex);

      Button remove = new Button
      {
        Content = "-",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 60,
        Tag = operationIndex
      };
      remove.Click += (s, e) =>
      {

        Button sender = s as Button ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(sender.Tag);
        OperationsList.RemoveAt(index);
        List<Control> toDelete = new List<Control>();
        foreach (Control control in GridOperations.Children)
        {
          if ((int)control.GetValue(Grid.RowProperty) == index)
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
    }
    public void RenderList()
    {
      int operationIndex = 0;
      foreach (Operation operation in OperationsList)
      {
        AddOperationRow(operation, operationIndex);
        operationIndex++;
      }

      RowDefinition addRow = new RowDefinition
      {
        Height = new GridLength(42)
      };
      GridOperations.RowDefinitions.Add(addRow);

      Button add = new Button
      {
        Name = "ButtonAddRobocopy",
        Content = "+ Operation",
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 240
      };
      add.Click += (s, e) =>
      {
        Operation newOp = new Operation(string.Empty, string.Empty);
        OperationsList.Add(newOp);
        Button addArbitrary = GridOperations.FindName("ButtonAddArbitrary") as Button ?? throw new Exception("Couldn't find ButtonAddArbitrary");
        int currentAddRow = (int)addArbitrary.GetValue(Grid.RowProperty);
        AddOperationRow(newOp, currentAddRow);
        Grid.SetRow(addArbitrary, currentAddRow + 1);
        Grid.SetRow(s as Control, currentAddRow + 1);
      };
      Button addArbitrary = new Button
      {
        Name = "ButtonAddArbitrary",
        Content = "+ Arbitrary Command",
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 240
      };
      addArbitrary.Click += (s, e) =>
      {
        Operation newOp = new Operation(true, true, string.Empty);
        OperationsList.Add(newOp);
        Button addRobocopy = GridOperations.FindName("ButtonAddRobocopy") as Button ?? throw new Exception("Couldn't find ButtonAddRobocopy");
        int currentAddRow = (int)addRobocopy.GetValue(Grid.RowProperty);
        AddOperationRow(newOp, currentAddRow);
        Grid.SetRow(addRobocopy, currentAddRow + 1);
        Grid.SetRow(s as Control, currentAddRow + 1);
      };
      Grid.SetColumn(add, 7);
      Grid.SetColumnSpan(add, 4);
      Grid.SetRow(add, operationIndex);
      Grid.SetColumn(addArbitrary, 4);
      Grid.SetRow(addArbitrary, operationIndex);

      GridOperations.Children.Add(add);
      GridOperations.Children.Add(addArbitrary);
      GridOperations.RegisterName(add.Name, add);
      GridOperations.RegisterName(addArbitrary.Name, addArbitrary);
      registeredNames.Add(add.Name);
      registeredNames.Add(addArbitrary.Name);
      GroupOperations.Visibility = Visibility.Visible;
    }

    private void ButtonCommit_Click(object sender, RoutedEventArgs e)
    {
      // MessageBox.Show("Inspect!");  // set breakpoint here for convenient variable inspection

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
        if ((item.IsArbitrary && !string.IsNullOrWhiteSpace(item.Command)) ||
          (!string.IsNullOrWhiteSpace(item.SourceFolder) && !string.IsNullOrWhiteSpace(item.DestinationFolder)))
        {
          file.WriteLine();
          if (!item.IsEnabled)
          {
            file.Write("REM ");
          }
          if (!item.IsArbitrary)
          {
            file.WriteLine("echo " + item.Name);
          }
          file.WriteLine(item.GetCommand());
        }
      }
      file.Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Properties.Settings.Default.MainWindowWidth = MainWindow1.Width;
      Properties.Settings.Default.MainWindowHeight = MainWindow1.Height;
      Properties.Settings.Default.LastFile = currentFile;
      Properties.Settings.Default.Save();
    }
  }
}
