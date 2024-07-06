// Decompiled with JetBrains decompiler
// Type: YawGEAPI.IButtonFeatureMap
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll


namespace YawGEAPI
{
  public interface IButtonFeatureMap
  {
    ARCADE_BUTTON_TYPE ButtonId { get; set; }

    ButtonFeature Feature { get; set; }
  }
}
