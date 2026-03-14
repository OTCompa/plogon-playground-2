using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.String;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamplePlugin;

internal unsafe class Hook : IDisposable
{
    private delegate void HookDelegate(IntPtr a1, IntPtr a2);
    [Signature("48 89 91 ?? ?? ?? ?? C3 ?? ?? ?? ?? ?? ?? ?? ?? 80 A1", DetourName = nameof(HookDetour))]
    private Hook<HookDelegate>? TestHook { get; set; } = null!;

    public Hook()
    {
        Plugin.GameInteropProvider.InitializeFromAttributes(this);
        TestHook?.Enable();
        Plugin.Log.Info("Hello");
    }

    public void Dispose()
    {
        TestHook?.Dispose();
    }

    private void HookDetour(IntPtr a1, IntPtr a2, IntPtr a3)
    {
        Plugin.Log.Information($"Hooked function called: {a1:X02}, {a2:X02}");
        //var ret = TestHook!.Original(a1, a2, a3);
        //Plugin.Log.Information($"ret: {ret}");
        //return ret;
        TestHook!.Original(a1, a2);
    }
}
