// Decompiled with JetBrains decompiler
// Type: TheCrew.Properties.Resources
// Assembly: TheCrew, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4722EE52-F477-46B0-8A58-A26AC0A5A193
// Assembly location: G:\apps\GameEngine\Gameplugins\TheCrew2Plugin.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace TheCrew.Properties
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
        if (TheCrew.Properties.Resources.resourceMan == null)
          TheCrew.Properties.Resources.resourceMan = new ResourceManager("TheCrew.Properties.Resources", typeof (TheCrew.Properties.Resources).Assembly);
        return TheCrew.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => TheCrew.Properties.Resources.resourceCulture;
      set => TheCrew.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap background => (Bitmap) TheCrew.Properties.Resources.ResourceManager.GetObject(nameof (background), TheCrew.Properties.Resources.resourceCulture);

    internal static string description => TheCrew.Properties.Resources.ResourceManager.GetString(nameof (description), TheCrew.Properties.Resources.resourceCulture);

    internal static Bitmap logo => (Bitmap) TheCrew.Properties.Resources.ResourceManager.GetObject(nameof (logo), TheCrew.Properties.Resources.resourceCulture);

    internal static Bitmap small => (Bitmap) TheCrew.Properties.Resources.ResourceManager.GetObject(nameof (small), TheCrew.Properties.Resources.resourceCulture);
  }
}
