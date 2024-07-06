// Decompiled with JetBrains decompiler
// Type: TheCrew2.Properties.Resources
// Assembly: TheCrew2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 57EA1049-7C99-4299-9B7E-8E15049A09F6
// Assembly location: C:\apps\yawge\Gameplugins\TheCrew2Plugin.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;


namespace TheCrew2.Properties
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
        if (TheCrew2.Properties.Resources.resourceMan == null)
          TheCrew2.Properties.Resources.resourceMan = new ResourceManager("TheCrew2.Properties.Resources", typeof (TheCrew2.Properties.Resources).Assembly);
        return TheCrew2.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => TheCrew2.Properties.Resources.resourceCulture;
      set => TheCrew2.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap background
    {
      get
      {
        return (Bitmap) TheCrew2.Properties.Resources.ResourceManager.GetObject(nameof (background), TheCrew2.Properties.Resources.resourceCulture);
      }
    }

    internal static string description
    {
      get => TheCrew2.Properties.Resources.ResourceManager.GetString(nameof (description), TheCrew2.Properties.Resources.resourceCulture);
    }

    internal static Bitmap logo
    {
      get => (Bitmap) TheCrew2.Properties.Resources.ResourceManager.GetObject(nameof (logo), TheCrew2.Properties.Resources.resourceCulture);
    }

    internal static Bitmap small
    {
      get
      {
        return (Bitmap) TheCrew2.Properties.Resources.ResourceManager.GetObject(nameof (small), TheCrew2.Properties.Resources.resourceCulture);
      }
    }
  }
}
