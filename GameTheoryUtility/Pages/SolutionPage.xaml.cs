using GameTheoryUtility.Logic.Computations;
using GameTheoryUtility.Services;
using System.Windows;
using System.Windows.Controls;

namespace GameTheoryUtility.Pages;


public partial class SolutionPage : UserControl
{
    public readonly static DependencyProperty StepsProperty = DependencyProperty.Register("Steps", typeof(IEnumerable<IComputationStep>), typeof(SolutionPage), new PropertyMetadata { DefaultValue = new List<IComputationStep>() });
    readonly WpfVisualizationEngine _visualizer;

    public SolutionPage()
    {
        InitializeComponent();
        _visualizer = new(Panel.Children);
    }

    public IEnumerable<IComputationStep> Steps
    {
        get => (IEnumerable<IComputationStep>)GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
    }

    void Render()
    {
        Panel.Children.Clear();
        foreach (var step in Steps)
            step.Render(_visualizer);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == StepsProperty)
            Render();
    }
}
