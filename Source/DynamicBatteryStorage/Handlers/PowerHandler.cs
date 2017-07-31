using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace DynamicBatteryStorage
{
    public class PowerHandler
    {
        protected PartModule pm;
        // Must initialize the handler, eg cache things for better speed
        public virtual void Initialize(PartModule module)
        {
            pm = module;
        }
        // Must return the current power use/draw
        // Negative if consuming, positive if producing
        public virtual double GetPower()
        {
          return 0d;
        }

        // Must determine if the part is currently producing
        public virtual bool IsProducer()
        {
          return true;
        }
        public string ModuleName()
        {
            return pm.moduleName;
        }

    }
}
