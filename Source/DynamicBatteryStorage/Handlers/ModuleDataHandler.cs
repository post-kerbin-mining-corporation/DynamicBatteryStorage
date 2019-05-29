using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{
  public class ModuleDataHandler
  {
    protected PartModule pm;
    protected bool visible = true;
    protected bool simulated = true;

    // Must initialize the handler, eg cache things for better speed
    public virtual void Initialize(PartModule module)
    {
      pm = module;
    }
    public bool Simulated
    {
      get {return simulated;}
      set {simulated = value;}
    }
    // Must determine if the part is currently producing something or not
    public virtual bool IsProducer()
    {
      return true;
    }
    public virtual bool IsVisible()
    {
      return visible;
    }
    public virtual bool AffectedBySunDistance()
    {
      return false;
    }
    public virtual double GetValue()
    {
      return 0d;
    }
    public virtual double GetValue(float scalar)
    {
      return GetValue();
    }
    public string ModuleName()
    {
      return pm.moduleName;
    }
    public virtual string PartTitle()
    {
      return pm.part.partInfo.title;
    }

    public override string ToString()
    {
      string pState = "Consuming";
      if (IsProducer())
        pState = "Producing";
      return String.Format("{0}: {1} {2} EC/s", ModuleName(), pState, GetValue());
    }

  }
}
