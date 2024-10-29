using PropertyChanged;

namespace SigeTools.MAUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class BaseViewModel:IInitializeAsync
{
    protected INavigationService NavigationService { get; }
    protected IPageDialogService PageDialogService { get; }
    public bool IsBusy { get; set; }

    public bool IsNotBusy { get; set; }

    public BaseViewModel(INavigationService navigationService,IPageDialogService pageDialogService)
    {
        NavigationService = navigationService;
        PageDialogService = pageDialogService;
    }

    public virtual Task InitializeAsync(INavigationParameters parameters)
    {
        return Task.CompletedTask;
    }
}