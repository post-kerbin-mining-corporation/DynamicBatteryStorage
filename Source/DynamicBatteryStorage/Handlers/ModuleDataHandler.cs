using System;

namespace DynamicBatteryStorage
{
  public abstract class ModuleDataHandler
  {
    // The associated PartModule to monitor
    protected PartModule pm;
    // The config data for this data handler type
    protected HandlerModuleData data;
    // Is this module visible in the UI?
    protected bool visible = true;
    // Should this module be included in the DBS simulations?
    protected bool simulated = true;
    // Does this module generate/produce during timewarp?
    protected bool timewarpFunctional = true;
    // Is this a producer of things?
    protected bool producer = false;
    // Is this a consumer of things?
    protected bool consumer = false;
    // Is this affected by solar distance?
    protected bool solarEfficiencyEffects = false;
    // Current solar distance factor
    protected float solarEfficiency = 1.0f;

    #region Accessors
    public PartModule PM
    {
      get { return pm; }
      set { pm = value; }
    }
    public bool Simulated
    {
      get { return simulated; }
      set { simulated = value; }
    }
    public bool Visible
    {
      get { return visible; }
      set { visible = value; }
    }
    public bool TimewarpFunctional
    {
      get { return timewarpFunctional; }
    }
    public bool Producer
    {
      get { return producer; }
    }
    public bool Consumer
    {
      get { return consumer; }
    }
    public bool AffectedBySunDistance
    {
      get { return solarEfficiencyEffects; }
    }
    public float SolarEfficiency
    {
      get { return solarEfficiency; }
      set { solarEfficiency = value; }
    }
    #endregion Accessors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data">The config-loaded data for this module type </param>
    public ModuleDataHandler(HandlerModuleData moduleData)
    {
      data = moduleData;

      solarEfficiencyEffects = moduleData.solarEfficiencyEffects;
      consumer = moduleData.consumer;
      producer = moduleData.producer;
      timewarpFunctional = moduleData.continuous;
      simulated = moduleData.simulated;
      visible = moduleData.visible;
    }
    /// <summary>
    /// Initializer - should cache relevant items. Should also validate whether this
    /// instance of the PM is really valid - ie check a converter module for actual power use
    /// </summary>
    /// <param name="module">The PartModule to monitor </param>
    public virtual bool Initialize(PartModule module)
    {
      pm = module;
      return true;
    }

    /// <summary>
    /// Gets the monitored value
    /// </summary>
    public double GetValue()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        return GetValueFlight();
      }
      if (HighLogic.LoadedSceneIsEditor)
      {
        return GetValueEditor();
      }
      return 0d;
    }

    /// <summary>
    /// Gets the monitored value in flight
    /// </summary>
    protected abstract double GetValueFlight();
    /// <summary>
    /// Gets the monitored value in the editor
    /// </summary>
    protected abstract double GetValueEditor();
    /// <summary>
    /// Accessor for the name of the module.
    /// </summary>
    public string ModuleName()
    {
      return pm.moduleName;
    }

    /// <summary>
    /// Gets the part title. Override for custom work
    /// </summary>
    public virtual string PartTitle()
    {
      return pm.part.partInfo.title;
    }

    /// <summary>
    /// Returns a string representation of the module
    /// </summary>
    public override string ToString()
    {
      return String.Format("{0}: {1} {2:F2} units/s", ModuleName(), producer ? "Producing" : "Consuming", GetValue());
    }

  }
}
