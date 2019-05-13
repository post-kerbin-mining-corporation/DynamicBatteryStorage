using System;
using DynamicBatteryStorage;

namespace DynamicBatteryStorage.UI
{
    public class UIWindow
    {
        public bool Drawn
        {
            get { return drawn; }
            set { drawn = value; }
        }
        protected int windowID = 0;
        protected DynamicBatteryStorageUI host;
        protected bool drawn = false;


        public UIBaseWindow UIHost { get { return host; } }

        public UIWindow(System.Random randomizer, DynamicBatteryStorageUI uiHost)
        {
            host = uiHost;
            windowID = randomizer.Next();
        }


    }
}
