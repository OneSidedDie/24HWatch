using MSCLoader;
using UnityEngine;

namespace _24HWatch
{
    public class _24HWatch : Mod
    {
        public override string ID => "24HWatch"; //Your mod ID (unique)
        public override string Name => "24 Hour Watch"; //You mod name
        public override string Author => "OneSidedDie"; //Your Username
        public override string Version => "1.1"; //Version

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
        private string realMinuteString;
        private string realHourString;
        private int realHour;
        private int realMinute;
        private AssetBundle ab;
        private GameObject digitalWatch;
        private GameObject prefab;
        private TextMesh digitalTime;
        private TextMesh digitalDate;
        private TextMesh digitalDay;
        private string[] digitalDayArray = new string[] { "Er", "Mo", "Tu", "We", "Th", "Fr", "Sa", "Su" };
        private readonly static Settings testSlider = new Settings("slider", "Watch Mode", 2, AoD);
        private GameObject wristwatchHand;
        private float HourAdjust()
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
        private string DigitalTime()
        {
            realMinute = (int)GameObject.Find("MAP/SUN/Pivot/SUN").GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Minutes").Value;
            realHour = GameObject.Find("MAP/SUN/Pivot/SUN").GetComponent<PlayMakerFSM>().FsmVariables.FindFsmInt("Time").Value;
            if (realMinute > 59)
            {
                realHour += 1;
                realMinute -= 60;
            }
            realMinuteString = realMinute.ToString();
            if (realMinute < 10)
            {
                realMinuteString = "0" + realMinuteString;
            }
            realHourString = realHour.ToString();
            if (realHour < 10)
            {
                realHourString = "0" + realHourString;
            }
            if (realHour == 24)
            {
                realHourString = "00";
            }
            if (realHour == 25)
            {
                realHourString = "01";
            }
            return realHourString + ":" + realMinuteString;
        }

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => true;

        public override void OnNewGame()
        {
            // Called once, when starting a New Game, you can reset your saves here
        }

        public override void OnLoad()
        {
            // Called once, when mod is loading after game is fully loade
            wristwatchHand = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand");
            realHour = GameObject.Find("MAP/SUN/Pivot/SUN").GetComponent<PlayMakerFSM>().FsmVariables.FindFsmInt("Time").Value;
            realMinute = 0;
            infoText = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter
            };
            infoText.normal.textColor = Color.yellow;
            infoText.fontSize = 18;
            ab = LoadAssets.LoadBundle(this, "24hwatch.unity3d");
            prefab = ab.LoadAsset<GameObject>("digitalwatch.prefab");
            digitalWatch = GameObject.Instantiate(prefab);
            GameObject.Destroy(prefab);
            digitalDate = digitalWatch.transform.FindChild("digitaldate").gameObject.GetComponent<TextMesh>();
            digitalTime = digitalWatch.transform.FindChild("digitaltime").gameObject.GetComponent<TextMesh>();
            digitalDay = digitalWatch.transform.FindChild("digitalday").gameObject.GetComponent<TextMesh>();
            digitalWatch.transform.SetParent(wristwatchHand.transform);
            digitalWatch.transform.localPosition = new Vector3(0f, 0f, 0f);
            digitalWatch.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            digitalWatch.transform.localScale = new Vector3(1f, 1f, 1f);
            watchHour24 = new GameObject("watchHour24");
            watchHour24.AddComponent<MeshRenderer>();
            watchHour24.AddComponent<MeshFilter>();
            digitalWatch.layer = 20;
            watchHour24.layer = 20;
            hourHandMaterial = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Hour/hour").GetComponent<MeshRenderer>().material;
            filter = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Hour/hour").GetComponent<MeshFilter>().mesh;
            watchHour24.transform.GetComponent<MeshRenderer>().material = hourHandMaterial;
            watchHour24.transform.GetComponent<MeshFilter>().mesh = filter;
            watchHour24.transform.SetParent(wristwatchHand.transform);
            watchHour24.transform.localPosition = new Vector3(0f, 0f, 0f);
            watchHour24.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            watchHour24.transform.localScale = new Vector3(1f, 1f, 1f);
            AoD();
            ab.Unload(false);
            ModConsole.Print("24 Hour Watch loaded");
        }

        public override void ModSettings()
        {
            // All settings should be created here. 
            // DO NOT put anything else here that settings.
            /*if (debugging)
            {
                Keybind.AddHeader(this, "Debugging");
                Keybind.Add(this, debugToggleKey);
            }
            */
            Settings.AddHeader(this, "Analog 12H(0), Analog 24H(1), Digital 24H(2)");
            Settings.AddSlider(this, testSlider, 0, 2);
        }
        public static void AoD()
        {
            int sliderValue = int.Parse(testSlider.GetValue().ToString());
            if (sliderValue == 0)
            {
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/watchHour24").SetActive(false);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/digitalwatch(Clone)").SetActive(false);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/glass").SetActive(true);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Hour").SetActive(true);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/table").SetActive(true);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Minute").SetActive(true);
            }
            else if (sliderValue == 1)
            {
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/watchHour24").SetActive(true);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/digitalwatch(Clone)").SetActive(false);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/glass").SetActive(true);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Hour").SetActive(false);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/table").SetActive(true);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Minute").SetActive(true);
            }
            else if (sliderValue == 2)
            {
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/watchHour24").SetActive(false);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/digitalwatch(Clone)").SetActive(true);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/glass").SetActive(false);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Hour").SetActive(false);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/table").SetActive(false);
                GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera/Watch/Animate/BreathAnim/WristwatchHand/Clock/Pivot/Minute").SetActive(false);
            }
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
                GUI.Label(new Rect((float)(Screen.width / 2 - width / 2), (float)(Screen.height - height), (float)width, (float)height), textToShow, infoText);
            }
        }

        public override void Update()
        {
            //Update is called once per frame
            /*if (debugToggleKey.GetKeybindDown())
            {
                if (debugging)
                { debugging = false; }
                else
                { debugging = true; }
            }*/
            if (wristwatchHand.activeSelf)
            {
                if (watchHour24.activeSelf)
                {
                    if (debugging) { textToShow = ""; }
                    rotateY = PlayMakerGlobals.Instance.Variables.FindFsmFloat("TimeRotationHour").Value / 2 * -1;
                    if (debugging) { textToShow = PlayMakerGlobals.Instance.Variables.FindFsmFloat("TimeRotationHour").ToString() + "/2=" + rotateY.ToString() + "-"; }
                    rotateY -= HourAdjust();
                    if (debugging) { textToShow += HourAdjust() + "=" + rotateY.ToString() + " " + sunRotation.ToString(); }
                    if (!(PlayMakerGlobals.Instance.Variables.FindFsmFloat("TimeRotationHour").Value <= 0f))
                    {
                        watchHour24.transform.localRotation = Quaternion.Euler(0f, 0f, rotateY);
                    }
                }
                if (digitalWatch.activeSelf)
                {
                    digitalTime.text = DigitalTime();
                    digitalDay.text = digitalDayArray[PlayMakerGlobals.Instance.Variables.FindFsmInt("GlobalDay").Value];
                    digitalDate.text = System.DateTime.Now.ToString("dd") + ",Aug";
                    if (debugging) { textToShow += " " + DigitalTime(); }
                }
            }
        }
    }
}
