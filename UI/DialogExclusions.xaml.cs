using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace robocopy_gui
{
    /// <summary>
    /// Interaction logic for DialogExclusions.xaml
    /// </summary>
    public partial class DialogExclusions : Window
    {
        private List<StringObject> exclusions = new List<StringObject>();
        public List<string> returnExclusions = new List<string>();
        public DialogExclusions(List<string> exclusionList)
        {
            InitializeComponent();
            foreach (string item in exclusionList)
            {
                exclusions.Add(new StringObject(item));
            }
            renderList();
        }

        private void renderList()
        {
            int index = 0; 
            GridExclusions.RowDefinitions.Clear();
            GridExclusions.Children.Clear();
            foreach (StringObject exclusion in exclusions)
            {
                RowDefinition newRow = new RowDefinition();
                newRow.Height = new GridLength(42);
                GridExclusions.RowDefinitions.Add(newRow);

                TextBox pattern = new TextBox();
                //pattern.Text = exclusions[index];
                pattern.VerticalAlignment = VerticalAlignment.Top;
                pattern.Margin = new Thickness(10, 10, 0, 0);
                pattern.TextWrapping = TextWrapping.NoWrap;
                Binding binding = new Binding
                {
                    Path = new PropertyPath("Value"),
                    Source = exclusions[index],
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                pattern.SetBinding(TextBox.TextProperty, binding);
                Grid.SetColumn(pattern, 0);
                Grid.SetRow(pattern, index);

                Button remove = new Button();
                remove.Content = "-";
                remove.HorizontalAlignment = HorizontalAlignment.Center;
                remove.VerticalAlignment = VerticalAlignment.Center;
                remove.Width = 60;
                remove.Tag = index;
                remove.Click += (s, e) =>
                {
                    Button sender = s as Button ?? throw new Exception("Sender is null");
                    int index = Convert.ToInt32(sender.Tag);
                    exclusions.RemoveAt(index);
                    renderList();
                };
                Grid.SetColumn(remove, 1);
                Grid.SetRow(remove, index);

                GridExclusions.Children.Add(pattern);
                GridExclusions.Children.Add(remove);
                index++;
            }

            RowDefinition addRow = new RowDefinition();
            addRow.Height = new GridLength(42);
            GridExclusions.RowDefinitions.Add(addRow);

            Button add = new Button();
            add.Content = "+";
            add.HorizontalAlignment = HorizontalAlignment.Center;
            add.VerticalAlignment = VerticalAlignment.Center;
            add.Width = 60;
            add.Click += (s, e) =>
            {
                exclusions.Add(new StringObject(""));
                renderList();
            };
            Grid.SetColumn(add, 1);
            Grid.SetRow(add, index);

            GridExclusions.Children.Add(add);
        }

        private class StringObject
        {
            public string Value { get; set; } = string.Empty;
            public StringObject(string init)
            {
                Value = init;
            }
        }

        private void ButtonExclusionsOK_Click(object sender, RoutedEventArgs e)
        {
            returnExclusions.Clear();
            foreach (StringObject exclusion in exclusions)
            {
                if( !string.IsNullOrWhiteSpace(exclusion.Value) ) 
                {
                    returnExclusions.Add(exclusion.Value);
                }                
            }
            DialogExclusionsWindow.DialogResult = true;
        }

        private void ButtonExclusionsCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogExclusionsWindow.DialogResult= false;
        }
    }
}
