using GT7Plugin.Properties;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using YawGEAPI;

namespace YawVR_Game_Engine.Plugin.Properties
{
    [Export(typeof(Game))]
    [ExportMetadata("Name", "Gran Turismo 7")]
    [ExportMetadata("Version", "1.0")]
    public class GT7Plugin : Game
    {
        private volatile bool running = false;
        private IMainFormDispatcher dispatcher;
        private IProfileManager controller;

        public int STEAM_ID => 0;

        public string PROCESS_NAME => string.Empty;

        public bool PATCH_AVAILABLE => false;

        public string AUTHOR => "Drowhunter";

        public string Description => Resources.description;

        public Image Logo => Resources.logo;

        public Image SmallLogo => Resources.small;

        public Image Background => Resources.background;

        public LedEffect DefaultLED()
        {
            return new LedEffect(EFFECT_TYPE.KNIGHT_RIDER_2, 7, new YawColor[4]
            {
                new YawColor((byte) 66, (byte) 135, (byte) 245),
                new YawColor((byte) 80, (byte) 80, (byte) 80),
                new YawColor((byte) 128, (byte) 3, (byte) 117),
                new YawColor((byte) 110, (byte) 201, (byte) 12)
            }, 25f);
        }

        public List<Profile_Component> DefaultProfile()
        {
            return new List<Profile_Component>()
            {
                new Profile_Component(4, 1, 1f, 0.0f, 0.0f, false, false, -1f, 1f),
                new Profile_Component(7, 1, 0.5f, 0.0f, 0.0f, false, true, -1f, 0.3f),
                new Profile_Component(1, 2, 0.1f, 0.0f, 0.0f, false, false, -1f, 0.05f),
                new Profile_Component(5, 2, 1f, 0.0f, 0.0f, false, true, -1f, 1f),
                new Profile_Component(0, 1, 0.1f, 0.0f, 0.0f, false, false, -1f, 1f),
                new Profile_Component(3, 0, 1f, 0.0f, 0.0f, false, true, -1f, 1f)
            };
        }

        public void Exit()
        {
            this.running = false;
        }

        public Dictionary<string, ParameterInfo[]> GetFeatures()
        {
            throw new NotImplementedException();
        }

        public string[] GetInputData()
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            this.running = true;
        }

        public void PatchGame()
        {
            throw new NotImplementedException();
        }

        public void SetReferences(IProfileManager controller, IMainFormDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.controller = controller;
        }
    }
}
