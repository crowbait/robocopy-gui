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
  public bool RestartableBackup { get; set; } = false;
  public bool Create { get; set; } = false;
  public bool LoggingEnabled { get; set; } = true;
  public bool LogFiles { get; set; } = true;
  public bool LogFolders { get; set; } = true;
  public bool LogHeader { get; set; } = true;
  public bool LogSummary { get; set; } = true;
  public bool LogProgress { get; set; } = true;
  public bool LogSizes { get; set; } = true;
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
    RestartableBackup = operation.IsRestartableBackup;
    CheckRestartableBackup.IsChecked = RestartableBackup;
    Create = operation.IsCreate;
    CheckCreate.IsChecked = Create;

    LoggingEnabled = operation.IsLoggingEnabled;
    CheckEnableLogging.IsChecked = LoggingEnabled;
    SwitchLoggingState(LoggingEnabled);
    LogFiles = operation.IsLoggingFiles;
    CheckLogFiles.IsChecked = LogFiles;
    LogFolders = operation.IsLoggingFolders;
    CheckLogFolders.IsChecked = LogFolders;
    LogHeader = operation.IsLoggingJobHeader;
    CheckLogHeader.IsChecked = LogHeader;
    LogSummary = operation.IsLoggingJobSummary;
    CheckLogSummary.IsChecked = LogSummary;
    LogProgress = operation.IsLoggingProgress;
    CheckLogProgress.IsChecked = LogProgress;
    LogSizes = operation.IsLoggingSize;
    CheckLogSizes.IsChecked = LogSizes;
    RetryCount = operation.RetryCount;
    NumericRetryCount.Value = RetryCount;
    MultiThread = operation.MultiThreadCount;
    NumericMultiThread.Value = MultiThread;
  }

  private void SwitchLoggingState(bool newState) {
    CheckLogFiles.IsEnabled = newState;
    CheckLogFolders.IsEnabled = newState;
    CheckLogHeader.IsEnabled = newState;
    CheckLogSummary.IsEnabled = newState;
    CheckLogProgress.IsEnabled = newState;
    CheckLogSizes.IsEnabled = newState;
  }
  private void CheckEnableLogging_Checked(object? sender, RoutedEventArgs e) {
    SwitchLoggingState(true);
  }
  private void CheckEnableLogging_Unchecked(object? sender, RoutedEventArgs e) {
    SwitchLoggingState(false);
  }
  private void CheckOnlyNewer_Check(object? sender, RoutedEventArgs e) {
    CheckFAT.IsChecked = true;
  }

  private void ButtonDone_Click(object? sender, RoutedEventArgs e) {
    IsCancel = false;
    OnlyNewer = CheckOnlyNewer.IsChecked ?? false;
    FATTime = CheckFAT.IsChecked ?? false;
    RestartableBackup = CheckRestartableBackup.IsChecked ?? false;
    Create = CheckCreate.IsChecked ?? false;
    LoggingEnabled = CheckEnableLogging.IsChecked ?? false;
    LogFiles = CheckLogFiles.IsChecked ?? false;
    LogFolders = CheckLogFolders.IsChecked ?? false;
    LogHeader = CheckLogHeader.IsChecked ?? false;
    LogSummary= CheckLogSummary.IsChecked ?? false;
    LogProgress = CheckLogProgress.IsChecked ?? false;
    LogSizes = CheckLogSizes.IsChecked ?? false;
    RetryCount = Convert.ToInt32(NumericRetryCount.Value);
    MultiThread = Convert.ToInt32(NumericMultiThread.Value);
    RobocopySettingsWindow.Close();
  }
  private void ButtonCancel_Click(object? sender, RoutedEventArgs e) {
    IsCancel = true;
    RobocopySettingsWindow.Close();
  }

}
