using System;
using System.Linq;

namespace SigeTools.MAUI.Model
{
    public enum Perfil
    {
        All = 0,
        Dashboard = 1,
        DashboardPlusCheckList = 2,
        DashboardPlusItems = 3,
        RestaurantWaiter = 4
    }

    public class Usuario
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserEmail { get; set; }
        public string UserTitle { get; set; }
        public Int16 UserPerfil { get; set; }
        public Int16 UserPin { get; set; }
        public string StoreId { get; set; }
        public Perfil Perfil {
            get {

                if (UserPerfil < 0 || UserPerfil > 4)
                {
                    return Perfil.All;
                }
                return (Perfil)UserPerfil;
            }
        }
    }

    
}
