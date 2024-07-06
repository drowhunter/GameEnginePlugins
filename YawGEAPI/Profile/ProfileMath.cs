// Decompiled with JetBrains decompiler
// Type: YawGEAPI.ProfileMath
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace YawGEAPI
{
  public class ProfileMath : INotifyPropertyChanged
  {
    private MathType mathType;
    private int otherInputIndex;

    public MathType MathType
    {
      get => this.mathType;
      set
      {
        this.mathType = value;
        this.OnPropertyChanged(nameof (MathType));
      }
    }

    public int OtherInput
    {
      get => this.otherInputIndex;
      set
      {
        this.otherInputIndex = value;
        this.OnPropertyChanged(nameof (OtherInput));
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
  }
}
