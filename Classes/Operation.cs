using System;
using System.Collections.Generic;

namespace robocopy_gui.Classes {
  public class Operation {
    public bool IsEnabled { get; set; } = true;
    public bool IsArbitrary { get; set; } = false;
    public string Command { get; set; } = string.Empty;
    public string Name { get; set; }
    public string SourceFolder { get; set; }
    public string DestinationFolder { get; set; }
    public bool IsMirror { get; set; } //true -> delete extra files not present in source | false -> keep extra files (/e /xx)
                                       //mirror and move are mutually exclusive
    public bool IsMove { get; set; } = false; //true -> move files instead of copying, needs /e /xx
    public bool IsOnlyIfNewer { get; set; } = false; //ignore if target file is newer
    public bool IsUseFATTime { get; set; } = false; //useful when copying between two file systems, 2s precision
    public bool IsRestartableBackup { get; set; } = false; //copies files in restartable mode. If file access is denied, switches to backup mode.
    public bool IsCreate { get; set; } = false; //copies only folder structure, with zero-length files
    public bool IsLoggingFiles { get; set; } = true; //nfl doesn't list files names
    public bool IsLoggingFolders { get; set; } = true; //ndl doesn't list folder names
    public bool IsLoggingJobHeader { get; set; } = false; //njh doesn't log job header
    public bool IsLoggingJobSummary { get; set; } = false; //njs doesn't log job summary
    public bool IsLoggingProgress { get; set; } = true; //np doesn't show copying progress
    public bool IsLoggingSize { get; set; } = true; //ns doesn't log file size
    public bool IsLoggingEnabled { get; set; } = true; //> NUL at EOL doesn't output log at all
    public List<string> ExcludeFiles { get; set; }
    public List<string> ExcludeFolders { get; set; }
    public int MultiThreadCount { get; set; } = 3;
    public int RetryCount { get; set; } = 0;

    public Operation(
        string source,
        string destination,
        bool mirrorFlag = true
        ) {
      SourceFolder = source;
      DestinationFolder = destination;
      Name = CreateName(source, destination);
      IsMirror = mirrorFlag;
      ExcludeFiles = new List<string>();
      ExcludeFolders = new List<string>();
    }
    public Operation(
        string source,
        string destination,
        List<string> excludePatterns,
        bool excludeIsFolders,
        bool mirrorFlag = true
        ) {
      SourceFolder = source;
      DestinationFolder = destination;
      Name = source.Substring(0, 2) + " " + source.Split("\\")[source.Split("\\").Length - 1]
          + " -> "
          + destination.Substring(0, 2) + " " + destination.Split("\\")[destination.Split("\\").Length - 1];
      IsMirror = mirrorFlag;
      if (excludeIsFolders) {
        ExcludeFiles = new List<string>();
        ExcludeFolders = excludePatterns;
      } else {
        ExcludeFiles = excludePatterns;
        ExcludeFolders = new List<string>();
      }
    }
    public Operation(
        string source,
        string destination,
        List<string> excludeFilePatterns,
        List<string> excludeFolderPatterns,
        bool mirrorFlag = true
        ) {
      SourceFolder = source;
      DestinationFolder = destination;
      Name = source.Substring(0, 2) + " " + source.Split("\\")[source.Split("\\").Length - 1]
          + " -> "
          + destination.Substring(0, 2) + " " + destination.Split("\\")[destination.Split("\\").Length - 1];
      IsMirror = mirrorFlag;
      ExcludeFiles = excludeFilePatterns;
      ExcludeFolders = excludeFolderPatterns;
    }

