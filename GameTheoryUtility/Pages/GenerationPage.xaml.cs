using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic.Solvers;
using System.Windows.Controls;

namespace GameTheoryUtility.Pages;

public partial class GenerationPage : Page
{
    public GenerationPage(GenerationViewModel viewModel)
    {
        DataContext = viewModel;
        viewModel.Visual = this;
        InitializeComponent();
    }
}
