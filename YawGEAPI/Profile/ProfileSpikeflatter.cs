// Decompiled with JetBrains decompiler
// Type: YawGEAPI.ProfileSpikeflatter
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace YawGEAPI
{
  public class ProfileSpikeflatter : INotifyPropertyChanged
  {
    private bool enabled = false;
    private float limit;
    private float strength;

    public bool Enabled
    {
      get => this.enabled;
      set
      {
        this.enabled = value;
        this.OnPropertyChanged(nameof (Enabled));
      }
    }

    public float Limit
    {
      get => this.limit;
      set
      {
        this.limit = value;
        this.OnPropertyChanged(nameof (Limit));
      }
    }

    public float Strength
    {
      get => this.strength;
      set
      {
        this.strength = value;
        this.OnPropertyChanged(nameof (Strength));
      }
    }

    public ProfileSpikeflatter()
    {
      this.enabled = false;
      this.strength = 0.5f;
      this.limit = 100f;
    }

    public ProfileSpikeflatter(bool enabled, float limit, float strength)
    {
      this.limit = limit;
      this.strength = strength;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(name));
    }
  }
}
