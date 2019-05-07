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
        // Must initialize the handler, eg cache things for better speed
        public virtual void Initialize(PartModule module)
        {
            pm = module;
        }

        // Must determine if the part is currently producing something or not
        public virtual bool IsProducer()
        {
          return true;
        }
        public virtual double GetValue()
        {
          return 0d;
        }
        public string ModuleName()
        {
            return pm.moduleName;
        }
        public string PartTitle()
        {
            return pm.part.partInfo.title;
        }

        public virtual string ToString()
        {
            string pState = "Consuming";
            if (IsProducer):
              pState = "Producing";
            return String.Format("{0}: {1} {2} EC/s", ModuleName(), pState);
        }

    }
}
