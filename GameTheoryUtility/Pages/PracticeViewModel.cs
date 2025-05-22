using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic;
using GameTheoryUtility.Logic.Computations;
using GameTheoryUtility.Logic.Contexts;
using GameTheoryUtility.Logic.Matrix;
using GameTheoryUtility.Logic.Solvers;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows.Controls;

namespace GameTheoryUtility.Pages;

public partial class PracticeViewModel(PracticeContext context, ISnackbarMessageQueue? messages = null, Action<IEnvironmentContextViewModel>? closeCallback = null, Action<IEnvironmentContextViewModel>? selectCallback = null, IEnvironmentContextViewModel? parentModel = null) : ObservableObject, IEnvironmentContextViewModel
{
    readonly PracticeContext _context = context;

    public PracticeViewModel(ISnackbarMessageQueue? messages = null, Action<IEnvironmentContextViewModel>? closeCallback = null, Action<IEnvironmentContextViewModel>? selectCallback = null, IEnvironmentContextViewModel? parentModel = null) : this(new(), messages, closeCallback, selectCallback, parentModel) { }

    public IEnvironmentContext Context => _context;

    [ObservableProperty]
    bool _isSelected = false;

    public Page? Visual { get; set; }

    [RelayCommand]
    void Select()
    {
        selectCallback?.Invoke(this);
        IsSelected = true;
    }

    [RelayCommand]
    void Close()
    {
        closeCallback?.Invoke(this);
    }

    public PackIconKind Icon => PackIconKind.School;

    [ObservableProperty]
    MatrixViewModel _matrix = context.Game.Matrix.CreateViewModel(context.Game.Parameters);

    [ObservableProperty]
    bool _isIntegerGame = context.Game.Parameters.IsInteger;

    public bool EnableSolution { get; init; } = true;

    partial void OnIsIntegerGameChanged(bool value)
    {
        _context.Game.Parameters.IsInteger = value;
        UpdateMatrix();
    }

    [ObservableProperty]
    bool _isPassiveSecondPlayer = context.Game.Parameters.IsPassiveSecondPlayer;

    [ObservableProperty]
    PackIconKind _secondPlayerIcon = context.Game.Parameters.IsPassiveSecondPlayer ? PackIconKind.WeatherLightningRainy : PackIconKind.HumanMale;

    partial void OnIsPassiveSecondPlayerChanged(bool value)
    {
        _context.Game.Parameters.IsPassiveSecondPlayer = value;
        SecondPlayerIcon = value ? PackIconKind.WeatherLightningRainy : PackIconKind.HumanMale;
        UpdateSolvers();
    }

    [ObservableProperty]
    int _decimalPrecision = context.Game.Parameters.DecimalPrecision;

    partial void OnDecimalPrecisionChanged(int value)
    {
        _context.Game.Parameters.DecimalPrecision = value;
        UpdateMatrix();
    }

    [ObservableProperty]
    int _strategiesCountA = context.Game.Parameters.StrategiesCountA;

    partial void OnStrategiesCountAChanged(int value)
    {
        _context.Game.Parameters.StrategiesCountA = value;
        UpdateMatrix();
    }

    [ObservableProperty]
    int _strategiesCountB = context.Game.Parameters.StrategiesCountB;

    partial void OnStrategiesCountBChanged(int value)
    {
        _context.Game.Parameters.StrategiesCountB = value;
        UpdateMatrix();
    }

    [ObservableProperty]
    int? _generatorSeed = context.Game.Parameters.GeneratorSeed;

    partial void OnGeneratorSeedChanged(int? value) =>
         _context.Game.Parameters.GeneratorSeed = value;

    [ObservableProperty]
    double _maxCellValue = context.Game.Parameters.MaxCellValue;

    partial void OnMaxCellValueChanged(double value) =>
        _context.Game.Parameters.MaxCellValue = value;

    [ObservableProperty]
    double _minCellValue = context.Game.Parameters.MinCellValue;

