using System.Windows;
using System.Windows.Controls;

namespace robocopy_gui.Classes
{
  internal class UIOperationArbitrary
  {
    public int index { get; set; }
    public Label label { get; set; }
    public TextBox Command { get; set; }

    public UIOperationArbitrary(int operationIndex)
    {
      index = operationIndex;

      label = new Label();
      label.Content = "Command:";
      label.HorizontalAlignment = HorizontalAlignment.Center;
      label.VerticalAlignment = VerticalAlignment.Center;

      Command = new TextBox();
      Command.Name = "arbitraryCommand" + operationIndex;
      Command.Text = MainWindow.OperationsList[operationIndex].arbitraryCommand;
      Command.VerticalAlignment = VerticalAlignment.Top;
      Command.Margin = new Thickness(10, 10, 0, 0);
      Command.TextWrapping = TextWrapping.NoWrap;
      Command.Tag = operationIndex;
    }
  }
}
