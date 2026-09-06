using Dalamud.Hooking;
using Dalamud.Memory;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Common.Lua;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamplePlugin;

internal unsafe class Hook : IDisposable
{
    /*
     * For future reference
     * camera found by checking non null FFXIV::Client::Graphics::Scene::CameraManager
     * sig found by checking what modifies FFXIV::Client::Graphics::Scene::Camera during forced cs
     */
    private unsafe delegate void SpectatorCameraDelegate(byte* a1);
    [Signature("40 56 48 83 EC 50 80 39 00 ", DetourName = nameof(ProcessPacketRSVDataDetour))]
    private Hook<SpectatorCameraDelegate>? SpectatorHook { get; set; } = null!;


    public Hook()
    {
        Plugin.GameInteropProvider.InitializeFromAttributes(this);
        SpectatorHook?.Enable();
        Plugin.Log.Info("Hello");
    }

    public void Dispose()
    {
        SpectatorHook?.Dispose();
    }
    private unsafe void ProcessPacketRSVDataDetour(byte* a1)
    {
        //SpectatorHook!.Original(a1);
        Plugin.Log.Debug("a");
    }


}
