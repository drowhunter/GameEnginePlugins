// Decompiled with JetBrains decompiler
// Type: YawGEAPI.ProfileCreator
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace YawGEAPI
{
  public static class ProfileCreator
  {
    public static Profile CreateProfileFromJSON(string json)
    {
      try
      {
        return JsonConvert.DeserializeObject<Profile>(json);
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        return new Profile("Default", new List<Profile_Component>(), new LedEffect());
      }
    }
  }
}
