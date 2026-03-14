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
    private delegate IntPtr HookDelegate(IntPtr blacklistManager, UInt64 accountId, UInt64 contentId);
    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 49 8B F8 48 8B 0D ?? ?? ?? ?? 48 8B F2 E8 ?? ?? ?? ?? 48 85 C0 0F 84", DetourName = nameof(HookDetour))]
    private Hook<HookDelegate>? TestHook { get; set; } = null!;

    private readonly Utf8String* name1 = Utf8String.FromString("test123");
    public Hook()
    {
        Plugin.GameInteropProvider.InitializeFromAttributes(this);
        TestHook?.Enable();
        Plugin.Log.Info("Hello");
    }

    public void Dispose()
    {
        TestHook?.Dispose();
        name1->Dtor(true);
    }

    private IntPtr HookDetour(IntPtr blacklistManager, UInt64 accountId, UInt64 contentId)
    {
        Plugin.Log.Information($"Hooked function called: {accountId}, {contentId}");
        var ret = TestHook!.Original(blacklistManager, accountId, contentId);
        Plugin.Log.Information($"ret: {ret}");
        if (ret != 0)
        {
            return ret;
        }
        return (IntPtr)name1;
        //TestHook!.Original(a1, a2);
    }
}
