using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;

namespace GameTheoryUtility;

public partial class NavigationManager(Page? defaultPage = null) : ObservableObject
{
    private readonly Stack<Page> _backStack = new();
    private readonly Stack<Page> _forwardStack = new();

    [ObservableProperty]
    private Page? _currentPage = defaultPage;
    public bool CanGoBack => _backStack.Count > 0;
    public bool CanGoForward => _forwardStack.Count > 0;

    partial void OnCurrentPageChanged(Page? oldValue, Page? newValue) => Navigated?.Invoke(oldValue, newValue);

    public void Navigate(Page page)
    {
        if (page == CurrentPage)
            return;
        if (CurrentPage != null)
            _backStack.Push(CurrentPage);

        _forwardStack.Clear();
        CurrentPage = page;
        RaiseCanGoChanged();
    }

    [RelayCommand]
    public void GoBack()
    {
        if (!CanGoBack)
            return;

        _forwardStack.Push(CurrentPage!);
        CurrentPage = _backStack.Pop();
        RaiseCanGoChanged();
    }

    [RelayCommand]
    public void GoForward()
    {
        if (!CanGoForward)
            return;

        _backStack.Push(CurrentPage!);
        CurrentPage = _forwardStack.Pop();
        RaiseCanGoChanged();
    }

    private void RaiseCanGoChanged()
    {
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
    }

    public void Remove(Page page, Page substitute)
    {
        var newBackStack = new Stack<Page>(_backStack.Where(p => p != page));
        _backStack.Clear();
        foreach (var p in newBackStack)
            _backStack.Push(p);

        var newForwardStack = new Stack<Page>(_forwardStack.Where(p => p != page));
        _forwardStack.Clear();
        foreach (var p in newForwardStack)
            _forwardStack.Push(p);

        if (CurrentPage == page)
        {
            if (_backStack.Count > 0)
            {
                CurrentPage = _backStack.Pop();
                _forwardStack.Clear();
            }
            else if (_forwardStack.Count > 0)
                CurrentPage = _forwardStack.Pop();
            else
                CurrentPage = substitute;
        }

        RaiseCanGoChanged();
    }

    public void Clear()
    {
        _backStack.Clear();
        _forwardStack.Clear();
        RaiseCanGoChanged();
    }

    public event Action<Page?, Page?>? Navigated;
}
