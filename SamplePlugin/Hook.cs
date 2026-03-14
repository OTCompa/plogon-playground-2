using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamplePlugin;

internal unsafe class Hook : IDisposable
{
    private delegate IntPtr TextNameDelegate(IntPtr a1, UInt64 a2, UInt64 a3);
    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B F1 49 8B D8 48 8B 0D ?? ?? ?? ?? 48 8B FA E8", DetourName = nameof(TextNameDetour))]
    private Hook<TextNameDelegate>? TextNameHook { get; set; } = null!;


    private delegate IntPtr PlistNameDelegate(IntPtr blacklistManager, UInt64 accountId, UInt64 contentId);
    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 49 8B F8 48 8B 0D ?? ?? ?? ?? 48 8B F2 E8 ?? ?? ?? ?? 48 85 C0 0F 84", DetourName = nameof(PlistDetour))]
    private Hook<PlistNameDelegate>? PlistHook { get; set; } = null!;

    private readonly Utf8String* name1 = Utf8String.FromString("test123");
    private readonly Utf8String* name2 = Utf8String.FromString("test123");
    public Hook()
    {
        Plugin.GameInteropProvider.InitializeFromAttributes(this);
        TextNameHook?.Enable();
        PlistHook?.Enable();
        Plugin.Log.Info("Hello");
    }

    public void Dispose()
    {
        PlistHook?.Dispose();
        TextNameHook?.Dispose();
        name1->Dtor(true);
        name2->Dtor(true);
    }

    private IntPtr TextNameDetour(IntPtr a1, UInt64 accountId, UInt64 contentId)
    {
        var pmember = GroupManager.Instance()->MainGroup.GetPartyMemberByContentId(contentId);
        Plugin.Log.Information($"TextName called: {accountId}, {contentId}");
        var ret = TextNameHook!.Original(a1, accountId, contentId);
        Plugin.Log.Information($"ret: {ret:X02}");
        //return (IntPtr)name1;
        if (pmember != null)
        {
            return (IntPtr)name1;
        }
        else
        {
            return ret;
        }
        //return a1 + 848;
        //TestHook!.Original(a1, a2);
    }

    private IntPtr PlistDetour(IntPtr blacklistManager, UInt64 accountId, UInt64 contentId)
    {
        var pmember = GroupManager.Instance()->MainGroup.GetPartyMemberByContentId(contentId);
        Plugin.Log.Information($"Plist called: {accountId}, {contentId}");
        var ret = PlistHook!.Original(blacklistManager, accountId, contentId);
        Plugin.Log.Information($"ret: {ret}");
        if (pmember != null)
        {
            return (IntPtr)name2;
        } else
        {
            return ret;
        }
        //TestHook!.Original(a1, a2);
    }
}
