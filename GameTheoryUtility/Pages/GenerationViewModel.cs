using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTheoryUtility.Logic.Contexts;
using GameTheoryUtility.Logic.External.Word;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Linq;

namespace GameTheoryUtility.Pages;

public partial class GenerationViewModel : EnvironmentContextViewModel
{
    readonly Action<IEnvironmentContextViewModel>? _closeCallback;
    readonly NavigationManager _navigation;
    readonly GenerationContext _context;
    readonly WordFileExporter _assignmentExporter = new() { FileType = "Завдання", IncludeProblem = true, IncludeSolution = true, ProducePdf = true };
    readonly WordFileExporter _solutionExporter = new() { FileType = "Відповіді", IncludeSolution = true };

    public GenerationViewModel(NavigationManager navigation, ISnackbarMessageQueue? messages = null, Action<IEnvironmentContextViewModel>? closeCallback = null, Action<IEnvironmentContextViewModel>? selectCallback = null) : this(new(), navigation, messages, closeCallback, selectCallback) { }

    public GenerationViewModel(GenerationContext context, NavigationManager navigation, ISnackbarMessageQueue? messages = null, Action<IEnvironmentContextViewModel>? closeCallback = null, Action<IEnvironmentContextViewModel>? selectCallback = null) : base(context: context, messages: messages, closeCallback: closeCallback, selectCallback: selectCallback)
    {
        _context = context;
        _navigation = navigation;
        _closeCallback = closeCallback;
        Templates = new(context.Templates.Select(x =>
        {
            var vm = new PracticeViewModel(new PracticeContext(x), closeCallback: RemoveTemplate, parentModel: this) { EnableSolution = false };
            vm.Visual = new PracticePage(vm);    
            return vm;
        }));
        int idx = 1;
        _instances = new(context.Instances.Select(x =>
        {
            var vm = new InstanceViewModel(new PracticeViewModel(new PracticeContext(x), closeCallback: RemoveInstance, parentModel: this) { EnableSolution = true }, idx++);
            vm.Model.Visual = new PracticePage(vm.Model);
            return vm;
        }));
        _instancesCount = _instances.Count;
        Templates.CollectionChanged += (o, e) =>
        {
            context.Templates.Clear();
            context.Templates.AddRange(e.NewItems!.Cast<PracticeViewModel>().Select(x => ((PracticeContext)x.Context).Game));
        };
    }

    public ObservableCollection<PracticeViewModel> Templates { get; }

    [ObservableProperty]
    List<InstanceViewModel> _instances;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GenerateInstancesCommand))]
    int _instancesCount;

    public override PackIconKind Icon => PackIconKind.FileDocumentMultiple;

    partial void OnInstancesChanged(List<InstanceViewModel> value)
    {
        _context.Instances.Clear();
        _context.Instances.AddRange(value.Select(x => ((PracticeContext)x.Model.Context).Game));
    }

    [RelayCommand]
    public void CreateTemplate()
    {
        var param = new Logic.Game.GameEnvironment
        {
            Parameters = new Logic.Matrix.GeneratedMatrixGameParameters
            {
                StrategiesCountA = 2,
                StrategiesCountB = 3
            }
        };
        param.UpdateMatrix();
        var template = new PracticeViewModel(new PracticeContext(param), closeCallback: RemoveTemplate, parentModel: this)
        { EnableSolution = false };
        template.Visual = new PracticePage(template);
        Templates.Add(template);
    }

    void RemoveTemplate(IEnvironmentContextViewModel template)
    {
        Templates.Remove((PracticeViewModel)template);
        _closeCallback?.Invoke(template);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    async Task ExportAssignments()
    {
        var dialog = new OpenFolderDialog()
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        };
        if (dialog.ShowDialog() == true)
            await _assignmentExporter.ExportAsync(_context, dialog.FolderName);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    async Task ExportSolutions()
    {
        var dialog = new OpenFolderDialog()
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        };
        if (dialog.ShowDialog() == true)
            await _solutionExporter.ExportAsync(_context, dialog.FolderName);
    }

    [RelayCommand]
    public void Open(PracticeViewModel page) => _navigation.Navigate(page.Visual!);

    [RelayCommand]
    public void CloseTemplate(PracticeViewModel template) =>
        RemoveTemplate(template);

    void RemoveInstance(IEnvironmentContextViewModel instance)
    {
        int idx = 1;
        Instances = Instances.Where(x => x.Model != instance).Select(x => new InstanceViewModel(x.Model, idx++)).ToList();
        _closeCallback?.Invoke(instance);
    }

    [RelayCommand]
    public void CloseInstance(IEnvironmentContextViewModel instance) =>
        RemoveInstance(instance);

    bool CanGenerate => InstancesCount > 0;

    [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanGenerate))]
    public async Task GenerateInstancesAsync()
    {
        var generated = new List<PracticeViewModel>();
        Instances = (await Task.WhenAll(Enumerable.Range(0, InstancesCount).Select(async i => {
            var model = await Task.Run(async () =>
            {
                var template = Templates[i % Templates.Count];
                var clone = template.GenerateClone();
                return new InstanceViewModel(clone, i + 1);
            });
            model.Model.Visual = new PracticePage(model.Model);
            await Task.Run(async () => await model.Model.ComputeAsync());
            return model;
        }))).ToList();
    }
}
