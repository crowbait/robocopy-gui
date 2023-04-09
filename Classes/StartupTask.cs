using Microsoft.Win32;
using System;
using System.Windows;

namespace robocopy_gui.Classes
{
  internal class StartupTask
  {
    public string Path { get; set; }
    public string Name { get; set; }

    private const string startupKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

    public StartupTask(string path, string name)
    {
      Path = path;
      Name = name;
    }
    public bool CheckRegistration()
    {
      RegistryKey key = Registry.CurrentUser.OpenSubKey(startupKey) ?? throw new ArgumentException("HKEY_CURRENT_USER run key does not exist");
      foreach (string valueName in key.GetValueNames())
      {
        if(valueName == Name)
        {
          if (key?.GetValue(Name)?.ToString() != Path)  //key and balue should actually never be null because of previous checks
          {
            return false; //Name is present, but Path doesn't match
          }
          else
          {
            return true;  //Name and Path match
          }
        }
      }
      return false; //Name is not present in key's values
    }
    public void Register()
    {
      RegistryKey key = Registry.CurrentUser.OpenSubKey(startupKey, RegistryKeyPermissionCheck.ReadWriteSubTree) ?? throw new ArgumentException("HKEY_CURRENT_USER run key does not exist");
      foreach (string valueName in key.GetValueNames())
      {
        if(valueName.Equals(Name))
        {
          if(key?.GetValue(Name)?.ToString() != Path)
          {
            if(MessageBox.Show(
              Name + " is already set to startup with path '" + key?.GetValue(Name)?.ToString() + "'.\nOverwrite?",
              "Name conflict!",
              MessageBoxButton.YesNo) == MessageBoxResult.Yes )
            {
              break;
            } else
            {
              return; //User does not want to overwrite existing startup key
            }
          }
          return; //Name is already set with correct Path
        }
      }
      key?.SetValue(Name, Path, RegistryValueKind.String);
    }

    public void Unregister()
    {
      RegistryKey key = Registry.CurrentUser.OpenSubKey(startupKey, RegistryKeyPermissionCheck.ReadWriteSubTree) ?? throw new ArgumentException("HKEY_CURRENT_USER run key does not exist");
      if(key?.GetValue(Name)?.ToString() == Path)
      {
        key?.DeleteValue(Name);
      }
    }
  }
}
