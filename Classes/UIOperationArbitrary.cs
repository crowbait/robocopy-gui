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

      Label = new Label();
      Label.Content = "Command:";
      Label.HorizontalAlignment = HorizontalAlignment.Center;
      Label.VerticalAlignment = VerticalAlignment.Center;

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
