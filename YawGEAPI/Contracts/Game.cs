// Decompiled with JetBrains decompiler
// Type: YawGEAPI.Game
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using System.Collections.Generic;
using System.Drawing;
using System.Reflection;


namespace YawGEAPI
{
  public interface Game
  {
    int STEAM_ID { get; }

    string PROCESS_NAME { get; }

    bool PATCH_AVAILABLE { get; }

    string AUTHOR { get; }

    Image Logo { get; }

    Image SmallLogo { get; }

    Image Background { get; }

    string Description { get; }

    void Exit();

    void Init();

    void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher);

    string[] GetInputData();

    List<Profile_Component> DefaultProfile();

    LedEffect DefaultLED();

    Dictionary<string, ParameterInfo[]> GetFeatures();

    void PatchGame();
  }
}
