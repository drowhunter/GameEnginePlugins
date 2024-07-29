using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;


namespace GT7Plugin.Properties
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
        if (GT7Plugin.Properties.Resources.resourceMan == null)
          GT7Plugin.Properties.Resources.resourceMan = new ResourceManager("GT7Plugin.Properties.Resources", typeof (GT7Plugin.Properties.Resources).Assembly);
        return GT7Plugin.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => GT7Plugin.Properties.Resources.resourceCulture;
      set => GT7Plugin.Properties.Resources.resourceCulture = value;
    }

    public static Bitmap background
    {
      get
      {
        return (Bitmap) GT7Plugin.Properties.Resources.ResourceManager.GetObject(nameof (background), GT7Plugin.Properties.Resources.resourceCulture);
      }
    }

    public static string defProfile
    {
      get => GT7Plugin.Properties.Resources.ResourceManager.GetString(nameof (defProfile), GT7Plugin.Properties.Resources.resourceCulture);
    }

    public static string description
    {
      get => GT7Plugin.Properties.Resources.ResourceManager.GetString(nameof (description), GT7Plugin.Properties.Resources.resourceCulture);
    }

    public static Bitmap logo
    {
      get => (Bitmap) GT7Plugin.Properties.Resources.ResourceManager.GetObject(nameof (logo), GT7Plugin.Properties.Resources.resourceCulture);
    }

    public static Bitmap small
    {
      get
      {
        return (Bitmap) GT7Plugin.Properties.Resources.ResourceManager.GetObject(nameof (small), GT7Plugin.Properties.Resources.resourceCulture);
      }
    }
  }
}
