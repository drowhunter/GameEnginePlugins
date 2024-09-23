using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YawGEAPI;


namespace PluginTester
{
    public class pm : IProfileManager
    {
        public void ResetYawOffset()
        {
            //return string.Empty;
        }

        public void SetInput(int index, float value)
        {
            //return string.Empty;
        }
    }

    public class dp : IMainFormDispatcher
    {
        public void DialogShow(string _string, DIALOG_TYPE type, Action<bool> _yes = null, Action<bool> _no = null, bool showChk = false, bool chkDefault = false, string chkText = "")
        {
            
        }

        public void ExitGame()
        {
            
        }

        public void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, bool overwrite)
        {
            
        }

        public string GetInstallPath(string name)
        {
            return string.Empty;
        }

        public string GetLanguage()
        {
            return string.Empty;
        }

        public T GetSavedObjectAndUpdate<T>(string plugin, string URL = null)
        {
            return default(T);
        }

        public List<Profile_Component> JsonToComponents(string json)
        {
            return new List<Profile_Component>();
        }

        public LedEffect JsonToLED(string json)
        {
            return new LedEffect();
        }

        public void OpenPluginManager()
        {
            
        }

        public void RestartApp(bool admin)
        {
            
        }

        public void ShowNotification(NotificationType type, string text)
        {
            
        }
    }
    internal class Program
    {
        
        static async Task Main(string[] args)
        {
            var plugin = new YawVR_Game_Engine.Plugin.TDUSCPlugin(new pm(), new dp());
            

            try
            {
                plugin.Init();
                    
                Console.WriteLine("Plugin initialized.. press a key to quit");
                
                Console.ReadKey();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }


        }
    }
}
