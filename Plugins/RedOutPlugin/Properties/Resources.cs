// Decompiled with JetBrains decompiler
// Type: RedOutPlugin.Properties.Resources
// Assembly: RedOutPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C05644C-29E2-4C25-BA06-FC759BF669E2
// Assembly location: C:\src\yawge\Gameplugins\RedOutPlugin.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;


namespace RedOutPlugin.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (RedOutPlugin.Properties.Resources.resourceMan == null)
          RedOutPlugin.Properties.Resources.resourceMan = new ResourceManager("RedOutPlugin.Properties.Resources", typeof (RedOutPlugin.Properties.Resources).Assembly);
        return RedOutPlugin.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => RedOutPlugin.Properties.Resources.resourceCulture;
      set => RedOutPlugin.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap background
    {
      get
      {
        return (Bitmap) RedOutPlugin.Properties.Resources.ResourceManager.GetObject(nameof (background), RedOutPlugin.Properties.Resources.resourceCulture);
      }
    }

    internal static string defProfile
    {
      get => RedOutPlugin.Properties.Resources.ResourceManager.GetString(nameof (defProfile), RedOutPlugin.Properties.Resources.resourceCulture);
    }

    internal static string description
    {
      get => RedOutPlugin.Properties.Resources.ResourceManager.GetString(nameof (description), RedOutPlugin.Properties.Resources.resourceCulture);
    }

    internal static Bitmap logo
    {
      get => (Bitmap) RedOutPlugin.Properties.Resources.ResourceManager.GetObject(nameof (logo), RedOutPlugin.Properties.Resources.resourceCulture);
    }

    internal static Bitmap small
    {
      get
      {
        return (Bitmap) RedOutPlugin.Properties.Resources.ResourceManager.GetObject(nameof (small), RedOutPlugin.Properties.Resources.resourceCulture);
      }
    }
  }
}
