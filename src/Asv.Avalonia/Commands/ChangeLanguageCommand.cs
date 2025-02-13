using System.Composition;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
public class ChangeLanguageCommand : ICommandFactory
{
    #region Static

    public const string Id = "language.change";

    private static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = RS.ChangeLanguageCommand_CommandInfo_Name,
        Description = RS.ChangeLanguageCommand_CommandInfo_Description,
        Icon = MaterialIconKind.Translate,
        DefaultHotKey = null,
    };

    #endregion

    private readonly ILocalizationService _svc;

    [ImportingConstructor]
    public ChangeLanguageCommand(ILocalizationService svc)
    {
        ArgumentNullException.ThrowIfNull(svc);
        _svc = svc;
    }

    public ICommandInfo Info => StaticInfo;

    public bool CanExecute(IRoutable context, IPersistable? parameter)
    {
        return true;
    }

    public ValueTask<IPersistable?> Execute(
        IRoutable context,
        IPersistable? parameter,
        CancellationToken cancel = default
    )
    {
        if (parameter is Persistable<string> memento)
        {
            // execute with parameter
            var oldValue = _svc.CurrentLanguage.Value.Id;
            var language = _svc.AvailableLanguages.FirstOrDefault(x => x.Id == memento.Value);
            if (language is not null)
            {
                _svc.CurrentLanguage.Value = language;
            }

            return ValueTask.FromResult<IPersistable?>(new Persistable<string>(oldValue));
        }
        else
        {
            // execute without parameter
            var oldValue = _svc.CurrentLanguage.Value.Id;
            var temp = _svc.AvailableLanguages.ToList();
            var index = temp.IndexOf(_svc.CurrentLanguage.Value);
            index++;
            if (index >= temp.Count)
            {
                index = 0;
            }

            _svc.CurrentLanguage.Value = temp[index];
            return ValueTask.FromResult<IPersistable?>(new Persistable<string>(oldValue));
        }
    }

    public async ValueTask Undo(
        IRoutable context,
        IPersistable? parameter,
        CancellationToken cancel = default
    )
    {
        await Execute(context, parameter, cancel);
    }
}
