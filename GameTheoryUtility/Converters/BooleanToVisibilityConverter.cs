using System.Globalization;
using System.Windows;

namespace GameTheoryUtility.Converters;

public sealed class BooleanToVisibilityConverter : System.Windows.Data.IValueConverter
{
    public Visibility True { get; set; } = Visibility.Visible;
    public Visibility False { get; set; } = Visibility.Collapsed;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool?)value == true ? True : False;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)value == True;
}