    partial void OnMinCellValueChanged(double value) =>
        _context.Game.Parameters.MinCellValue = value;

    [ObservableProperty]
    bool _noRepeats = context.Game.Parameters.NoRepeats;
    partial void OnNoRepeatsChanged(bool value) =>
        _context.Game.Parameters.NoRepeats = value;

    [ObservableProperty]
    int? _desirableSaddlePoint = context.Game.Parameters.DesirableSaddlePoint;

    partial void OnDesirableSaddlePointChanged(int? value) =>
        _context.Game.Parameters.DesirableSaddlePoint = value;

    [ObservableProperty]
    bool _enableSaddlePoint = context.Game.Parameters.EnableSaddlePoint;

    [ObservableProperty]
    bool _areGenerationParametersEnabled = false;

    partial void OnEnableSaddlePointChanged(bool value) =>
        _context.Game.Parameters.EnableSaddlePoint = value;

    public IEnumerable<SolverViewModel> AvailableSolvers { get; } = CreateSolversSet(context);

    public IEnvironmentContextViewModel? ParentContext => parentModel;

    [ObservableProperty]
    GameComputationInstance? _computationInstance = context.Game.LastComputation;

    void ResetComputation() => ComputationInstance = null;
 
    [RelayCommand(AllowConcurrentExecutions = false)]
    public async Task ComputeAsync(CancellationToken cancellationToken = default)
    {
        var cmp = _context.Game.CreateNewComputation();
        messages?.Enqueue("Розпочато процес обчислення", null, null, null, false, true, TimeSpan.FromSeconds(2));
        await Task.Run(cmp.Compute);
        messages?.Enqueue("Обчислення завершено.", null, null, null, false, true, TimeSpan.FromSeconds(4));
        _context.Game.LastComputation = cmp;
        ComputationInstance = cmp;
    }

    [RelayCommand]
    void GenerateMatrix()
    {
        Matrix = _context.Game
            .GenerateMatrix()
            .CreateViewModel(_context.Game.Parameters);
        GeneratorSeed = _context.Game.Parameters.GeneratorSeed;
        if (_context.Game.Parameters.NoRepeats || DesirableSaddlePoint == null)
            DesirableSaddlePoint = _context.Game.Parameters.DesirableSaddlePoint;
    }

    void UpdateSolvers()
    {
        foreach (var solver in AvailableSolvers)
            solver.UpdateApplicability(_context.Game.Parameters);
        foreach (var solver in AvailableSolvers)
            if (solver.IsApplicable)
                solver.IsEnabled = true;
    }

    void UpdateMatrix()
    {
        Matrix = _context.Game
            .UpdateMatrix()
            .CreateViewModel(_context.Game.Parameters);
    }

    static IEnumerable<SolverViewModel> CreateSolversSet(PracticeContext context)
    {
        IEnumerable<SolverViewModel> solvers = [
            new CleanStrategySolver().CreateViewModel(),
            new LinearProgrammingSolver().CreateViewModel(),
            new BrownRobinsonSolver().CreateViewModel(),
            new WaldCriterionSolver().CreateViewModel(),
            new HurwitzCriterionSolver().CreateViewModel(),
            new SavageCriterionSolver().CreateViewModel()
        ];
        foreach (var solver in solvers)
        {
            solver.UpdateApplicability(context.Game.Parameters);
            solver.PropertyChanged += (_, _) =>
            {
                context.Game.Solvers.Clear();
                context.Game.Solvers.AddRange(solvers.Where(x => x.IsEnabled).Select(x => x.Solver));
            };
        }
        foreach (var solver in solvers)
            solver.IsEnabled = solver.IsApplicable;
        return solvers;
    }

    public PracticeViewModel GenerateClone()
    {
        var cloned = _context.Game.Clone();
        var vm = new PracticeViewModel(new PracticeContext(cloned), parentModel: ParentContext);
        if (AreGenerationParametersEnabled)
        {
            vm.AreGenerationParametersEnabled = true;
            vm.GenerateMatrix();
        }
        return vm;
    }
}
