using System.Windows.Controls;

namespace GameTheoryUtility.Pages;

public partial class PracticePage : Page
{
    public PracticePage(PracticeViewModel model)
    {
        DataContext = model;
        model.Visual = this;
        InitializeComponent();
    }
}