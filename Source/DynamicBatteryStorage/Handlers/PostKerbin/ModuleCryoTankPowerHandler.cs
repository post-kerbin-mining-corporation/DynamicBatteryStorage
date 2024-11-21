using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicBatteryStorage
{
  // CryoTank
  public class ModuleCryoTankPowerHandler : ModuleDataHandler
  {

    string[] fuels;
    public ModuleCryoTankPowerHandler(HandlerModuleData moduleData) : base(moduleData)
    { }

    public override bool Initialize(PartModule pm)
    {
      bool visible = base.Initialize(pm);
      GetFuelTypes();
      return visible;
    }

    protected void GetFuelTypes()
    {
      ConfigNode node = ReloadModuleNode();
      ConfigNode[] fuelNodes = node.GetNodes("BOILOFFCONFIG");

      fuels = new string[fuelNodes.Length];
      for (int i = 0; i < fuelNodes.Length; i++)
      {
        fuels[i] = fuelNodes[i].GetValue("FuelName");
      }
    }

    ConfigNode ReloadModuleNode()
    {
      ConfigNode cfg = null;
      foreach (UrlDir.UrlConfig pNode in GameDatabase.Instance.GetConfigs("PART"))
      {
        if (pNode.name.Replace("_", ".") == pm.part.partInfo.name)
        {
          List<ConfigNode> cryoNodes = pNode.config.GetNodes("MODULE").ToList().FindAll(n => n.GetValue("name") == data.handledModule);
          if (cryoNodes.Count > 1)
          {
            try
            {
              ConfigNode node = cryoNodes.Single(n => n.GetValue("moduleID") == data.handledModule);
              cfg = node;
            }
            catch (InvalidOperationException)
            {
              // Thrown if predicate is not fulfilled, ie moduleName is not unqiue
              Utils.Log(String.Format("[ModuleCryoTankPowerHandler]: Critical configuration error: Multiple ModuleCryoTank nodes found with identical or no moduleName"), Utils.LogType.Handlers);
            }
            catch (ArgumentNullException)
            {
              // Thrown if ModuleCryoTank is not found (a Large Problem (tm))
              Utils.Log(String.Format("[ModuleCryoTankPowerHandler]: Critical configuration error: No ModuleCryoTank nodes found in part"), Utils.LogType.Handlers);
            }
          }
          else
          {
            cfg = cryoNodes[0];
          }
        }
      }
      return cfg;
    }

    protected override double GetValueEditor()
    {
      double resAmt = GetMaxFuelAmt();
      double results = 0d;
      if (resAmt > 0d)
      {
        visible = true;
        Utils.TryGetField(pm, "currentCoolingCost", out results);
      }
      else
      {
        visible = false;
      }
      return results * -1.0d;
    }
    protected override double GetValueFlight()
    {
      Utils.TryGetField(pm, "currentCoolingCost", out double results);
      return results * -1.0d;
    }
    protected double GetMaxFuelAmt()
    {
      double max = 0d;
      if (fuels == null || fuels[0] == null)
        GetFuelTypes();

      for (int i = 0; i < fuels.Length; i++)
      {
        int id = PartResourceLibrary.Instance.GetDefinition(fuels[i]).id;
        PartResource res = pm.part.Resources.Get(id);
        if (res != null)
          max += res.maxAmount;
      }
      return max;
    }

  }
}
