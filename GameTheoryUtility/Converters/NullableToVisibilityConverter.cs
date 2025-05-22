using System.Globalization;
using System.Windows;

namespace GameTheoryUtility.Converters;

public sealed class NullableToVisibilityConverter : System.Windows.Data.IValueConverter
{
    public Visibility Null { get; set; } = Visibility.Collapsed;
    public Visibility NotNull { get; set; } = Visibility.Visible;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (object?)value == null ? Null : NotNull;
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)value == Null ? null : null!;
}
