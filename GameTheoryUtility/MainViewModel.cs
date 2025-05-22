using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic.Contexts;
using GameTheoryUtility.Logic.Matrix;
using GameTheoryUtility.Pages;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace GameTheoryUtility;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        Navigation = new(new AboutPage());
        Navigation.Navigated += (pr, nx) =>
        {
            if (nx != null && nx.DataContext is IEnvironmentContextViewModel nvm)
            {
                if (nvm.ParentContext != null)
                    return;
                nvm.IsSelected = true;
            }
            if (pr != null && pr.DataContext is IEnvironmentContextViewModel vm)
            {
                if (nx!.DataContext == vm.ParentContext && vm.ParentContext != null)
                    return;
                vm.IsSelected = false;
                if (vm.ParentContext != null)
                    vm.ParentContext.IsSelected = false;
            }
        };
        App.Current.DispatcherUnhandledException += (o, e) =>
        {
            MessageQueue.Enqueue(e.Exception.Message, null, null, null, true, true, TimeSpan.FromSeconds(5));
            e.Handled = true;
        };
    }

    public NavigationManager Navigation { get; }
    public ObservableCollection<IEnvironmentContextViewModel> ActiveContexts { get; } = [];

    [ObservableProperty]
    private MatrixViewModel _matrix = new MatrixViewModel(new Matrix<int>(3, 3));

    public SnackbarMessageQueue MessageQueue { get; } = new();

    void CloseContext(IEnvironmentContextViewModel context)
    {
        if(context is GenerationViewModel gen)
        {
            foreach (var t in gen.Templates)
                Navigation.Remove(t.Visual!, new AboutPage());
            foreach(var i in gen.Instances)
                Navigation.Remove(i.Model.Visual!, new AboutPage());
        }
        Navigation.Remove(context.Visual!, new AboutPage());
        ActiveContexts.Remove(context);
    }

    void SelectContext(IEnvironmentContextViewModel context)
    {
        Navigation.Navigate(context.Visual!);
    }

    [RelayCommand(CanExecute = nameof(CanGoSettings))]
    void Settings() => Navigation.Navigate(new SettingsPage());
    bool CanGoSettings => Navigation.CurrentPage?.GetType() != typeof(SettingsPage);

    [RelayCommand]
    void StartNewPractice()
    {
        var ctx = new PracticeContext();
        var vm = new PracticeViewModel(new PracticeContext(), MessageQueue, CloseContext, SelectContext);
        vm.Visual = new PracticePage(vm);
        ActiveContexts.Add(vm);
        Navigation.Navigate(vm.Visual);
    }

    [RelayCommand]
    void StartNewGeneration()
    {
        var ctx = new GenerationContext();
        var vm = new GenerationViewModel(ctx, Navigation, MessageQueue, CloseContext, SelectContext);
        vm.Visual = new GenerationPage(vm);
        ActiveContexts.Add(vm);
        Navigation.Navigate(vm.Visual);
    }

    [RelayCommand]
    void About() => Navigation.Navigate(new AboutPage());

    [RelayCommand]
    async Task Save()
    {
        if (Navigation.CurrentPage?.DataContext is not IEnvironmentContextViewModel)
            return;
        var dialog = new SaveFileDialog()
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Filter = "Файл середовища (*.json)|*.json",
            FileName = ((IEnvironmentContextViewModel)Navigation.CurrentPage.DataContext).Context.Name,
        };
        if (dialog.ShowDialog() == true)
        {
            await File.WriteAllTextAsync(dialog.FileName!, JsonSerializer.Serialize(((IEnvironmentContextViewModel)Navigation.CurrentPage.DataContext).Context, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            }));
        }
    }

    [RelayCommand]
    async Task Open()
    {
        var dialog = new OpenFileDialog()
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            CheckFileExists = true,
            Filter = "Файл середовища (*.json)|*.json"
        };

        if (dialog.ShowDialog() == true)
        {
            var context = JsonSerializer.Deserialize<IEnvironmentContext>(await File.ReadAllTextAsync(dialog.FileName));
            if(context is PracticeContext p)
            {
                var vm = new PracticeViewModel(p, MessageQueue, CloseContext, SelectContext);
                vm.Visual = new PracticePage(vm);
                ActiveContexts.Add(vm);
                Navigation.Navigate(vm.Visual);
            }
            else
            {
                var vm = new GenerationViewModel((GenerationContext)context!, Navigation, MessageQueue, CloseContext, SelectContext);
                vm.Visual = new GenerationPage(vm);
                ActiveContexts.Add(vm);
                Navigation.Navigate(vm.Visual);
            }
        }
    }
}