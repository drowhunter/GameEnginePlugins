// Decompiled with JetBrains decompiler
// Type: YawGEAPI.Profile
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using System.Collections.Generic;


namespace YawGEAPI
{
  public class Profile
  {
    public string GameName;
    public string Name;
    public List<Profile_Component> components;
    public LedEffect effects;
    public List<IButtonFeatureMap> functions = new List<IButtonFeatureMap>();

    public Profile(string name, List<Profile_Component> profile, LedEffect ledEffect)
    {
      this.Name = name;
      this.components = profile;
      this.effects = ledEffect;
    }
  }
}
