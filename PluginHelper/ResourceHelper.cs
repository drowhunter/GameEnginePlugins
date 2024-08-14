using System;
using System.IO;
using System.Reflection;

namespace PluginHelper
{
    public class ResourceHelper
    {
        public static string LoadEmbeddedResourceString(string resourceName)
        {
            
            var result = string.Empty;

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var name = assembly.GetName().Name;
                using (Stream stream = assembly.GetManifestResourceStream($"{name}.Resources.{resourceName}"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }
    }
}
