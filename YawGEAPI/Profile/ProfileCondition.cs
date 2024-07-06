// Decompiled with JetBrains decompiler
// Type: YawGEAPI.ProfileCondition
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace YawGEAPI
{
  public class ProfileCondition : INotifyPropertyChanged
  {
    private ConditionOperator conditionOperator = ConditionOperator.EQUALS;
    private int otherIndex = 0;
    private float _value;

    public ConditionOperator ComponentCondition
    {
      get => this.conditionOperator;
      set
      {
        this.conditionOperator = value;
        this.OnPropertyChanged(nameof (ComponentCondition));
      }
    }

    public int OtherIndex
    {
      get => this.otherIndex;
      set
      {
        this.otherIndex = value;
        this.OnPropertyChanged(nameof (OtherIndex));
      }
    }

    public float Value
    {
      get => this._value;
      set
      {
        this._value = value;
        this.OnPropertyChanged(nameof (Value));
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
