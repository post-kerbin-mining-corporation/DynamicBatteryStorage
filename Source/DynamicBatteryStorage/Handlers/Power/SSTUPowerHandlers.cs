using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class SSTUSolarPanelDeployablePowerHandler: ModuleDataHandler
{
  public override void Initialize(PartModule pm)
  {
    base.Initialize(pm);
  }

  public override double GetValue()
  {
    double results = 0d;
    double.TryParse(pm.Fields.GetValue("ECOutput").ToString(), out results);
    return results;
  }

  public override double GetValue(float scalar)
  {
    return GetValue() * scalar;
  }
  public override bool AffectedBySunDistance()
  {
    return true;
  }
}
public class SSTUSolarPanelStaticPowerHandler: ModuleDataHandler
{
  public override void Initialize(PartModule pm)
  {
    base.Initialize(pm);
  }

  public override double GetValue()
  {
    double results = 0d;
    if (HighLogic.LoadedSceneIsEditor)
      double.TryParse(pm.Fields.GetValue("resourceAmount").ToString(), out results);
    if (HighLogic.LoadedSceneIsFlight)
      double.TryParse(pm.Fields.GetValue("energyFlow").ToString(), out results);
    return results;
  }

  public override double GetValue(float scalar)
  {
    return GetValue() * scalar;
  }
  public override bool AffectedBySunDistance()
  {
    return true;
  }
}
public class SSTUResourceBoiloffPowerHandler: ModuleDataHandler
{
  public override void Initialize(PartModule pm)
  {
    base.Initialize(pm);
  }

  public override double GetValue()
  {
    double results = 0d;
    if (HighLogic.LoadedSceneIsEditor)
      double.TryParse(pm.Fields.GetValue("guiECCost").ToString(), out results);
    if (HighLogic.LoadedSceneIsFlight)
      double.TryParse(pm.Fields.GetValue("guiECCost").ToString(), out results);
    return results;
  }
  public override bool IsProducer()
  {
    return false;
  }
}
