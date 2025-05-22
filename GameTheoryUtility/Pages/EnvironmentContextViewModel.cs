using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTheoryUtility.Logic.Contexts;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls;

namespace GameTheoryUtility.Pages;

public abstract partial class EnvironmentContextViewModel(IEnvironmentContext context, IEnvironmentContextViewModel? parentContext = null, ISnackbarMessageQueue? messages = null, Action<IEnvironmentContextViewModel>? closeCallback = null, Action<IEnvironmentContextViewModel>? selectCallback = null) : ObservableObject, IEnvironmentContextViewModel
{
    public IEnvironmentContext Context => context;

    [RelayCommand]
    void Select()
    {
        selectCallback?.Invoke(this);
        IsSelected = true;
    }

    [ObservableProperty]
    public bool isSelected = false;

    [RelayCommand]
    void Close() => closeCallback?.Invoke(this);

    public abstract PackIconKind Icon { get; }

    public Page? Visual { get; set; }

    public IEnvironmentContextViewModel? ParentContext => parentContext;
}
