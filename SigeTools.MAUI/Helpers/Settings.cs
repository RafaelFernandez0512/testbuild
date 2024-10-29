namespace SigeTools.MAUI.Helpers
{
    public static class AppSettings
    {
        public static string CurrentUser
        {
            get { return Preferences.Get(nameof(CurrentUser), string.Empty); }
            set { Preferences.Set(nameof(CurrentUser), value); }
        }
        public static string UserProfile
        {
            get { return Preferences.Get(nameof(UserProfile), string.Empty); }
            set { Preferences.Set(nameof(UserProfile), value); }
        }
        public static string CurrentStore
        {
            get { return Preferences.Get(nameof(CurrentStore), string.Empty); }
            set { Preferences.Set(nameof(CurrentStore), value); }
        }
        public static string BaseServiceUrl
        {
            get { return Preferences.Get(nameof(BaseServiceUrl), @"http://192.168.0.101/sigesapi/api"); }
            set { Preferences.Set(nameof(BaseServiceUrl), value); }
        }
        public static string CompanyInfo
        {
            get { return Preferences.Get(nameof(CompanyInfo), string.Empty); }
            set { Preferences.Set(nameof(CompanyInfo), value); }
        }
    }
}
