using System.Collections.Generic;

namespace robocopy_gui.Classes
{
  internal class Operation
  {
    public bool enabled { get; set; } = true;
    public string Name { get; set; }
    public string SourceFolder { get; set; }
    public string DestinationFolder { get; set; }
    public bool mirror { get; set; } //true -> delete extra files not present in source | false -> keep extra files (/e /xx)
                                     //mirror and move are mutually exclusive
    public bool move { get; set; } = false; //true -> move files instead of copying, needs /e /xx
    public bool onlyIfNewer { get; set; } = false; //ignore if target file is newer
    public bool useFATTime { get; set; } = false; //useful when copying between two file systems, 2s precision
    public List<string> ExcludeFiles { get; set; }
    public List<string> ExcludeFolders { get; set; }
    public int MultiThreadCount { get; set; } = 3;
    public int RetryCount { get; set; } = 0;

    public Operation(
        string source,
        string destination,
        bool mirrorFlag = true
        )
    {
      SourceFolder = source;
      DestinationFolder = destination;
      Name = CreateName(source, destination);
      mirror = mirrorFlag;
      ExcludeFiles = new List<string>();
      ExcludeFolders = new List<string>();
      MultiThreadCount = 5;
      RetryCount = 5;
    }
    public Operation(
        string source,
        string destination,
        List<string> excludePatterns,
        bool excludeIsFolders,
        bool mirrorFlag = true
        )
    {
      SourceFolder = source;
      DestinationFolder = destination;
      Name = source.Substring(0, 2) + " " + source.Split("\\")[source.Split("\\").Length - 1]
          + " -> "
          + destination.Substring(0, 2) + " " + destination.Split("\\")[destination.Split("\\").Length - 1];
      mirror = mirrorFlag;
      if (excludeIsFolders)
      {
        ExcludeFiles = new List<string>();
        ExcludeFolders = excludePatterns;
      }
      else
      {
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
        )
    {
      SourceFolder = source;
      DestinationFolder = destination;
      Name = source.Substring(0, 2) + " " + source.Split("\\")[source.Split("\\").Length - 1]
          + " -> "
          + destination.Substring(0, 2) + " " + destination.Split("\\")[destination.Split("\\").Length - 1];
      mirror = mirrorFlag;
      ExcludeFiles = excludeFilePatterns;
      ExcludeFolders = excludeFolderPatterns;
    }

    public Operation(string command)
    {
      string[] parts = command.Split(" ");
      int i;
      if (parts[0].ToLower() == "rem")    //detect commented lines ("REM ...")
      {
        enabled = false;
        i = 2;
      }
      else
      {
        i = 1;
      }
      if (parts[i].EndsWith("\""))    //get source folder (including checking for spaces in path)
      {
        SourceFolder = parts[i];
      }
      else
      {
        while (!parts[i].EndsWith("\""))
        {
          SourceFolder += parts[i] + " ";
          i++;
        }
        SourceFolder += parts[i];
      }
      i++;
      if (parts[i].EndsWith("\""))    //get destination folder (including checking for spaces in path)
      {
        DestinationFolder = parts[i];
      }
      else
      {
        while (!parts[i].EndsWith("\""))
        {
          DestinationFolder += parts[i] + " ";
          i++;
        }
        DestinationFolder += parts[i];
      }
      i++;
      SourceFolder = SourceFolder.Replace("\"", string.Empty);
      DestinationFolder = DestinationFolder.Replace("\"", string.Empty);

      mirror = false;
      ExcludeFiles = new List<string>();
      ExcludeFolders = new List<string>();
      while (i < parts.Length)      //check for flags
      {
        if (parts[i].ToLower() == "/mir") { mirror = true; }
        if (parts[i].ToLower() == "/mov") { move = true; }
        if (parts[i].ToLower() == "/xo") { onlyIfNewer = true; }
        if (parts[i].ToLower() == "/fft") { useFATTime = true; }
        if (parts[i].ToLower().StartsWith("/mt"))
        {
          MultiThreadCount = int.Parse(parts[i].Split(":")[1]);
        }
        if (parts[i].ToLower().StartsWith("/r"))
        {
          RetryCount = int.Parse(parts[i].Split(":")[1].ToLower());
        }
        if (parts[i].ToLower() == "/xf")    //get excluded file patterns
        {
          i++;
          while (!parts[i].StartsWith("/"))
          {
            ExcludeFiles.Add(parts[i]);
            i++;
            if (i > parts.Length - 1) { break; }
          }
        }
        if (parts[i].ToLower() == "/xd")    //get excluded folder patterns
        {
          i++;
          while (!parts[i].StartsWith("/"))
          {
            ExcludeFolders.Add(parts[i]);
            i++;
            if (i > parts.Length - 1) { break; }
          }
        }
        i++;
      }
      Name = CreateName();
    }

    public string Command()
    {
      string command = "";
      if (enabled)
      {
        command += "robocopy";
      }
      else
      {
        command += "REM robocopy";
      }
      command += " \"" + SourceFolder + "\"";
      command += " \"" + DestinationFolder + "\"";
      if (mirror && !move)
      {
        command += " /mir";
      }
      else
      {
        command += " /e /xx";
      }
      if (move)
      {
        command += " /mov";
      }
      if (onlyIfNewer)
      {
        command += " /xo";
      }
      if (useFATTime)
      {
        command += " /fft";
      }
      command += " /mt:" + MultiThreadCount + " /R:" + RetryCount;
      if (ExcludeFiles.Count > 0)
      {
        command += " /xf";
        foreach (string item in ExcludeFiles)
        {
          command += " " + item;
        }
      }
      if (ExcludeFolders.Count > 0)
      {
        command += " /xd";
        foreach (string item in ExcludeFolders)
        {
          command += " " + item;
        }
      }
      return command;
    }

    public string CreateName()
    {
      if (string.IsNullOrWhiteSpace(SourceFolder) || string.IsNullOrWhiteSpace(DestinationFolder))
      {
        return string.Empty;
      }
      else
      {
        return SourceFolder.Substring(0, 2) + " " + SourceFolder.Split("\\")[SourceFolder.Split("\\").Length - 1]
        + " -> "
        + DestinationFolder.Substring(0, 2) + " " + DestinationFolder.Split("\\")[DestinationFolder.Split("\\").Length - 1];
      }
    }
    public string CreateName(string source, string destination)
    {
      if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination))
      {
        return string.Empty;
      }
      else
      {
        return source.Substring(0, 2) + " " + source.Split("\\")[source.Split("\\").Length - 1]
        + " -> "
        + destination.Substring(0, 2) + " " + destination.Split("\\")[destination.Split("\\").Length - 1];
      }
    }
  }
}
