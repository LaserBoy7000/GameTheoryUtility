using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace GameTheoryUtility;

[ObservableObject]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        //Process.GetProcesses().FirstOrDefault(x => x.MainWindowTitle.Contains("test.docx"))?.CloseMainWindow();
        //var str = File.Open("C:\\Users\\vasyl\\Desktop\\test.docx", FileMode.Create);
        //using var wrt = new WordFileWriter(str);
        //wrt.UseStyle(new DefaultEducationalDocument());
        //wrt.PageHeader("Варіант 1");
        //wrt.CommonText("Задано ігрову матрицю");
        //wrt.AddMatrix(new Logic.Matrix<double>(3, 3));
        //wrt.CommonText("Визначте:");
        //wrt.List(
        //    "мінімаксну стратегію;",
        //    "максимінну стратегію;",
        //    "сідлову точку або її відсутність;");
        //wrt.CommonText("Для пошуку розв'язку застосувати:");
        //wrt.List(
        //    "зведення до ЗЛП із подальшим розв'язком графічним методом",
        //    "обчислення");
        //wrt.Save();
    }

    private void Window_OnContentRendered(object sender, EventArgs e)
    {
        InvalidateVisual();
    }

    private void FullScreen(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
            ((PackIcon)((Button)sender).Content).Kind = PackIconKind.FullscreenExit;
        }
        else
        {
            WindowState = WindowState.Normal;
            ((PackIcon)((Button)sender).Content).Kind = PackIconKind.Fullscreen;
        }
        InvalidateVisual();
        UpdateWindowPadding();
    }

    private void UpdateWindowPadding()
    {
        if (WindowState == WindowState.Maximized)
        {
            // Remove window margins (no gaps when maximized)
            Padding = new Thickness(8); // Or zero depending on design
        }
        else
        {
            // Restore margins when normal
            Padding = new Thickness(0);
        }
    }

    private void Exit(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    private void Minimize(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Minimized)
            return;
        WindowState = WindowState.Minimized;
    }
}

public partial class NavigationViewInteractor(NavigationService service)
{
    public NavigationService Service => service;

    [RelayCommand]
    public void GoBack() => service.GoBack();

    [RelayCommand]
    public void GoForward() => service.GoForward();

    [RelayCommand]
    public void Navigate(string page) => service.Navigate(new Uri("PracticePage.xaml", UriKind.Relative));
}