using Asv.Common;
using Microsoft.Extensions.Logging;
using R3;

namespace Asv.Avalonia;

public sealed class HistoricalEnumProperty<TEnum> : HistoricalPropertyBase<Enum, TEnum>
    where TEnum : struct, Enum
{
    private bool _internalChange;
    private bool _externalChange;

    public HistoricalEnumProperty(
        NavigationId id,
        ReactiveProperty<Enum> modelValue,
        ILoggerFactory loggerFactory,
        IRoutable parent
    )
        : base(id, loggerFactory, parent)
    {
        ModelValue = modelValue;

        ViewValue = new BindableReactiveProperty<TEnum>().DisposeItWith(Disposable);
        IsSelected = new BindableReactiveProperty<bool>().DisposeItWith(Disposable);
        ViewValue.EnableValidation(ValidateValue);

        _internalChange = true;
        ViewValue.SubscribeAwait(OnChangedByUser, AwaitOperation.Drop).DisposeItWith(Disposable);
        _internalChange = false;

        ModelValue.Subscribe(OnChangeByModel).DisposeItWith(Disposable);
    }

    protected override Exception? ValidateValue(TEnum userValue)
    {
        return null;
    }

    protected override async ValueTask OnChangedByUser(TEnum userValue, CancellationToken cancel)
    {
        if (_internalChange)
        {
            return;
        }

        _externalChange = true;
        var newValue = new StringArg(Enum.GetName(userValue) ?? string.Empty);
        await this.ExecuteCommand(ChangeEnumPropertyCommand.Id, newValue, cancel: cancel);
        _externalChange = false;
    }

    protected override void OnChangeByModel(Enum modelValue)
    {
        if (_externalChange)
        {
            return;
        }

        _internalChange = true;
        if (modelValue is not TEnum newEnum)
        {
            throw new Exception($"{modelValue} is not a valid enum type for {nameof(TEnum)}");
        }

        ViewValue.OnNext(newEnum);
        _internalChange = false;
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    public override BindableReactiveProperty<TEnum> ViewValue { get; }
    public override BindableReactiveProperty<bool> IsSelected { get; }
    public override ReactiveProperty<Enum> ModelValue { get; }

    public TEnum[] EnumItems => Enum.GetValues<TEnum>();
}
