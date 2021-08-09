using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace _24HWatch
{
    public class _24HWatch : Mod
    {
        public override string ID => "24HWatch"; //Your mod ID (unique)
        public override string Name => "24 Hour Watch"; //You mod name
        public override string Author => "OneSidedDie"; //Your Username
        public override string Version => "1.0"; //Version

        private readonly int width = 200;

        private readonly int height = 20;

        private string textToShow;
        private readonly Keybind debugToggleKey = new Keybind("debugToggleKey", "Toggle Debug", KeyCode.RightBracket);
        private GUIStyle infoText;
        private float sunRotation;
        private float rotateY;
        private GameObject watchHour24;
        private Material hourHandMaterial;
        private Mesh filter;
        private bool debugging = false;
        private float IsAfternoon()
        {
            sunRotation = GameObject.Find("SUN/Pivot").GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Rotation").Value;
            if (sunRotation >= 180f && sunRotation <= 240f)
            {
                return -120f;
            }
            else if (sunRotation < 180f)
            {
                return 60f;
            }
            else if (sunRotation > 240f)
            {
                return 240f;
            }
            else
            {
                if (debugging) { ModConsole.Print("sunRotation out of range = " + sunRotation); }
                    return 0f;
            }
        }
        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => false;

        public override void OnNewGame()
        {
            // Called once, when starting a New Game, you can reset your saves here
        }

        public override void OnLoad()
        {
            if (debugging)
            {
                infoText = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter
                };
                infoText.normal.textColor = Color.yellow;
                infoText.fontSize = 18;
            }
            watchHour24 = new GameObject("watchHour24");
            watchHour24.AddComponent<MeshRenderer>();
            watchHour24.AddComponent<MeshFilter>();
            watchHour24.layer = 20;
            hourHandMaterial = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Hour/hour").GetComponent<MeshRenderer>().material;
            filter = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Hour/hour").GetComponent<MeshFilter>().mesh;
            watchHour24.transform.GetComponent<MeshRenderer>().material = hourHandMaterial;
            watchHour24.transform.GetComponent<MeshFilter>().mesh = filter;
            watchHour24.transform.SetParent(GameObject.Find("PLAYER").transform.FindChild("Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand"));
            watchHour24.transform.localPosition = new Vector3(0f, 0f, 0f);
            watchHour24.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            watchHour24.transform.localScale = new Vector3(1f, 1f, 1f);
            GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Hour").SetActive(false);
            ModConsole.Print("24 Hour Watch loaded");
            // Called once, when mod is loading after game is fully loaded

        }

        public override void ModSettings()
        {
            // All settings should be created here. 
            // DO NOT put anything else here that settings.
            Keybind.AddHeader(this, "Debugging");
            Keybind.Add(this, debugToggleKey);
        }

        public override void OnSave()
        {
            // Called once, when save and quit
            // Serialize your save file here.
        }

        public override void OnGUI()
        {
            // Draw unity OnGUI() here
            if (debugging)
            {
                base.OnGUI();
                GUI.Label(new Rect((float)(Screen.width / 2 - this.width / 2), (float)(Screen.height - this.height), (float)this.width, (float)this.height), this.textToShow, this.infoText);
            }
        }

        public override void Update()
        {
            //Update is called once per frame
            if (this.debugToggleKey.GetKeybindDown())
            {
                if (debugging)
                { debugging = false; }
                else
                { debugging = true; }
            }
                if (GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/").activeSelf)
            {
                if (debugging) { this.textToShow = ""; }
                rotateY = PlayMakerGlobals.Instance.Variables.FindFsmFloat("TimeRotationHour").Value / 2 * -1;
                if (debugging) { this.textToShow = PlayMakerGlobals.Instance.Variables.FindFsmFloat("TimeRotationHour").ToString() + "/2=" + rotateY.ToString() + "-"; }
                rotateY -= IsAfternoon();
                if (debugging) { this.textToShow = this.textToShow + IsAfternoon() + "=" + rotateY.ToString() + " " + sunRotation.ToString(); }
                if (!(PlayMakerGlobals.Instance.Variables.FindFsmFloat("TimeRotationHour").Value <= 0f))
                {
                    watchHour24.transform.localRotation = Quaternion.Euler(0f, 0f, rotateY);
                }
            }
        }
    }
}
