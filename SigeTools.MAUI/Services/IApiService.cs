namespace SigeTools.MAUI.Services
{
    public enum Priority
    {
        Background,
        UserInitiated,
        Speculative
    }

    public interface IApiService<T>
    {
        T GetApi(Priority priority);
    }
}
