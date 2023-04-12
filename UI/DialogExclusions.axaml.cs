using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Core;
using Avalonia.Data;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using Avalonia.Interactivity;

namespace robocopy_gui.UI;

public partial class DialogExclusions : Window {
  private class StringObject {
    public string Value { get; set; } = string.Empty;
    public StringObject(string init) {
      Value = init;
    }
  }

  private readonly List<StringObject> exclusions = new List<StringObject>();
  public List<string> ReturnExclusions = new List<string>();
  public bool IsCancel = false;

  public DialogExclusions() {
    InitializeComponent();
  }

  public DialogExclusions(List<string> exclusionList) {
    InitializeComponent();
    foreach (string item in exclusionList) {
      exclusions.Add(new StringObject(item));
    }
    RenderList();
  }

  private void RenderList() {
    int index = 0;
    GridExclusions.RowDefinitions.Clear();
    GridExclusions.Children.Clear();
    foreach (StringObject exclusion in exclusions) {
      RowDefinition newRow = new RowDefinition {
        Height = new GridLength(42)
      };
      GridExclusions.RowDefinitions.Add(newRow);

      TextBox pattern = new TextBox {
        [!TextBox.TextProperty] = new Binding("Value", BindingMode.TwoWay) {
          Source = exclusions[index]
        },
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
        Margin = new Thickness(10, 5, 0, 0),
        TextWrapping = TextWrapping.NoWrap
      };
      Grid.SetColumn(pattern, 0);
      Grid.SetRow(pattern, index);

      PathIcon removeIcon = new PathIcon {
        Data = Geometry.Parse(Icons.Delete)
      };
      Button remove = new Button {
        Content = removeIcon,
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        Width = 60,
        Tag = index
      };
      remove.Click += (s, e) => {
        Button sender = s as Button ?? throw new Exception("Sender is null");
        int index = Convert.ToInt32(sender.Tag);
        exclusions.RemoveAt(index);
        RenderList();
      };
      Grid.SetColumn(remove, 1);
      Grid.SetRow(remove, index);

      GridExclusions.Children.Add(pattern);
      GridExclusions.Children.Add(remove);
      index++;
    }

    RowDefinition addRow = new RowDefinition {
      Height = new GridLength(42)
    };
    GridExclusions.RowDefinitions.Add(addRow);

    PathIcon addIcon = new PathIcon {
      Data = Geometry.Parse(Icons.Add)
    };
    Button add = new Button {
      Content = addIcon,
      HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
      VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
      Width = 60
    };
    add.Click += (s, e) => {
      exclusions.Add(new StringObject(""));
      RenderList();
    };
    Grid.SetColumn(add, 1);
    Grid.SetRow(add, index);

    GridExclusions.Children.Add(add);
  }

  private void ButtonExclusionsOK_Click(object sender, RoutedEventArgs e) {
    ReturnExclusions.Clear();
    foreach (StringObject exclusion in exclusions) {
      if (!string.IsNullOrWhiteSpace(exclusion.Value)) {
        ReturnExclusions.Add(exclusion.Value);
      }
    }
    IsCancel = false;
    ExclusionsWindow.Close();
  }

  private void ButtonExclusionsCancel_Click(object sender, RoutedEventArgs e) {
    IsCancel = true;
    ExclusionsWindow.Close();
  }
}
