# Dynamic Battery Storage

A mod for Kerbal Space Program, intended to ease vessel construction and solve problems related to power flow. Effectively required by Near Future Electrical, Cryogenic Engines, Kerbal  Atomics and Cryogenic Tanks.

* [Features](#features)
* [Dependencies](#dependencies)
* [Installation](#installation)
* [Contributing](#contributing)
* [License](#licensing)

## Features

Dynamic Battery Storage has two components - Vessel Systems Management and Electrical Timewarp Compensation.

### Vessel Systems Management

The mod provides a vessel monitoring user interface to assist in looking at your ship's electrical and thermal properties. It is difficult in KSP to look at power **flows** - how much power you are generating versus how much power you are using - when you start to add a large number of electricity consumers and producers to your vessel. This UI lets you see summaries of how your vessel's power sources and sinks are interacting and lets you size batteries and generators to respond to actual power loads. In a nutshell, it...

* Shows whole-vessel power flows, separated into consumption and generation, in flight or in the VAB
* Estimates time to drain or time to charge batteries
* Can drill down to part categories (eg. Solar Panels, Harvesters, etc.)
* Can drill down to turn individual parts on or off for simulation purposes
* The VAB interface has a tool to simulate distance effects on solar panel efficiency

A similar interface can be applied to thermal flows - specifically, core heat, as used in resource converters, resource harvesters and radiators, plus mod parts like fission/fusion reactors. This thermal UI ...

* Shows whole-vessel core heat flows, separated into consumption and generation, in flight or in the VAB
* Can drill down to part categories (eg. Radiators, Harvesters, etc.)
* Can drill down to turn individual parts on or off for simulation purposes
* **NOTE:** does not handle non-core heat (eg. re-entry, engines, solar)
* **NOTE:** does not make a distinction between adjacent-only radiators and full-vessel radiators

### Electrical Timewarp Compensation

This mod dynamically adjusts ElectricCharge (EC) storage to combat the game's awful handling of resource generation/draw mechanics at high timewarp. It should dramatically reduce instances of bad ship consequences due to apparent EC loss at these times.

The mod functions by choosing a electricity-containing part as the 'buffer', and then expanding or contracting its storage temporarily when high timewarp factors are engaged. The size of the buffer is calculated by determining the total amount of electrical draw that all parts contribute in a single physics frame, and determining the total electrical production. If there is truly enough production to meet demand, the storage will be expanded so no timewarp related effects occur.

## Supported Mods/Modules

At the moment, DBS needs to be made aware of modules so it can get data from them.

### Supported Power Modules

* **ModuleDeployableSolarPanel** (Stock)
* **ModuleGenerator** (Stock)
* **ModuleResourceConverter** (Stock)
* **ModuleActiveRadiator** (Stock)
* **ModuleResourceHarvester** (Stock)
* **ModuleCommand** (Stock)
* **ModuleLight** (Stock)
* **ModuleDataTransmitter** (Stock)
* **ModuleEngines** (Stock)
* **ModuleEnginesFX** (Stock)
* **ModuleAlternator** (Stock)
* **ModuleCurvedSolarPanel** (Near Future Solar)
* **FissionGenerator** (Near Future Electrical)
* **DischargeCapacitor** (Near Future Electrical)
* **ModuleRadioisotopeGenerator** (Near Future Electrical)
* **ModuleCryoTank** (CryoTanks)
* **ModuleDeployableCentrifuge** (Stockalike Station Parts Expansion Redux)
* **RealBattery** (RealBattery)
* **KopernicusSolarPanel** (Kopernicus)
* **SoilRecycler** (Snacks!)
* **SnackProcessor** (Snacks!)
* **ModuleKerbalHealth** (Kerbal Health)
* **ModuleSignalDelay** (Signal Delay)

### Supported Heat Modules

* **ModuleResourceConverter** (Stock)
* **ModuleActiveRadiator** (Stock)
* **ModuleResourceHarvester** (Stock)
* **FissionGenerator** (Near Future Electrical)
* **FissionFlowRadiator** (Near Future Electrical)

Adding support for your mod is not difficult - contact me if you wish to do so and we can figure it out.

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
