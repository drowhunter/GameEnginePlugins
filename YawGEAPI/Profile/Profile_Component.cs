// Decompiled with JetBrains decompiler
// Type: YawGEAPI.Profile_Component
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace YawGEAPI
{
  public class Profile_Component : INotifyPropertyChanged, ICloneable
  {
    private float multiplierPos;
    private float multiplierNeg;
    private int output_index;
    private int input_index;
    private bool constant = false;
    private bool inverse = false;
    private float limit;
    private float smoothing;
    private float offset;

    public bool Constant
    {
      get => this.constant;
      set
      {
        this.constant = value;
        this.OnPropertyChanged(nameof (Constant));
      }
    }

    public int Input_index
    {
      get => this.input_index;
      set
      {
        this.input_index = value;
        this.OnPropertyChanged(nameof (Input_index));
      }
    }

    public int Output_index
    {
      get => this.output_index;
      set
      {
        this.output_index = value;
        this.OnPropertyChanged(nameof (Output_index));
      }
    }

    public float MultiplierPos
    {
      get => this.multiplierPos;
      set
      {
        this.multiplierPos = value;
        this.OnPropertyChanged(nameof (MultiplierPos));
      }
    }

    public float MultiplierNeg
    {
      get => this.multiplierNeg;
      set
      {
        this.multiplierNeg = value;
        this.OnPropertyChanged(nameof (MultiplierNeg));
      }
    }

    public float Offset
    {
      get => this.offset;
      set
      {
        this.offset = value;
        this.OnPropertyChanged(nameof (Offset));
      }
    }

    public bool Inverse
    {
      get => this.inverse;
      set
      {
        this.inverse = value;
        this.OnPropertyChanged(nameof (Inverse));
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

    public float Smoothing
    {
      get => this.smoothing;
      set
      {
        this.smoothing = value;
        this.OnPropertyChanged(nameof (Smoothing));
      }
    }

    [DefaultValue(true)]
    [JsonProperty]
    public bool Enabled { get; set; } = true;

    [JsonProperty]
    public ProfileSpikeflatter Spikeflatter { get; set; } = new ProfileSpikeflatter();

    [JsonProperty]
    public float Deadzone { get; set; } = 0.0f;

    [JsonProperty]
    public ProfileComponentType Type { get; set; } = ProfileComponentType.VALUE;

    [JsonProperty]
    public ObservableCollection<ProfileCondition> Condition { get; set; } = new ObservableCollection<ProfileCondition>();

    [JsonProperty]
    public ObservableCollection<ProfileMath> Math { get; set; } = new ObservableCollection<ProfileMath>();

    public Profile_Component(
      int input_index,
      int output_index,
      float multiplierPos,
      float multiplierNeg,
      float offset,
      bool constant,
      bool inverse,
      float limit,
      float smoothing,
      bool enabled = true,
      ObservableCollection<ProfileMath> math = null,
      ProfileSpikeflatter spikeflatter = null,
      float deadzone = 0.0f,
      ProfileComponentType type = ProfileComponentType.VALUE,
      ObservableCollection<ProfileCondition> condition = null)
    {
      this.Constant = constant;
      this.Input_index = input_index;
      this.Output_index = output_index;
      this.MultiplierPos = multiplierPos;
      this.MultiplierNeg = multiplierNeg;
      this.Offset = offset;
      this.Inverse = inverse;
      this.Limit = limit;
      this.Smoothing = smoothing;
      this.Enabled = enabled;
      this.Spikeflatter = spikeflatter == null ? new ProfileSpikeflatter() : spikeflatter;
      this.Deadzone = deadzone;
      this.Type = type;
      if (condition != null)
        this.Condition = condition;
      if (math == null)
        return;
      this.Math = math;
    }

    public Profile_Component()
    {
      this.Spikeflatter = new ProfileSpikeflatter();
      this.Math = new ObservableCollection<ProfileMath>();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(name));
    }

    public object Clone()
    {
      return (object) JsonConvert.DeserializeObject<Profile_Component>(JsonConvert.SerializeObject((object) this));
    }

    public override bool Equals(object obj)
    {
      if (!(obj is Profile_Component))
        return false;
      Profile_Component profileComponent = obj as Profile_Component;
      return JsonConvert.SerializeObject((object) this).Equals(JsonConvert.SerializeObject((object) profileComponent));
    }
  }
}
