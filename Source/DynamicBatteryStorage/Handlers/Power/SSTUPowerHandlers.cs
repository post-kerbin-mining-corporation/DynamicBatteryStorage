using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class SSTUSolarPanelDeployablePowerHandler: ModuleDataHandler
{
  public SSTUSolarPanelDeployablePowerHandler()
  {
    solarEfficiencyEffects = true;
    visible = true;
    simulated = true;
    timewarpFunctional = true;
    producer = true;
    consumer = false;
  }

  public override bool Initialize(PartModule pm)
  {
    base.Initialize(pm);
  }

  protected override double GetValueEditor()
  {
        return 0d;
  }
  protected override double GetValueFlight()
  {
    return 0d;
  }
}
public class SSTUSolarPanelStaticPowerHandler: ModuleDataHandler
{
  public SSTUSolarPanelStaticPowerHandler()
  {
    solarEfficiencyEffects = true;
    visible = true;
    simulated = true;
    timewarpFunctional = true;
    producer = true;
    consumer = false;
  }

  public override bool Initialize(PartModule pm)
  {
    base.Initialize(pm);
  }

  protected override double GetValueEditor()
  {
        return 0d;
  }
  protected override double GetValueFlight()
  {
    return 0d;
  }
}
public class SSTUResourceBoiloffPowerHandler: ModuleDataHandler
{
  public SSTUResourceBoiloffPowerHandler()
  {
    solarEfficiencyEffects = false;
    visible = true;
    simulated = true;
    timewarpFunctional = true;
    producer = false;
    consumer = true;
  }

  public override bool Initialize(PartModule pm)
  {
    base.Initialize(pm);
  }

  protected override double GetValueEditor()
  {
        return 0d;
  }
  protected override double GetValueFlight()
  {
    return 0d;
  }
}
