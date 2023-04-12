using Avalonia;
using Avalonia.Controls;

namespace robocopy_gui.UI;

public partial class DialogMessage : Window {
  private string _text;
  public string Text {
    get { return _text; }
    set {
      _text = value;
      TextBlock.Text = _text;
    }
  }
  private string _header;
  public string Header {
    get { return _header; }
    set {
      _header = value;
      TitleLabel.Content = value;
    }
  }
  public void SetButtons(string[] value) {
    int index = 0;
    foreach (string item in value) {
      Button button = new Button {
        Content = item,
        Margin = new Thickness(5, 0, 0, 0)
      };
      if (index == 0) {
        button.IsDefault = true;
      }
      button.Click += (s, e) => {
        ReturnValue = item;
        MessageWindow.Close();
      };
      GridButtons.Children.Add(button);
      GridButtons.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
      index++;
      Grid.SetColumn(button, index);
    }
  }
  public string ReturnValue = string.Empty;

  public DialogMessage() {
    InitializeComponent();
    _text = string.Empty;
    _header = string.Empty;
  }
  public DialogMessage(string text, string title) {
    InitializeComponent();
    _text = text;
    Text = text;
    _header = title;
    Header = title;
  }
}
