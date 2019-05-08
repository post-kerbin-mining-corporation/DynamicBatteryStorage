using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using NearFutureElectrical;
using KSP.Localization;

namespace NearFutureElectrical.UI
{

    public class UIWidget
    {

      UIBaseWindow uiHost;

      public UIBaseWindow UIHost { get {return uiHost;}}

      public UIWidget(UIBaseWindow uiBase)
      {
        uiHost = uiBase;
      }

      protected virtual void Localize()
      {

      }

    }

}
