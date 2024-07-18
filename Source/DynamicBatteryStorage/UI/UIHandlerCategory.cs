﻿using System;
using System.Collections.Generic;
using KSP.Localization;

namespace DynamicBatteryStorage
{
  /// <summary>
  /// Defines a part module category for the UI
  /// </summary>
  public class UIHandlerCategory
  {
    public string name;
    public List<string> handledModules;
    public string title;

    public UIHandlerCategory(ConfigNode node)
    {
      Load(node);
    }
    public void Load(ConfigNode node)
    {
      name = node.GetValue("name");
      title = Localizer.Format(node.GetValue("title"));

      handledModules = node.GetValuesList("module");
      if (Settings.DebugLoading)
        Utils.Log(String.Format("[Settings]: Loaded {0}", this.ToString()));
    }

    public override string ToString()
    {
      return String.Format("{0}", name);
    }
  }

}