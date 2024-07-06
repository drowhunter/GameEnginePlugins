// Decompiled with JetBrains decompiler
// Type: YawGEAPI.LedEffect
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace YawGEAPI
{
  public class LedEffect : INotifyPropertyChanged
  {
    private EFFECT_TYPE effectID;
    private int inputID;
    private float multiplier;
    private ObservableCollection<YawColor> colors = new ObservableCollection<YawColor>();

    public EFFECT_TYPE EffectID
    {
      get => this.effectID;
      set
      {
        this.effectID = value;
        this.OnPropertyChanged(nameof (EffectID));
      }
    }

    public int InputID
    {
      get => this.inputID;
      set
      {
        this.inputID = value;
        this.OnPropertyChanged(nameof (InputID));
      }
    }

    public float Multiplier
    {
      get => this.multiplier;
      set
      {
        this.multiplier = value;
        this.OnPropertyChanged(nameof (Multiplier));
      }
    }

    public ObservableCollection<YawColor> Colors
    {
      get => this.colors;
      set
      {
        this.colors = value;
        this.OnPropertyChanged(nameof (Colors));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(name));
    }

    public LedEffect(EFFECT_TYPE effectID, int inputID, YawColor[] colors, float multiplier)
    {
      this.EffectID = effectID;
      this.InputID = inputID;
      this.Colors = new ObservableCollection<YawColor>((IEnumerable<YawColor>) colors);
      this.Multiplier = multiplier;
    }

    public LedEffect()
    {
    }
  }
}
