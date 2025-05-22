using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameTheoryUtility.Logic.Matrix.Representation;
using System.ComponentModel.DataAnnotations;

namespace GameTheoryUtility.Controls;

public partial class CellViewModel(IValueFormatterProxy value) : ObservableValidator
{
    readonly IValueFormatterProxy _value = value;

    bool _isValid = true;
    string _stringValue = value.Value;

    [CustomValidation(typeof(CellViewModel), nameof(Validate))]
    public string StringValue
    {
        get => _stringValue;
        set => SetProperty(ref _stringValue, value, true);
    }

    [RelayCommand]
    public void Save()
    {
        if (!_isValid)
            return;
        _value.Value = StringValue;
        StringValue = _value.Value;
    }

    public static ValidationResult Validate(string value, ValidationContext context)
    {
        var instance = (CellViewModel)context.ObjectInstance;
        if (!instance._value.Validate((string)value, out var error))
        {
            instance._isValid = false;
            return new ValidationResult(error);
        }
        instance._isValid = true;
        return ValidationResult.Success;
    }
}
