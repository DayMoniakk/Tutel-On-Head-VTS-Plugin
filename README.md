# Tutel On Head
Allows you to attach your vtuber model to the head of a Live2D model in Unity.

This is a barebone implementation and will require some work to fit into your project. I left method in the script to easily control the behaviour of the plugin.

## Demo
(The cat is a Vtube Studio model and Hiyori is running inside Unity)

https://github.com/DayMoniakk/Tutel-On-Head-VTS-Plugin/assets/54359117/4312b84c-0ebd-4b32-97c9-7248fda4eb55

## Requirements
* [Live2D Cubism SDK for Unity](https://www.live2d.com/en/download/cubism-sdk/download-unity) (Official Website)
* [VTS-Sharp](https://assetstore.unity.com/packages/tools/integration/vts-sharp-203218) (Unity Asset Store)

## How to use
1. Import your Live2D model following the [official tutorial](https://docs.live2d.com/en/cubism-sdk-manual/cubism-sdk-for-unity/)
1. Import VTS-Sharp from the asset store
1. Add **TutelOnHeadPlugin.cs** to your project
1. Create a new empty gameobject and attach the **TutelOnHeadPlugin.cs** script on it
1. Fill the required field (except *Plugin Icon* which is not needed)
1. Open Vtube Studio, go to settings and scroll down until you see **Vtube Studio plugins**, then click **Start API**
1. Use the OBS Preview to adjust the scale of your vtuber model, once the plugin is connected to Vtube Studio the scale will be overriden by the plugin
1. Press play in unity
1. If your vtuber model isn't properly placed on the head feel free to adjust the **Model Offset** property in the inspector
