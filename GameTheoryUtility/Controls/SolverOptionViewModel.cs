using CommunityToolkit.Mvvm.ComponentModel;

namespace GameTheoryUtility.Controls;

public partial class SolverOptionViewModel(string name, Action<bool> set, bool initial) : ObservableObject
{
    public string Name => name;
    [ObservableProperty]
    bool _isEnabled = initial;
    partial void OnIsEnabledChanged(bool value) => set(value);
}
