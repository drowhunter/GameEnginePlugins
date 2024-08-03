using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;


namespace OverloadPlugin.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class Resources
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
        if (OverloadPlugin.Properties.Resources.resourceMan == null)
          OverloadPlugin.Properties.Resources.resourceMan = new ResourceManager("OverloadPlugin.Properties.Resources", typeof (OverloadPlugin.Properties.Resources).Assembly);
        return OverloadPlugin.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => OverloadPlugin.Properties.Resources.resourceCulture;
      set => OverloadPlugin.Properties.Resources.resourceCulture = value;
    }

    public static Bitmap background
    {
      get
      {
        return (Bitmap) OverloadPlugin.Properties.Resources.ResourceManager.GetObject(nameof (background), OverloadPlugin.Properties.Resources.resourceCulture);
      }
    }

    public static string defProfile
    {
      get => OverloadPlugin.Properties.Resources.ResourceManager.GetString(nameof (defProfile), OverloadPlugin.Properties.Resources.resourceCulture);
    }

    public static string description
    {
      get => OverloadPlugin.Properties.Resources.ResourceManager.GetString(nameof (description), OverloadPlugin.Properties.Resources.resourceCulture);
    }

    public static Bitmap logo
    {
      get => (Bitmap) OverloadPlugin.Properties.Resources.ResourceManager.GetObject(nameof (logo), OverloadPlugin.Properties.Resources.resourceCulture);
    }

    public static Bitmap small
    {
      get
      {
        return (Bitmap) OverloadPlugin.Properties.Resources.ResourceManager.GetObject(nameof (small), OverloadPlugin.Properties.Resources.resourceCulture);
      }
    }
  }
}
