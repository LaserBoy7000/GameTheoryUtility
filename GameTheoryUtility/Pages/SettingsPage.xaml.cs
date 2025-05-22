using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameTheoryUtility.Pages;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        var helper = new PaletteHelper();
        var theme = helper.GetTheme();
    }

    private void OnDisableDarkTheme(object sender, System.Windows.RoutedEventArgs e)
    {
        var helper = new PaletteHelper();
        var theme = helper.GetTheme();
        theme.SetLightTheme();
        theme.SetPrimaryColor(Color.FromRgb(17, 8, 71));
        new PaletteHelper().SetTheme(theme);
    }

    private void OnEnableDarkTheme(object sender, System.Windows.RoutedEventArgs e)
    {
        var helper = new PaletteHelper();
        var theme = helper.GetTheme();
        theme.SetPrimaryColor(Color.FromRgb(255, 255, 255));
        theme.SetDarkTheme();
        new PaletteHelper().SetTheme(theme);
    }
}
