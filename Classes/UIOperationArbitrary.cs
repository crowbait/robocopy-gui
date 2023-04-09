using System.Windows;
using System.Windows.Controls;

namespace robocopy_gui.Classes
{
  internal class UIOperationArbitrary
  {
    public int Index { get; set; }
    public Label Label { get; set; }
    public TextBox Command { get; set; }

    public UIOperationArbitrary(int operationIndex)
    {
      Index = operationIndex;

      Label = new Label
      {
        Content = "Command:",
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
      };

      Command = new TextBox
      {
        Name = "arbitraryCommand" + operationIndex,
        Text = MainWindow.OperationsList[operationIndex].Command,
        VerticalAlignment = VerticalAlignment.Top,
        Margin = new Thickness(10, 10, 0, 0),
        TextWrapping = TextWrapping.NoWrap,
        Tag = operationIndex
      };
    }
  }
}
