// my attempt at figuring out how this function works
// not really sure why this wouldn't include PCs too, at least in the plugin.. maybe I missed something?
// either way not super concerned since it looks kinda bad on PCs (try trusts/duty support for an example)
char __fastcall Client::UI::RaptureAtkModule_OnUpdate_NameplateCastbars(__int64 raptureAtkModule)
{
  __int64 v1RaptureAtkModule; // rdi
  __int64 raptureAtkModule; // rax
  __int64 castBarEnemyNumberArray; // r15
  __int64 raptureAtkModule; // rax
  unsigned int v5; // r14d
  __int64 castBarEnemyStringArray; // rax
  Component::GUI::StringArrayData *castBarEnemyStringArray; // r13
  __int64 castBarEnemySetting; // rax
  int castBarEnemySettingValue; // eax
  int currCastBarEnemyCount; // ebp
  __int64 namePlateObjectInfoOffset; // rbx
  __int64 objCtr; // r12
  __int64 temp13; // rsi
  __int64 currNamePlateObjectInfo; // rbx
  __int64 AsCharacter; // rax
  __int64 character; // rdi
  __int64 StatusManager; // rax
  char IsCastInProgress; // al
  __int64 character; // rdx
  __int64 CastInfo; // rax
  unsigned int SpellIdForAction; // esi
  Component::Exd::Sheets::Action *ActionRow_1; // rax
  Client::UI::UIModule *UIModule; // rax
  Client::UI::Misc::RaptureTextModule *raptureAtkModule24; // rbx
  char *temp25; // rax
  __int64 temp26; // rax
  float temp27; // xmm6_4
  __int64 temp28; // rax
  float temp29; // xmm0_4
  __int64 temp30; // r8
  float temp31; // xmm0_4
  float temp32; // xmm0_4
  __int64 temp33; // rax
  __int64 temp34; // rax
  unsigned int temp35; // edi
  Component::Exd::Sheets::Action *temp36; // rax
  Client::UI::UIModule *temp37; // rax
  Client::UI::Misc::RaptureTextModule *temp38; // rbx
  char *temp39; // rax
  __int64 NamePlateObjectInfoCount; // [rsp+30h] [rbp-88h]
  __int64 temp43; // [rsp+D0h] [rbp+18h]

  v1RaptureAtkModule = raptureAtkModule;
  Client::UI::UIModule uiModule = raptureAtkModule + 72760;
  raptureAtkModule = GetRaptureAtkModule2();
  castBarEnemyNumberArray = GetNumberArrayData(raptureAtkModule, 6i64);
  raptureAtkModule = GetRaptureAtkModule2();
  v5 = 5;
  castBarEnemyStringArray = GetStringArrayData(raptureAtkModule, 5i64);
  castBarEnemyStringArray = (Component::GUI::StringArrayData *)castBarEnemyStringArray;
  if ( castBarEnemyNumberArray && castBarEnemyStringArray && (*(char *)(castBarEnemyNumberArray + 30) > 0 || *(char *)(castBarEnemyStringArray + 30) > 0) )
  {
    LOBYTE(castBarEnemyStringArray) = Client::Game::Character::Character_IsInPvP(g_Client::Game::Control::Control_Instance.LocalPlayer);
    if ( !(_BYTE)castBarEnemyStringArray )
    {
      castBarEnemySetting = GetSettingAtIndex(&g_Client::System::Framework::Framework_InstancePointer2->SystemConfig, 604i64);
      if ( *(_DWORD *)(castBarEnemySetting + 24) )
        castBarEnemySettingValue = *(_DWORD *)(castBarEnemySetting + 32);
      else
        castBarEnemySettingValue = 0;
      Component::GUI::NumberArrayData_SetValueIfDifferent(castBarEnemyNumberArray, 0i64, castBarEnemySettingValue != 0);
      currCastBarEnemyCount = 0;
      NamePlateObjectInfoCount = *(int *)(ui3dModule + 85616);
      if ( NamePlateObjectInfoCount > 0 )
      {
        namePlateObjectInfoOffset = 85216i64;
        objCtr = 0i64;
        temp13 = 0i64;
        temp43 = 0i64;
        while ( 1 )
        {
          if ( currCastBarEnemyCount >= 10 )
          {
LABEL_59:
            if ( currCastBarEnemyCount > 10 )
              currCastBarEnemyCount = 10;
            goto LABEL_61;
          }
          currNamePlateObjectInfo = *(_QWORD *)(namePlateObjectInfoOffset + ui3dModule);
          if ( *(_BYTE *)(currNamePlateObjectInfo + 79) < 0x32u )
            break;
LABEL_58:
          ++objCtr;
          namePlateObjectInfoOffset += 8;
          if ( objCtr >= NamePlateObjectInfoCount )
            goto LABEL_59;
        }
        AsCharacter = Client::Game::Object::GameObject_GetAsCharacter(*(_QWORD *)(currNamePlateObjectInfo + 24));
        character = AsCharacter;
        if ( ((unsigned __int8)namePlateObjectInfo.NamePlateObjectKind - 3) > 1  // Not a PlayerCharacter, EventNpcCompanion, Retainer, BattleNpcEnemy, BattleNpcFriendly
          || !AsCharacter
          || !GetIsTargetable(AsCharacter)
          || (StatusManager = GetStatusManager(character),
              (unsigned __int8)Client::Game::StatusManager_HasFlag(StatusManager, 42i64))
          || ((*(_BYTE *)(character + 460) - 2) & 0xFB) != 0 )  // BattleChara.Icon != 2
        {
LABEL_57:
          v1RaptureAtkModule = raptureAtkModule;
          goto LABEL_58;
        }
        Component::GUI::NumberArrayData_SetValueIfDifferent(castBarEnemyNumberArray, v5 - 3, *(unsigned int *)(character + 120));
        IsCastInProgress = Client::Game::Character::Character_IsCastInProgress(character);
        character = *(_QWORD *)character;
        if ( IsCastInProgress )
        {
          CastInfo = GetCastInfo(character);
          if ( CastInfo )
            SpellIdForAction = Client::Game::ActionManager_GetSpellIdForAction(
                                 (Client::Game::ActionType)*(unsigned __int8 *)(CastInfo + 2),
                                 *(_DWORD *)(CastInfo + 4));
          else
            SpellIdForAction = 0;
          ActionRow_1 = Component::Exd::ExdModule_GetActionRow_1(SpellIdForAction);
          if ( ActionRow_1 && (ActionRow_1->PackedBool3E & 2) == 0 )  // Unknown22 in Action sheet
          {
            UIModule = Client::System::Framework::Framework_GetUIModule(g_Client::System::Framework::Framework_InstancePointer2);
            if ( UIModule )
            {
              raptureAtkModule24 = UIModule->GetRaptureTextModule(UIModule);
              sub_14097E300(1i64, SpellIdForAction);
              temp25 = (char *)Client::UI::Misc::RaptureTextModule_FormatAddonText1<int>(raptureAtkModule24, 0x7EAu);
            }
            else
            {
              temp25 = "NO DATA";
            }
            Component::GUI::StringArrayData_SetValueAndUpdate(castBarEnemyStringArray, currCastBarEnemyCount, temp25, 0, 1u);
            temp13 = temp43;
            if ( *(char **)((char *)castBarEnemyStringArray->StringArray + temp43) )
            {
              temp26 = GetCastInfo(character);
              if ( temp26 )
                temp27 = *(float *)(temp26 + 60);
              else
                temp27 = 0.0;
              temp28 = GetCastInfo(character);
              if ( temp28 )
                temp29 = *(float *)(temp28 + 52);
              else
                temp29 = 0.0;
              if ( temp27 == 0.0 )
              {
                temp30 = 0i64;
              }
              else if ( temp29 < temp27 )
              {
                temp31 = (float)(temp29 * 100.0) / temp27;
                if ( temp31 >= 0.0 )
                  temp32 = temp31 + 0.5;
                else
                  temp32 = temp31 - 0.5;
                temp30 = (unsigned int)(int)temp32;
              }
              else
              {
                temp30 = 100i64;
              }
            }
            else
            {
              temp30 = 0xFFFFFFFFi64;
            }
            Component::GUI::NumberArrayData_SetValueIfDifferent(castBarEnemyNumberArray, v5 - 2, temp30);
            temp33 = GetCastInfo(character);
            if ( temp33 )
              LOBYTE(temp33) = *(_BYTE *)(temp33 + 1);
            Component::GUI::NumberArrayData_SetValueIfDifferent(castBarEnemyNumberArray, v5 - 1, (unsigned __int8)temp33);
            Component::GUI::NumberArrayData_SetValueIfDifferent(castBarEnemyNumberArray, v5, 0i64);
            goto LABEL_56;
          }
          temp13 = temp43;
        }
        else
        {
          temp34 = GetCastInfo(character);
          if ( temp34 )
          {
            temp35 = *(_DWORD *)(temp34 + 348);
            if ( temp35 )
            {
              temp36 = Component::Exd::ExdModule_GetActionRow_1(temp35);
              if ( temp36 )
              {
                if ( (temp36->PackedBool3E & 2) == 0 )  // Unknown22 in Action sheet
                {
                  temp37 = Client::System::Framework::Framework_GetUIModule(g_Client::System::Framework::Framework_InstancePointer2);
                  if ( temp37 )
                  {
                    temp38 = temp37->GetRaptureTextModule(temp37);
                    sub_14097E300(1i64, temp35);
                    temp39 = (char *)Client::UI::Misc::RaptureTextModule_FormatAddonText1<int>(temp38, 0x7EAu);
                  }
                  else
                  {
                    temp39 = "NO DATA";
                  }
                  Component::GUI::StringArrayData_SetValueAndUpdate(castBarEnemyStringArray, currCastBarEnemyCount, temp39, 0, 1u);
                  Component::GUI::NumberArrayData_SetValueIfDifferent(castBarEnemyNumberArray, v5 - 1, 1i64);
                  Component::GUI::NumberArrayData_SetValueIfDifferent(castBarEnemyNumberArray, v5, 1i64);
                }
              }
            }
          }
        }
        Component::GUI::NumberArrayData_SetValueIfDifferent(castBarEnemyNumberArray, v5 - 2, 0xFFFFFFFFi64);
        Component::GUI::StringArrayData_SetValueAndUpdate(castBarEnemyStringArray, currCastBarEnemyCount, 0i64, 0, 1u);
LABEL_56:
        temp13 += 8i64;
        v5 += 4;
        temp43 = temp13;
        ++currCastBarEnemyCount;
        goto LABEL_57;
      }
LABEL_61:
      LOBYTE(castBarEnemyStringArray) = Component::GUI::NumberArrayData_SetValueIfDifferent(castBarEnemyNumberArray, 1i64, (unsigned int)currCastBarEnemyCount);
    }
  }
  return castBarEnemyStringArray;
}