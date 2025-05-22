using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GameTheoryUtility.Controls;

public partial class MatrixControl : UserControl
{
    public MatrixControl()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property.Name == nameof(DataContext))
        {
            if (DataContext == null || DataContext is not MatrixViewModel)
                return;
            var factory = new FrameworkElementFactory(typeof(UniformGrid));
            factory.SetValue(UniformGrid.RowsProperty, ((MatrixViewModel)DataContext).Size.Rows);
            factory.SetValue(UniformGrid.ColumnsProperty, ((MatrixViewModel)DataContext).Size.Columns);
            grid.ItemsPanel = new(factory);
        }
    }

    private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if ((e.Key >= System.Windows.Input.Key.D0 && e.Key <= System.Windows.Input.Key.D9) ||
            e.Key == System.Windows.Input.Key.OemComma ||
            e.Key == System.Windows.Input.Key.Back ||
            e.Key == System.Windows.Input.Key.Enter ||
            e.Key == System.Windows.Input.Key.OemPeriod ||
            e.Key == System.Windows.Input.Key.Delete ||
            (e.Key >= System.Windows.Input.Key.Left && e.Key <= System.Windows.Input.Key.Down))
        {
            var textBox = (System.Windows.Controls.TextBox)sender;
            var cell = (CellViewModel)textBox.DataContext;
            switch (e.Key)
            {
                case Key.Up or Key.Down:
                    e.Handled = true;
                    var request = new TraversalRequest(e.Key == Key.Up ? FocusNavigationDirection.Up : FocusNavigationDirection.Down);
                    textBox.MoveFocus(request);
                    cell.Save();
                    break;
                case Key.Left:
                    if (textBox.SelectionStart == 0)
                    {
                        e.Handled = true;
                        request = new TraversalRequest(FocusNavigationDirection.Left);
                        textBox.MoveFocus(request);
                        cell.Save();
                    }
                    break;
                case Key.Right:
                    if (textBox.SelectionStart + textBox.SelectionLength == textBox.Text.Length)
                    {
                        e.Handled = true;
                        request = new TraversalRequest(FocusNavigationDirection.Right);
                        textBox.MoveFocus(request);
                        cell.Save();
                    }
                    break;
            }
            return;
        }

        e.Handled = true;
    }

    private void TextBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter)
        {
            var request = new TraversalRequest(FocusNavigationDirection.Next);
            (sender as UIElement)?.MoveFocus(request);
        }

    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        ((System.Windows.Controls.TextBox)sender).SelectionStart = 0;
        ((System.Windows.Controls.TextBox)sender).SelectionLength = ((System.Windows.Controls.TextBox)sender).Text.Length;
    }
}
