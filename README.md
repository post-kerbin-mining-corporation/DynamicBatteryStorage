# Dynamic Battery Storage

A mod for Kerbal Space Program, intended to support my other projects. Effectively required by Near Future Electrical, CryoEngines, KerballAtomics and CryoTanks.

* [Features](#features)
* [Dependencies](#dependencies)
* [Installation](#installation)
* [External Compatibility](#features)
* [Contributing](#contributing)

## Features

This mod dynamically adjusts ElectricCharge (EC) storage to combat the game's awful handling of resource generation/draw mechanics at high timewarp. It should dramatically reduce instances of bad ship consequences due to EC loss at these times.

The mod functions by choosing a electricity-containing part as the 'buffer', and then expanding or contracting its storage temporarily when high timewarp factors are engaged. The size of the buffer is calculated by determining the total amount of electrical draw that all parts contribute in a single physics frame, and determining the total electrical production. If there is truly enough production to meet demand, the storage will be expanded so no timewarp related effects occur.

## Supported Mods/Modules

At the moment, DBS needs to be made aware of modules so it can get data from them to solve for the required buffers. This applies both to consumers of power and creators of power. The currently supported modules are:


* **ModuleDeployableSolarPanel** (Stock)
* **ModuleGenerator** (Stock)
* **ModuleResourceConverter** (Stock)
* **ModuleActiveRadiator** (Stock)
* **ModuleResourceHarvester** (Stock)
* **ModuleCurvedSolarPanel** (Near Future Solar)
* **FissionGenerator** (Near Future Electrical)
* **ModuleRadioisotopeGenerator** (Near Future Electrical)
* **ModuleCryoTank** (CryoTanks)
* **ModuleAntimatterTank** (Far Future Technologies)
* **RealBattery** (RealBattery)
* **KopernicusSolarPanel** (Kopernicus)

Adding support for your mod is fairly easy - consult [PowerHandler.cs](https://github.com/ChrisAdderley/DynamicBatteryStorage/blob/master/Source/DynamicBatteryStorage/Handlers/PowerHandlerTypes.cs) to see what to implement, and look at the examples in [StockPowerHandlers.cs](https://github.com/ChrisAdderley/DynamicBatteryStorage/blob/master/Source/DynamicBatteryStorage/Handlers/StockPowerHandlers.cs).

## Debugging

You can open the Debug window with `CTRL + SHIFT + K`. This will bring up a UII showing the list of consumers, producers and the buffer status. It will only enable at higher timewarps (1000x). Please provide shots of this in any bug report along with a KSP logfile

## Installation

To install, place the GameData folder inside your Kerbal Space Program folder. If asked to overwrite files, please do so.

NOTE: Do NOT rename or move folders within the GameData folder - this mod uses absolute paths to assets and will break if this happens.

## Contributing

I certainly accept pull requests. Please target all such things to the `dev` branch though!

## Licensing

MIT license:

Copyright (c) 2019 Chris Adderley
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