    public Operation(string command) {
      //init for variables that are reversely tested (aka are false by default and can only be tested for false)
      IsLoggingJobSummary = true;

      string[] parts = command.Split(" ");
      int i;
      if (parts[0].ToLower() == "rem")    //detect commented lines ("REM ...")
      {
        IsEnabled = false;
        i = 2;
      } else {
        i = 1;
      }
      if (parts[i].EndsWith("\""))    //get source folder (including checking for spaces in path)
      {
        SourceFolder = parts[i];
      } else {
        while (!parts[i].EndsWith("\"")) {
          SourceFolder += parts[i] + " ";
          i++;
        }
        SourceFolder += parts[i];
      }
      i++;
      if (parts[i].EndsWith("\""))    //get destination folder (including checking for spaces in path)
      {
        DestinationFolder = parts[i];
      } else {
        while (!parts[i].EndsWith("\"")) {
          DestinationFolder += parts[i] + " ";
          i++;
        }
        DestinationFolder += parts[i];
      }
      i++;
      SourceFolder = SourceFolder.Replace("\"", string.Empty);
      DestinationFolder = DestinationFolder.Replace("\"", string.Empty);

      IsMirror = false;
      ExcludeFiles = new List<string>();
      ExcludeFolders = new List<string>();
      while (i < parts.Length)      //check for flags
      {
        if (parts[i].ToLower() == "/mir") { IsMirror = true; }
        if (parts[i].ToLower() == "/mov") { IsMove = true; }
        if (parts[i].ToLower() == "/xo") { IsOnlyIfNewer = true; }
        if (parts[i].ToLower() == "/fft") { IsUseFATTime = true; }
        if (parts[i].ToLower() == "/zb") { IsRestartableBackup = true; }
        if (parts[i].ToLower() == "/create") { IsCreate = true; }
        if (parts[i].ToLower() == "/nfl") { IsLoggingFiles = false; }
        if (parts[i].ToLower() == "/ndl") { IsLoggingFolders = false; }
        if (parts[i].ToLower() == "/njh") { IsLoggingJobHeader = false; }
        if (parts[i].ToLower() == "/njs") { IsLoggingJobSummary = false; }
        if (parts[i].ToLower() == "/np") { IsLoggingProgress = false; }
        if (parts[i].ToLower() == "/ns") { IsLoggingSize = false; }
        if (parts[i].ToLower().StartsWith("/mt")) {
            MultiThreadCount = int.Parse(parts[i].Split(":")[1]);
          }
        if (parts[i].ToLower().StartsWith("/r")) {
          RetryCount = int.Parse(parts[i].Split(":")[1].ToLower());
        }
        if (parts[i].ToLower() == "/xf")    //get excluded file patterns
        {
          i++;
          while (!parts[i].StartsWith("/") && !parts[i].StartsWith(">")) {
            ExcludeFiles.Add(parts[i]);
            i++;
            if (i > parts.Length - 1) { break; }
          }
          if (i > parts.Length - 1) { break; }
        }
        if (parts[i].ToLower() == "/xd")    //get excluded folder patterns
        {
          i++;
          while (!parts[i].StartsWith("/") && !parts[i].StartsWith(">")) {
            ExcludeFolders.Add(parts[i]);
            i++;
            if (i > parts.Length - 1) { break; }
          }
        }
        if (i + 1 < parts.Length) {
          if (parts[i] == ">" && parts[i + 1] == "NUL") {
            IsLoggingEnabled = false;
            break;                          //> NUL is always at EOL, so loop can stop at this point
          }
        }
        i++;
      }
      Name = CreateName();
    }

    public Operation(bool isArbitraryCommand, bool isEnabled, string command) {
      if (isArbitraryCommand) {
        IsArbitrary = isArbitraryCommand;
        IsEnabled = isEnabled;
        Command = command;
        Name = string.Empty;
        SourceFolder = string.Empty;
        DestinationFolder = string.Empty;
        ExcludeFiles = new List<string>();
        ExcludeFolders = new List<string>();
      } else {
        // check to prevent accidental generation of wrongly-typed Operation objects
        throw new ArgumentException("Arbitrary commands must be called with isArbitraryCommand = true");
      }
    }

    public string GetCommand() {
      if (!IsArbitrary) {
        string command = "";
        if (IsEnabled) {
          command += "robocopy";
        } else {
          command += "REM robocopy";
        }
        command += " \"" + SourceFolder + "\"";
        command += " \"" + DestinationFolder + "\"";
        if (IsMirror && !IsMove) {
          command += " /mir";
        } else {
          command += " /e /xx";
        }
        if (IsMove) { command += " /mov"; }
        if (IsOnlyIfNewer) { command += " /xo"; }
        if (IsUseFATTime) { command += " /fft"; }
        if (IsRestartableBackup) { command += " /zb"; }
        if (IsCreate) { command += " /create"; }
        if (!IsLoggingFiles) { command += " /nfl"; }
        if (!IsLoggingFolders) { command += " /ndl"; }
        if (!IsLoggingJobHeader) { command += " /njh"; }
        if (!IsLoggingJobSummary) { command += " /njs"; }
        if (!IsLoggingProgress) { command += " /np"; }
        if (!IsLoggingSize) { command += " /ns"; }
        command += " /mt:" + MultiThreadCount + " /R:" + RetryCount;
        if (ExcludeFiles.Count > 0) {
          command += " /xf";
          foreach (string item in ExcludeFiles) {
            command += " " + item;
          }
        }
        if (ExcludeFolders.Count > 0) {
          command += " /xd";
          foreach (string item in ExcludeFolders) {
            command += " " + item;
          }
        }
        if (!IsLoggingEnabled) { command += " > NUL"; }
        return command;
      } else // is arbitrary command
        {
        if (IsEnabled) {
          return Command;
        } else {
          return "REM " + Command;
        }
      }
    }

    public string CreateName() {
      if (string.IsNullOrWhiteSpace(SourceFolder) || string.IsNullOrWhiteSpace(DestinationFolder)) {
        return string.Empty;
      } else {
        return SourceFolder.Substring(0, 2) + " " + SourceFolder.Split("\\")[SourceFolder.Split("\\").Length - 1]
        + " -> "
        + DestinationFolder.Substring(0, 2) + " " + DestinationFolder.Split("\\")[DestinationFolder.Split("\\").Length - 1];
      }
    }
    public static string CreateName(string source, string destination) {
      if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination)) {
        return string.Empty;
      } else {
        return source.Substring(0, 2) + " " + source.Split("\\")[source.Split("\\").Length - 1]
        + " -> "
        + destination.Substring(0, 2) + " " + destination.Split("\\")[destination.Split("\\").Length - 1];
      }
    }
  }
}
