// Decompiled with JetBrains decompiler
// Type: ACPlugin.Properties.Resources
// Assembly: ACPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4B94A2DB-8D35-4A0B-BF8B-FF6A3CB7A173
// Assembly location: E:\_Apps\GameEngine_2_6\Gameplugins\ACPlugin.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;


namespace ACPlugin.Properties
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
        if (ACPlugin.Properties.Resources.resourceMan == null)
          ACPlugin.Properties.Resources.resourceMan = new ResourceManager("ACPlugin.Properties.Resources", typeof (ACPlugin.Properties.Resources).Assembly);
        return ACPlugin.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ACPlugin.Properties.Resources.resourceCulture;
      set => ACPlugin.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap background
    {
      get
      {
        return (Bitmap) ACPlugin.Properties.Resources.ResourceManager.GetObject(nameof (background), ACPlugin.Properties.Resources.resourceCulture);
      }
    }

    internal static string defProfile
    {
      get => ACPlugin.Properties.Resources.ResourceManager.GetString(nameof (defProfile), ACPlugin.Properties.Resources.resourceCulture);
    }

    internal static Bitmap logo
    {
      get => (Bitmap) ACPlugin.Properties.Resources.ResourceManager.GetObject(nameof (logo), ACPlugin.Properties.Resources.resourceCulture);
    }

    internal static Bitmap small
    {
      get
      {
        return (Bitmap) ACPlugin.Properties.Resources.ResourceManager.GetObject(nameof (small), ACPlugin.Properties.Resources.resourceCulture);
      }
    }
  }
}
