using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

public static class ActionViewModelMixin
{
    public static IActionViewModel CreateAction(
        this ICommandInfo info,
        ILoggerFactory loggerFactory
    )
    {
        var model = new ActionViewModel(info.Id, loggerFactory)
        {
            Icon = info.Icon,
            Header = info.Name,
            Description = info.Description,
        };
        model.Command = new BindableAsyncCommand(info.Id, model);
        return model;
    }
}
