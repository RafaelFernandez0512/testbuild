using SigeTools.Helpers;

namespace SigeTools.Services
{
    public static class ServiceFactory
    {
        private static ISigeService sigeService;
        public static ISigeService SigeService
        {
            get
            {
                if (sigeService == null)
                {
                    sigeService = new SigeService();
                }
                return sigeService;
            }
        }

        private static ISigeApi sigeApi;
        public static ISigeApi SigeApi
        {
            get
            {
                if (sigeApi == null)
                {
                    sigeApi = (new ApiService<ISigeApi>(Settings.BaseServiceUrl)).GetApi(Priority.UserInitiated);
                }
                return sigeApi;
            }
        }

    }
}
