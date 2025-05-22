using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace GameTheoryUtility.Controls;

public partial class ReadonlyMatrixControl : UserControl
{
    public ReadonlyMatrixControl()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property.Name == nameof(DataContext))
        {
            if (DataContext == null || DataContext is not ReadonlyMatrixViewModel)
                return;
            var factory = new FrameworkElementFactory(typeof(UniformGrid));
            factory.SetValue(UniformGrid.RowsProperty, ((ReadonlyMatrixViewModel)DataContext).Size.Rows);
            factory.SetValue(UniformGrid.ColumnsProperty, ((ReadonlyMatrixViewModel)DataContext).Size.Columns);
            grid.ItemsPanel = new(factory);
        }
    }
}
