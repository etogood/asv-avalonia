using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public sealed class HistoricalStringProperty : HistoricalPropertyBase<string?, string?>
{
    private readonly ReactiveProperty<string?> _modelValue;
    private readonly IList<Func<string?, ValidationResult>> _validationRules = [];

    private bool _internalChange;
    private bool _externalChange;

    public override ReactiveProperty<string?> ModelValue => _modelValue;
    public override BindableReactiveProperty<string?> ViewValue { get; } = new();
    public override BindableReactiveProperty<bool> IsSelected { get; } = new();

    public HistoricalStringProperty(
        string id,
        ReactiveProperty<string?> modelValue,
        ILoggerFactory loggerFactory,
        IRoutable parent,
        IList<Func<string?, ValidationResult>>? validationRules = null
    )
        : base(id, loggerFactory, parent)
    {
        InternalInitValidationRules(validationRules);
        _modelValue = modelValue;
        ViewValue.EnableValidation(ValidateValue);

        _internalChange = true;
        _sub2 = ViewValue.SubscribeAwait(OnChangedByUser, AwaitOperation.Drop);
        _internalChange = false;

        _sub3 = _modelValue.Subscribe(OnChangeByModel);
    }

    public void AddValidationRule(Func<string?, ValidationResult> validationFunc)
    {
        _validationRules.Add(validationFunc);
    }

    protected override Exception? ValidateValue(string? userValue)
    {
        foreach (var rule in _validationRules)
        {
            var res = rule(userValue);
            if (res.IsFailed)
            {
                return res.ValidationException;
            }
        }

        return null;
    }

    protected override async ValueTask OnChangedByUser(string? userValue, CancellationToken cancel)
    {
        if (ViewValue.HasErrors)
        {
            return;
        }

        if (_internalChange)
        {
            return;
        }

        _externalChange = true;
        var newValue = new StringArg(userValue ?? string.Empty);
        await this.ExecuteCommand(ChangeStringPropertyCommand.Id, newValue, cancel);
        _externalChange = false;
    }

    protected override void OnChangeByModel(string? modelValue)
    {
        if (_externalChange)
        {
            return;
        }

        _internalChange = true;
        ViewValue.OnNext(modelValue);
        _internalChange = false;
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    private void InternalInitValidationRules(
        IList<Func<string?, ValidationResult>>? validationRules
    )
    {
        if (validationRules is null)
        {
            return;
        }

        foreach (var rules in validationRules)
        {
            AddValidationRule(rules);
        }
    }

    #region Dispose

    private readonly IDisposable _sub2;
    private readonly IDisposable _sub3;

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _sub2.Dispose();
        _sub3.Dispose();
        ViewValue.Dispose();
        IsSelected.Dispose();
        _validationRules.Clear();
    }

    #endregion
}
