using Refit;
using SigeTools.MAUI.Services;

namespace SigeTools.MAUI.Services
{
    public class ApiService<T> : IApiService<T>
    {
        public ApiService(string apiBaseAddress)
        {
            Func<HttpMessageHandler, T> createClient = messageHandler =>
            {
                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(apiBaseAddress)
                };

                return RestService.For<T>(client);
            };

            /*
            _background = new Lazy<T>(() => createClient(
                new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.Background)));

            _userInitiated = new Lazy<T>(() => createClient(
                new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.UserInitiated)));

            _speculative = new Lazy<T>(() => createClient(
                new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.Speculative)));
            */

            _background = new Lazy<T>(() => createClient(new HttpClientHandler() ));

            _userInitiated = new Lazy<T>(() => createClient(new HttpClientHandler()));

            _speculative = new Lazy<T>(() => createClient(new HttpClientHandler()));
        }

        private readonly Lazy<T> _background;
        private readonly Lazy<T> _userInitiated;
        private readonly Lazy<T> _speculative;

        private T Background
        {
            get { return _background.Value; }
        }

        private T UserInitiated
        {
            get { return _userInitiated.Value; }
        }

        private T Speculative
        {
            get { return _speculative.Value; }
        }

        public T GetApi(Priority priority)
        {
            switch (priority)
            {
                case Priority.Background:
                    return Background;
                case Priority.UserInitiated:
                    return UserInitiated;
                case Priority.Speculative:
                    return Speculative;
                default:
                    return UserInitiated;
            }
        }
    }
}
