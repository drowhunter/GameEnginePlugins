// Decompiled with JetBrains decompiler
// Type: GT7.Properties.Resources
// Assembly: GT7, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4722EE52-F477-46B0-8A58-A26AC0A5A193
// Assembly location: G:\apps\GameEngine\Gameplugins\GT72Plugin.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace GT7.Properties
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
        if (GT7.Properties.Resources.resourceMan == null)
          GT7.Properties.Resources.resourceMan = new ResourceManager("GT7.Properties.Resources", typeof (GT7.Properties.Resources).Assembly);
        return GT7.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => GT7.Properties.Resources.resourceCulture;
      set => GT7.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap background => (Bitmap) GT7.Properties.Resources.ResourceManager.GetObject(nameof (background), GT7.Properties.Resources.resourceCulture);

    internal static string description => GT7.Properties.Resources.ResourceManager.GetString(nameof (description), GT7.Properties.Resources.resourceCulture);

    internal static Bitmap logo => (Bitmap) GT7.Properties.Resources.ResourceManager.GetObject(nameof (logo), GT7.Properties.Resources.resourceCulture);

    internal static Bitmap small => (Bitmap) GT7.Properties.Resources.ResourceManager.GetObject(nameof (small), GT7.Properties.Resources.resourceCulture);
  }
}
