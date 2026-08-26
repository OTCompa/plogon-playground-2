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
    private unsafe delegate void ProcessPacketRSVDataDelegate(byte* packet);
    [Signature("44 8B 09 4C 8D 41 34", DetourName = nameof(ProcessPacketRSVDataDetour))]
    private Hook<ProcessPacketRSVDataDelegate>? RSVHook { get; set; } = null!;


    public Hook()
    {
        Plugin.GameInteropProvider.InitializeFromAttributes(this);
        RSVHook?.Enable();
        Plugin.Log.Info("Hello");
    }

    public void Dispose()
    {
        RSVHook?.Dispose();
    }
    private unsafe void ProcessPacketRSVDataDetour(byte* packet)
    {
        RSVHook!.Original(packet);
        var key = MemoryHelper.ReadStringNullTerminated((nint)(packet + 4));
        var val = MemoryHelper.ReadString((nint)(packet + 0x34), *(int*)packet);
        Plugin.Log.Debug($"RSV: {key} {val}");
    }


}
