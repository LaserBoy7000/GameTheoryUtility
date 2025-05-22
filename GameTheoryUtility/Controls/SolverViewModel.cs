using CommunityToolkit.Mvvm.ComponentModel;
using GameTheoryUtility.Logic.Matrix;
using GameTheoryUtility.Logic.Solvers;
using MaterialDesignThemes.Wpf;

namespace GameTheoryUtility.Controls;

public partial class SolverViewModel(ISolver solver, bool initialIsEnabled, params SolverOptionViewModel[] options) : ObservableObject
{
    public ISolver Solver => solver;
    public IEnumerable<SolverOptionViewModel> Options => options;
    [ObservableProperty]
    bool _isEnabled = initialIsEnabled;
    [ObservableProperty]
    bool _isApplicable = true;
    public void UpdateApplicability(IMatrixGameParameters parameters)
    {
        IsApplicable = Solver.IsApplicable(parameters);
        if (!IsApplicable)
            IsEnabled = false;
    }
    partial void OnIsEnabledChanged(bool value)
    {
        foreach (var option in Options)
            option.IsEnabled = value;
    }

    public PackIconKind SolverIcon { get; } = solver switch
    {
        LinearProgrammingSolver => PackIconKind.ChartLine,
        CleanStrategySolver => PackIconKind.AbTesting,
        BrownRobinsonSolver => PackIconKind.AlphaBCircle,
        HurwitzCriterionSolver => PackIconKind.AlphaHCircle,
        WaldCriterionSolver => PackIconKind.AlphaWCircle,
        SavageCriterionSolver => PackIconKind.AlphaSCircle,
        LagrangeCriterionSolver => PackIconKind.AlphaL,
        _ => default 
    };
}
