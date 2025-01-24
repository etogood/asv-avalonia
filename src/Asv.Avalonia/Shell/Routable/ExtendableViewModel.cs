using System.Collections.Immutable;
using R3;

namespace Asv.Avalonia;

public class ExtendableViewModel<TSelfInterface> : RoutableViewModel, IExtendable
    where TSelfInterface : class
{
    private readonly BindableReactiveProperty<bool> _isLoading;
    private readonly BindableReactiveProperty<string?> _loadingMessage;
    private readonly ImmutableArray<IExtensionFor<TSelfInterface>> _extentions;

    protected ExtendableViewModel(string id, params IEnumerable<IExtensionFor<TSelfInterface>> extensions)
        : base(id)
    {
        _isLoading = new BindableReactiveProperty<bool>(false);
        _loadingMessage = new BindableReactiveProperty<string?>(null);

        var context = this as TSelfInterface ??
                      throw new Exception($"Class {GetType().FullName} must implement {nameof(TSelfInterface)}");
        _extentions = [..extensions];
        BeginLoadExtensions(_extentions, context);
    }

    public IReadOnlyBindableReactiveProperty<bool> IsLoading => _isLoading;

    public IReadOnlyBindableReactiveProperty<string?> LoadingMessage => _loadingMessage;

    protected virtual ValueTask AfterLoadExtensions()
    {
        return ValueTask.CompletedTask;
    }

    private async void BeginLoadExtensions(IEnumerable<IExtensionFor<TSelfInterface>> extensions, TSelfInterface context)
    {
        try
        {
            _isLoading.Value = true;
            foreach (var extension in extensions)
            {
                try
                {
                    await extension.Extend(context);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }

            await AfterLoadExtensions();
        }
        catch (Exception e)
        {

        }
        finally
        {
            _isLoading.Value = false;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _isLoading.Dispose();
            _loadingMessage.Dispose();
            foreach (var item in _extentions)
            {
                item.Dispose();
            }
        }

        base.Dispose(disposing);
    }
}