using CommunityToolkit.Mvvm.Input;
using GameTheoryUtility.Logic.Contexts;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls;

namespace GameTheoryUtility.Pages;

public interface IEnvironmentContextViewModel
{
    public IEnvironmentContext Context { get; }
    public bool IsSelected { get; set; }
    public IRelayCommand SelectCommand { get; }
    public IRelayCommand CloseCommand { get; }
    public PackIconKind Icon { get; }
    public Page? Visual { get; }
    public IEnvironmentContextViewModel? ParentContext { get; }
}
