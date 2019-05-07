using System;
using System.Collections.Generic;
using DynamicBatteryStorage;

namespace DynamicBatteryStorage.UI
{
  public class UIThermalView
  {

    protected DynamicBatteryStorageUI host;

    #region GUI Strings
    #endregion



    public UIElectricalView(DynamicBatteryStorageUI uiHost)
    {
      host = uiHost;
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: [ThermalView]: New instance created");
    }

    public void Draw()
    {
    }

    public void Update()
    {
    }

  }
}
