// Decompiled with JetBrains decompiler
// Type: YawGEAPI.IMainFormDispatcher
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using System;


namespace YawGEAPI
{
  public interface IMainFormDispatcher
  {
    string GetLanguage();

    void ShowNotification(NotificationType type, string text);

    void DialogShow(
      string _string,
      DIALOG_TYPE type,
      Action<bool> _yes = null,
      Action<bool> _no = null,
      bool showChk = false,
      bool chkDefault = false,
      string chkText = "");

    string GetInstallPath(string name);

    void OpenPluginManager();

    void ExitGame();

    T GetSavedObjectAndUpdate<T>(string plugin, string URL = null);

    void RestartApp(bool admin);

    void ExtractToDirectory(
      string sourceArchiveFileName,
      string destinationDirectoryName,
      bool overwrite);

    Profile JsonToProfile(string json);
  }
}
