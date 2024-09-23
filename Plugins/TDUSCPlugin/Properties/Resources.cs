using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;


namespace TDUSCPlugin.Properties
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
        if (TDUSCPlugin.Properties.Resources.resourceMan == null)
          TDUSCPlugin.Properties.Resources.resourceMan = new ResourceManager("TDUSCPlugin.Properties.Resources", typeof (TDUSCPlugin.Properties.Resources).Assembly);
        return TDUSCPlugin.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => TDUSCPlugin.Properties.Resources.resourceCulture;
      set => TDUSCPlugin.Properties.Resources.resourceCulture = value;
    }

    public static Bitmap background
    {
      get
      {
        return (Bitmap) TDUSCPlugin.Properties.Resources.ResourceManager.GetObject(nameof (background), TDUSCPlugin.Properties.Resources.resourceCulture);
      }
    }

    public static string defProfile
    {
      get => TDUSCPlugin.Properties.Resources.ResourceManager.GetString(nameof (defProfile), TDUSCPlugin.Properties.Resources.resourceCulture);
    }

    public static string description
    {
      get => TDUSCPlugin.Properties.Resources.ResourceManager.GetString(nameof (description), TDUSCPlugin.Properties.Resources.resourceCulture);
    }

    public static Bitmap logo
    {
      get => (Bitmap) TDUSCPlugin.Properties.Resources.ResourceManager.GetObject(nameof (logo), TDUSCPlugin.Properties.Resources.resourceCulture);
    }

    public static Bitmap small
    {
      get
      {
        return (Bitmap) TDUSCPlugin.Properties.Resources.ResourceManager.GetObject(nameof (small), TDUSCPlugin.Properties.Resources.resourceCulture);
      }
    }
  }
}
