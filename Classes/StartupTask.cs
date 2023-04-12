using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Win32;
using robocopy_gui.UI;
using System;
using System.Threading.Tasks;

namespace robocopy_gui
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
    public async Task<bool> Register()
    {
      RegistryKey key = Registry.CurrentUser.OpenSubKey(startupKey, RegistryKeyPermissionCheck.ReadWriteSubTree) ?? throw new ArgumentException("HKEY_CURRENT_USER run key does not exist");
      foreach (string valueName in key.GetValueNames())
      {
        if(valueName.Equals(Name))
        {
          if(key?.GetValue(Name)?.ToString() != Path)
          {
            DialogMessage message = new DialogMessage(
              Name + " is already set to startup with path '" + key?.GetValue(Name)?.ToString() + "'.\nOverwrite?",
              "Name conflict!"
            );
            message.SetButtons(new string[] { "Yes", "No" });
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
              await message.ShowDialog(desktop.MainWindow);
            }
            if(message.ReturnValue == "Yes")
            {
              break;
            } else
            {
              return false; //User does not want to overwrite existing startup key
            }
          }
          return true; //Name is already set with correct Path
        }
      }
      key?.SetValue(Name, Path, RegistryValueKind.String);
      return true;
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
