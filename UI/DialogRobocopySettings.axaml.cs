using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using robocopy_gui.Classes;
using System;

namespace robocopy_gui.UI;

public partial class DialogRobocopySettings : Window {
  public bool IsCancel { get; set; } = false;
  public bool OnlyNewer { get; set; } = false;
  public bool FATTime { get; set; } = false;
  public int RetryCount { get; set; } = 0;
  public int MultiThread { get; set; } = 3;

  public DialogRobocopySettings() {
    InitializeComponent();
  }

  public DialogRobocopySettings(Operation operation) {
    InitializeComponent();
    OnlyNewer = operation.IsOnlyIfNewer;
    CheckOnlyNewer.IsChecked = OnlyNewer;
    FATTime = operation.IsUseFATTime;
    CheckFAT.IsChecked = FATTime;
    RetryCount = operation.RetryCount;
    NumericRetryCount.Value = RetryCount;
    MultiThread = operation.MultiThreadCount;
    NumericMultiThread.Value = MultiThread;
  }

  private void ButtonDone_Click(object? sender, RoutedEventArgs e) {
    IsCancel = false;
    OnlyNewer = CheckOnlyNewer.IsChecked ?? false;
    FATTime = CheckFAT.IsChecked ?? false;
    RetryCount = Convert.ToInt32(NumericRetryCount.Value);
    MultiThread = Convert.ToInt32(NumericMultiThread.Value);
    RobocopySettingsWindow.Close();
  }
  private void ButtonCancel_Click(object? sender, RoutedEventArgs e) {
    IsCancel = true;
    RobocopySettingsWindow.Close();
  }

  private void OperationCheckOnlyNewer_enable(object? sender, RoutedEventArgs e) {
    //CheckBox s = sender as CheckBox ?? throw new Exception("Sender is null");
    //int index = Convert.ToInt32(s.Tag);
    //OperationsList[index].IsOnlyIfNewer = true;
    //foreach (Control control in GridOperations.Children) {
    //  if (control.Name == "FATtime" && Convert.ToInt32(control.Tag) == index) {
    //    ((CheckBox)control).IsChecked = true;
    //  }
    //}
  }
}
