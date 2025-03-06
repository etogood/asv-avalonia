namespace Asv.Avalonia;

public static class ActionViewModelMixin
{
    public static IActionViewModel CreateAction(this ICommandInfo info)
    {
        var model = new ActionViewModel(info.Id)
        {
            Icon = info.Icon,
            Header = info.Name,
            Description = info.Description,
        };
        model.Command = new BindableAsyncCommand(info.Id, model);
        return model;
    }
}
