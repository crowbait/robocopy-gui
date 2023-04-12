using Avalonia.Controls;

namespace robocopy_gui.UI {
  internal class RowOperationArbitrary {
    public int Index { get; set; }
    public TextBox Command { get; set; }

    public RowOperationArbitrary(int operationIndex) {
      Index = operationIndex;
      Command = new TextBox {
        Name = "arbitraryCommand",
        Text = MainWindow.OperationsList[operationIndex].Command,
        Watermark = "Command",
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
        Margin = new Avalonia.Thickness(10, 5, 0, 0),
        TextWrapping = Avalonia.Media.TextWrapping.NoWrap,
        Tag = operationIndex
      };
    }
  }
}
