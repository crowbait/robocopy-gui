using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Microsoft.Win32;
using robocopy_gui.Classes;
using robocopy_gui.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace robocopy_gui;

public partial class MainWindow : Window {
  public static List<Operation> OperationsList { get; set; } = new List<Operation>();
  private static RegistryKey Regkey =
#pragma warning disable CS8601 // Possible null reference assignment.
      //Yes, the key is possibly null here. This is handled later, in initializations.
      //So later on, this key is not null and all other checks should reflect that.
      Registry.CurrentUser.OpenSubKey("Software\\Stormbase\\robocopy-ui", RegistryKeyPermissionCheck.ReadWriteSubTree);
#pragma warning restore CS8601
  private string currentFile = string.Empty;
  private string scriptTitle = "Backup";

  public MainWindow() {
    InitializeComponent();
    try {
      var version = System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
      if (version is not null) {
        TitleLabel.Content = "Robocopy GUI " + version;
      }
      string lastFile = "";
      if (Regkey != null) {
        string? lastFileValue = Regkey.GetValue("lastFile") as string;
        if (lastFileValue is not null) {
          lastFile = lastFileValue;
        }
        var lastWidth = Regkey.GetValue("Width");
        var lastHeight = Regkey.GetValue("Height");
        if (lastHeight is not null && lastWidth is not null) {
          MainWindow1.Width = Convert.ToInt32(lastWidth);
          MainWindow1.Height = Convert.ToInt32(lastHeight);
        }
      } else {
        RegistryKey? key = Registry.CurrentUser.OpenSubKey("Software\\Stormbase", RegistryKeyPermissionCheck.ReadWriteSubTree);
        if (key == null) {
          Registry.CurrentUser.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree)?.CreateSubKey("Stormbase", true);
          key = Registry.CurrentUser.OpenSubKey("Software\\Stormbase", RegistryKeyPermissionCheck.ReadWriteSubTree);
        }
        key = key?.OpenSubKey("robocopy-ui");
        if (key == null) {
          Registry.CurrentUser.OpenSubKey("Software\\Stormbase", RegistryKeyPermissionCheck.ReadWriteSubTree)?.CreateSubKey("robocopy-ui", true);
        }
        Regkey = Registry.CurrentUser.OpenSubKey("Software\\Stormbase\\robocopy-ui", RegistryKeyPermissionCheck.ReadWriteSubTree)
          ?? throw new Exception("Couldn't open registry key after creation");
      }
      if (!string.IsNullOrWhiteSpace(lastFile)) {
        if (File.Exists(lastFile)) {
          InputFilePath.Text = lastFile;
          currentFile = lastFile;
          ButtonCommit.IsEnabled = true;
          ReadFile();
        }
      }
    } catch (Exception e) {
      DialogMessage dialog = new DialogMessage(e.Message, "Error");
      dialog.ShowDialog(MainWindow1);
      throw;
    }
  }


  /*                      UI ELEMENTS
   * ===============================================================================
   * ===============================================================================
   */


  private async void ButtonPickFile_Click(object sender, RoutedEventArgs e) {
    OpenFileDialog openFileDialog = new OpenFileDialog {
      Filters = new List<FileDialogFilter> { new FileDialogFilter { Extensions = new List<string> { "bat" }, Name = "Batch Files" } },
      AllowMultiple = false
    };

    var result = await openFileDialog.ShowAsync(MainWindow1);
    if (result is not null && result.Length > 0) {
      string fileName = result[0];
      currentFile = fileName;
      InputFilePath.Text = currentFile;
      if (File.Exists(fileName)) {
        ReadFile();
      } else {
        File.CreateText(fileName).Dispose();
        //show operations group with empty list to allow adding of new operations
        ClearOperationsList();
        RenderList();
      }
      ButtonCommit.IsEnabled = true;
    }
  }
  private async void ButtonReloadFile_Click(object sender, RoutedEventArgs e) {
    if (File.Exists(currentFile)) {
      ReadFile();
    } else {
      DialogMessage message = new DialogMessage(
        "The file / path you've entered does not exist.\nDo you want to create it?",
        "File doesn't exist"
        );
      message.SetButtons(new string[] { "Yes", "Cancel" });
      await message.ShowDialog(MainWindow1);
      if (message.ReturnValue == "Yes") {
        File.CreateText(currentFile).Dispose();
        //show operations group with empty list to allow adding of new operations
        ClearOperationsList();
        RenderList();
        ButtonCommit.IsEnabled = true;
      } else {
        InputFilePath.Text = currentFile;
      }
      message.Close();
    }
  }
  private async void InputFilePath_LostFocus(object? sender, RoutedEventArgs e) {
    InputFilePath.LostFocus -= InputFilePath_LostFocus; //necessary, because on Enter this procedure would then fire again
    if (currentFile != InputFilePath.Text) {
      string fileName = InputFilePath.Text;
      if (File.Exists(fileName)) {
        currentFile = fileName;
        ReadFile();
        ButtonCommit.IsEnabled = true;
      } else {
        DialogMessage message = new DialogMessage(
          "The file / path you've entered does not exist.\nDo you want to create it?",
          "File doesn't exist"
          );
        message.SetButtons(new string[] { "Yes", "Cancel" });
        await message.ShowDialog(MainWindow1);
        if (message.ReturnValue == "Yes") {
          File.CreateText(fileName).Dispose();
          currentFile = fileName;
          //show operations group with empty list to allow adding of new operations
          ClearOperationsList();
          RenderList();
          ButtonCommit.IsEnabled = true;
        } else {
          InputFilePath.Text = currentFile;
        }
        message.Close();
      }
    }
    InputFilePath.LostFocus += InputFilePath_LostFocus; //see above
  }
  private void InputFilePath_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key == Key.Enter) {
      InputFilePath_LostFocus(sender, e);
    }
  }
  private void InputScriptTitle_LostFocus(object sender, RoutedEventArgs e) {
    scriptTitle = InputScriptTitle.Text;
  }
  private void InputScriptTitle_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key == Key.Enter) {
      InputScriptTitle_LostFocus(sender, e);
    }
  }
  private async void CheckStartup_Checked(object sender, RoutedEventArgs e) {
    if (!await new StartupTask(currentFile, scriptTitle).Register()) {
      CheckStartup.IsChecked = false;
    }
  }
  private void CheckStartup_Unchecked(object sender, RoutedEventArgs e) {
    new StartupTask(currentFile, scriptTitle).Unregister();
  }


  /*                      ROUTINES FOR GENERATED OPERATION ROW UI ELEMENTS
 * ===============================================================================
 * ===============================================================================
 */

  private async void OperationButtonSearchSource_Click(object? sender, RoutedEventArgs e) {
    Button s = sender as Button ?? throw new Exception("Sender is null");
    int index = Convert.ToInt32(s.Tag);
    OpenFolderDialog dialog = new OpenFolderDialog {
      Directory = Path.GetDirectoryName(OperationsList[index].SourceFolder)
    };
    string? result = await dialog.ShowAsync(MainWindow1);
    if (result is not null) {
      OperationsList[index].SourceFolder = result;
      OperationsList[index].Name = OperationsList[index].CreateName();
      foreach (Control control in GridOperations.Children) {
        if (control.Name == "source" && Convert.ToInt32(control.Tag) == index) {
          ((TextBox)control).Text = result;
        }
      }
    }
  }
  private async void OperationButtonSearchDest_Click(object? sender, RoutedEventArgs args) {
    Button s = sender as Button ?? throw new Exception("Sender is null");
    int index = Convert.ToInt32(s.Tag);
    OpenFolderDialog dialog = new OpenFolderDialog {
      Directory = Path.GetDirectoryName(OperationsList[index].DestinationFolder)
    };
    string? result = await dialog.ShowAsync(MainWindow1);
    if (result is not null) {
      OperationsList[index].DestinationFolder = result;
      OperationsList[index].Name = OperationsList[index].CreateName();
      foreach (Control control in GridOperations.Children) {
        if (control.Name == "dest" && Convert.ToInt32(control.Tag) == index) {
          ((TextBox)control).Text = result;
        }
      }
    }
  }
  private void OperationTextBoxSource_LostFocus(object? sender, RoutedEventArgs e) {
    TextBox s = sender as TextBox ?? throw new Exception("Sender is null");
    int index = Convert.ToInt32(s.Tag);
    OperationsList[index].SourceFolder = s.Text;
    OperationsList[index].Name = OperationsList[index].CreateName();
  }
  private void OperationTextBoxDest_LostFocus(object? sender, RoutedEventArgs e) {
    TextBox s = sender as TextBox ?? throw new Exception("Sender is null");
    int index = Convert.ToInt32(s.Tag);
    OperationsList[index].DestinationFolder = s.Text;
    OperationsList[index].Name = OperationsList[index].CreateName();
  }
  private void OperationTextBoxCommand_LostFocus(object? sender, RoutedEventArgs e) {
    TextBox s = sender as TextBox ?? throw new Exception("Sender is null");
    int index = Convert.ToInt32(s.Tag);
    OperationsList[index].Command = s.Text;
  }
  private void OperationCheckMirror_enable(object? sender, RoutedEventArgs e) {
    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
    int index = Convert.ToInt32(s.Tag);
    OperationsList[index].IsMirror = true;
    foreach (Control control in GridOperations.Children) {
      if (control.Name == "move" && Convert.ToInt32(control.Tag) == index) {
        ((CheckBox)control).IsChecked = false;
      }
    }
  }
  private void OperationCheckMove_enable(object? sender, RoutedEventArgs e) {
    CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
    int index = Convert.ToInt32(s.Tag);
    OperationsList[index].IsMove = true;
    foreach (Control control in GridOperations.Children) {
      if (control.Name == "mirror" && Convert.ToInt32(control.Tag) == index) {
        ((CheckBox)control).IsChecked = false;
      }
    }
  }



  /*                      ROUTINES FOR READING AND PARSING FILE
   * ===============================================================================
   * ===============================================================================
   */


  private void ClearOperationsList() {
    OperationsList.Clear();
    foreach (Control control in GridOperations.Children) {
      if (control is TextBox) // this prevents LostFocus events from firing for non-existing elements after the list is cleared or re-populated
      {
        control.LostFocus -= OperationTextBoxSource_LostFocus;
        control.LostFocus -= OperationTextBoxDest_LostFocus;
      }
    }
    GridOperations.Children.Clear();
    GridOperations.RowDefinitions.Clear();
  }


  private void ReadFile() {
    Regkey.SetValue("lastFile", currentFile);
    ClearOperationsList();

    //read lines in file
    List<string> operationStrings = new List<string>();
    using (StreamReader reader = File.OpenText(currentFile)) {
      int skipFirstLines = 1; // 0 to disable - 1 is designed to skip "echo off" at the beginning of file
      for (int i = 0; i < skipFirstLines; i++) {
        reader.ReadLine();
      }
      while (!reader.EndOfStream) {
        var readLine = reader.ReadLine();
        if (readLine != null && !string.IsNullOrWhiteSpace(readLine)) {
          operationStrings.Add(readLine);
        }
      }
      reader.Close();
    }

    //interpret all read lines as operations
    foreach (string operation in operationStrings) {
      if (operation.ToLower().StartsWith("robocopy") || operation.ToLower().StartsWith("rem robocopy")) {
        OperationsList.Add(new Operation(operation));
      } else if (operation.ToLower().StartsWith("title ")) {
        scriptTitle = operation.Substring(6);
      } else if (operation.ToLower().StartsWith("rem ") && !operation.ToLower().StartsWith("rem echo")) {
        OperationsList.Add(new Operation(true, false, operation.Substring(3)));
      } else if (!operation.ToLower().StartsWith("echo")) {
        OperationsList.Add(new Operation(true, true, operation));
      }
    }
    InputScriptTitle.Text = scriptTitle;

    if (new StartupTask(currentFile, scriptTitle).CheckRegistration()) {
      CheckStartup.IsChecked = true;
    } else {
      CheckStartup.IsChecked = false;
    }
    RenderList();
  }
  private void AddOperationRow(Operation operation, int operationIndex) {
    RowDefinition newRow = new RowDefinition {
      Height = new GridLength(42)
    };
    GridOperations.RowDefinitions.Add(newRow);

    if (!operation.IsArbitrary) {
      RowOperationRobocopy row = new RowOperationRobocopy(operationIndex);
      row.SearchSourceButton.Click += OperationButtonSearchSource_Click;
      row.SearchDestButton.Click += OperationButtonSearchDest_Click;
      row.SourceText.LostFocus += OperationTextBoxSource_LostFocus;
      row.DestText.LostFocus += OperationTextBoxDest_LostFocus;
      row.Mirror.Checked += OperationCheckMirror_enable;
      row.Move.Checked += OperationCheckMove_enable;

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
      Grid.SetColumn(row.SettingsButton, 9);
      Grid.SetRow(row.SettingsButton, operationIndex);

      GridOperations.Children.Add(row.SearchSourceButton);
      GridOperations.Children.Add(row.SourceText);
      GridOperations.Children.Add(row.SearchDestButton);
      GridOperations.Children.Add(row.DestText);
      GridOperations.Children.Add(row.ExclFilesButton);
      GridOperations.Children.Add(row.ExclFoldersButton);
      GridOperations.Children.Add(row.Mirror);
      GridOperations.Children.Add(row.Move);
      GridOperations.Children.Add(row.SettingsButton);
    } else {
      RowOperationArbitrary row = new RowOperationArbitrary(operationIndex);
      row.Command.LostFocus += OperationTextBoxCommand_LostFocus;
      Grid.SetColumn(row.Command, 1);
      Grid.SetColumnSpan(row.Command, 10);
      Grid.SetRow(row.Command, operationIndex);
      GridOperations.Children.Add(row.Command);
    }

    CheckBox enabled = new CheckBox {
      Content = "Enabled",
      IsChecked = operation.IsEnabled,
      HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
      VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
      Tag = operationIndex
    };
    enabled.Checked += (sender, e) => {
      CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].IsEnabled = true;
    };
    enabled.Unchecked += (sender, e) => {
      CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(s.Tag);
      OperationsList[index].IsEnabled = false;
    };
    Grid.SetColumn(enabled, 0);
    Grid.SetRow(enabled, operationIndex);

    PathIcon removeIcon = new PathIcon {
      Data = Geometry.Parse(Icons.Delete)
    };
    Button remove = new Button {
      Content = removeIcon,
      HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
      VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
      Width = 60,
      Tag = operationIndex,
      HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center
    };
    remove.Click += (s, e) => {
      Button sender = s as Button ?? throw new Exception("Sender is null");
      int index = Convert.ToInt32(sender.Tag);
      OperationsList.RemoveAt(index);
      List<Control> toDelete = new List<Control>();
      foreach (Control control in GridOperations.Children) {
        if ((int)control.GetValue(Grid.RowProperty) == index) {
          toDelete.Add(control);  //the separate list is needed to not modify the collection that drives the foreach
        }
        if ((int)control.GetValue(Grid.RowProperty) > index) {
          Grid.SetRow(control, (int)control.GetValue(Grid.RowProperty) - 1);
          control.Tag = Convert.ToInt32(control.Tag) - 1;
        }
      }
      foreach (Control control in toDelete) {
        GridOperations.Children.Remove(control);
      }
      GridOperations.RowDefinitions.RemoveAt(index);
    };
    Grid.SetColumn(remove, 11);
    Grid.SetRow(remove, operationIndex);

    GridOperations.Children.Add(enabled);
    GridOperations.Children.Add(remove);
  }
  public void RenderList() {
    int operationIndex = 0;
    foreach (Operation operation in OperationsList) {
      AddOperationRow(operation, operationIndex);
      operationIndex++;
    }

    RowDefinition addRow = new RowDefinition {
      Height = new GridLength(42)
    };
    GridOperations.RowDefinitions.Add(addRow);

    Button add = new Button {
      Name = "ButtonAddRobocopy",
      Content = "+ Operation",
      HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
      VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
      Width = 240
    };
    add.Click += (s, e) => {
      Operation newOp = new Operation(string.Empty, string.Empty);
      OperationsList.Add(newOp);
      int currentAddRow = 0;
      foreach (Control control in GridOperations.Children) {
        if (control.Name == "ButtonAddArbitrary") {
          currentAddRow = (int)control.GetValue(Grid.RowProperty);
          Grid.SetRow(control, currentAddRow + 1);
          Grid.SetRow(s as Control, currentAddRow + 1);
        }
      }
      AddOperationRow(newOp, currentAddRow);
    };
    Button addArbitrary = new Button {
      Name = "ButtonAddArbitrary",
      Content = "+ Arbitrary Command",
      HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
      VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
      Width = 240
    };
    addArbitrary.Click += (s, e) => {
      Operation newOp = new Operation(true, true, string.Empty);
      OperationsList.Add(newOp);
      int currentAddRow = 0;
      foreach (Control control in GridOperations.Children) {
        if (control.Name == "ButtonAddRobocopy") {
          currentAddRow = (int)control.GetValue(Grid.RowProperty);
          Grid.SetRow(control, currentAddRow + 1);
          Grid.SetRow(s as Control, currentAddRow + 1);
        }
      }
      AddOperationRow(newOp, currentAddRow);
    };
    Grid.SetColumn(add, 7);
    Grid.SetColumnSpan(add, 4);
    Grid.SetRow(add, operationIndex);
    Grid.SetColumn(addArbitrary, 4);
    Grid.SetColumnSpan(addArbitrary, 3);
    Grid.SetRow(addArbitrary, operationIndex);

    GridOperations.Children.Add(add);
    GridOperations.Children.Add(addArbitrary);
  }

  private void ButtonCommit_Click(object sender, RoutedEventArgs e) {
    // MessageBox.Show("Inspect!");  // set breakpoint here for convenient variable inspection

    StreamWriter file;
    if (!File.Exists(currentFile)) {
      file = File.CreateText(currentFile);
    } else {
      file = new StreamWriter(currentFile);
    }
    file.WriteLine("echo off");
    file.WriteLine("title " + scriptTitle);

    foreach (Operation item in OperationsList) {
      if ((item.IsArbitrary && !string.IsNullOrWhiteSpace(item.Command)) ||
        (!string.IsNullOrWhiteSpace(item.SourceFolder) && !string.IsNullOrWhiteSpace(item.DestinationFolder))) {
        file.WriteLine();
        if (!item.IsEnabled) {
          file.Write("REM ");
        }
        if (!item.IsArbitrary) {
          file.WriteLine("echo " + item.Name);
        }
        file.WriteLine(item.GetCommand());
      }
    }
    file.Close();
  }

  private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
    Regkey.SetValue("Width", MainWindow1.Width);
    Regkey.SetValue("Height", MainWindow1.Height);
  }
}
