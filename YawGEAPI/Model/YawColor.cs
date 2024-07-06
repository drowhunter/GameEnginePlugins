// Decompiled with JetBrains decompiler
// Type: YawGEAPI.YawColor
// Assembly: YawGEAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FB729A17-7B5B-4DB2-AD4D-7D0F4F421877
// Assembly location: C:\src\mine\GameEnginePlugins\dlls\YawGEAPI.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;


namespace YawGEAPI
{
  public class YawColor : INotifyPropertyChanged
  {
    public static YawColor BLACK = new YawColor((byte) 0, (byte) 0, (byte) 0);
    public static YawColor WHITE = new YawColor((byte) 0, (byte) 0, (byte) 0);
    public static YawColor GRAY = new YawColor((byte) 80, (byte) 80, (byte) 80);
    private byte r;
    private byte g;
    private byte b;
    private static Random randomGenerator = new Random();

    public byte R
    {
      get => this.r;
      set
      {
        this.r = value;
        this.OnPropertyChanged(string.Empty);
      }
    }

    public byte G
    {
      get => this.g;
      set
      {
        this.g = value;
        this.OnPropertyChanged(string.Empty);
      }
    }

    public byte B
    {
      get => this.b;
      set
      {
        this.b = value;
        this.OnPropertyChanged(string.Empty);
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(name));
    }

    public YawColor(byte r, byte g, byte b)
    {
      this.r = r;
      this.g = g;
      this.b = b;
    }

    public override bool Equals(object obj)
    {
      return obj is YawColor yawColor && (int) this.r == (int) yawColor.r && (int) this.g == (int) yawColor.g && (int) this.b == (int) yawColor.b;
    }

    public static YawColor Lerp(YawColor a, YawColor b, float t)
    {
      YawColor yawColor = new YawColor((byte) 0, (byte) 0, (byte) 0);
      t = Math.Abs(t);
      yawColor.R = (byte) ((double) a.R + (double) ((int) b.R - (int) a.R) * (double) t);
      yawColor.G = (byte) ((double) a.G + (double) ((int) b.G - (int) a.G) * (double) t);
      yawColor.B = (byte) ((double) a.B + (double) ((int) b.B - (int) a.B) * (double) t);
      return yawColor;
    }

    public static YawColor RandomColor()
    {
      byte[] numArray = new byte[3];
      YawColor.randomGenerator.NextBytes(numArray);
      float num = (float) ((int) byte.MaxValue / (int) ((IEnumerable<byte>) numArray).Max<byte>());
      numArray[0] = (byte) ((double) numArray[0] * (double) num);
      numArray[1] = (byte) ((double) numArray[1] * (double) num);
      numArray[2] = (byte) ((double) numArray[2] * (double) num);
      return new YawColor(numArray[0], numArray[1], numArray[2]);
    }

    public static YawColor FromDrawColor(Color dColor)
    {
      return new YawColor(dColor.R, dColor.G, dColor.B);
    }

    public static Color ToDrawColor(YawColor c) => Color.FromArgb((int) c.r, (int) c.g, (int) c.b);

    public override int GetHashCode()
    {

            // calculate hashcode based on rgb properties
            unchecked
            {
                return ((-839137856 * -1521134295 + this.r.GetHashCode()) * -1521134295 + this.g.GetHashCode()) * -1521134295 + this.b.GetHashCode();
            }
      //return ((-839137856 * -1521134295 + this.r.GetHashCode()) * -1521134295 + this.g.GetHashCode()) * -1521134295 + this.b.GetHashCode();
    }
  }
}
