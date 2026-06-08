using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using HarmonyLib;

namespace CheatsMod
{
    using Utils;
    using UnityEngine.SceneManagement;

    enum CheatOptions
    {
        Fill_Fuel_and_Health,
        Toggle_Launch_Codes,
        Toggle_Eye_Coordinates,
        Toggle_All_Frequencies,
        Toggle_Rumors,
        Teleport_To_Sun,
        Teleport_To_SunStation,
        Teleport_To_EmberTwin,
        Teleport_To_AshTwin,
        Teleport_To_AshTwinProject,
        Teleport_To_TimberHearth,
        Teleport_To_SkyShutterSatellite,
        Teleport_To_Attlerock,
        Teleport_To_BrittleHollow,
        Teleport_To_HollowsLantern,
        Teleport_To_GiantsDeep,
        Teleport_To_ProbeCannon,
        Teleport_To_ProbeCannonTrackingModule,
        Teleport_To_DarkBramble,
        Teleport_To_Vessel,
        Teleport_To_Interloper,
        Teleport_To_WhiteHole,
        Teleport_To_WhiteHoleStation,
        Teleport_To_Stranger,
        Teleport_To_DreamWorld,
        Teleport_To_QuantumMoon,
        Teleport_To_Ship,
        Teleport_To_Probe,
        Teleport_To_Nomai_Probe,
        Teleport_To_Mapping_Satellite,
        Teleport_To_Backer_Satellite,
        Teleport_Ship_To_Player,
        Teleport_Menu,
        Toggle_Helmet,
        Toggle_Invincibility,
        Toggle_Ship_Invincibility,
        Toggle_Spacesuit,
        Toggle_Training_Suit,
        Toggle_Player_Gravity,
        Toggle_Ship_Gravity,
        Toggle_Player_Collision,
        Toggle_Ship_Collision,
        Toggle_Player_Fluid_Collision,
        Toggle_Ship_Fluid_Collision,
        Toggle_Unlimited_Boost,
        Toggle_Unlimited_Fuel,
        Toggle_Unlimited_Oxygen,
        Toggle_Unlimited_Health,
        Toggle_Anglerfish_AI,
        Toggle_Inhabitants_AI,
        Toggle_Inhabitants_Hostility,
        Toggle_Supernova_Timer,
        Decrease_Supernova_Timer,
        Increase_Supernova_Timer,
        Quantum_Moon_Collapse,
        Quantum_Moon_EmberTwin,
        Quantum_Moon_TimberHearth,
        Quantum_Moon_BrittleHollow,
        Quantum_Moon_GiantsDeep,
        Quantum_Moon_DarkBramble,
        Quantum_Moon_Eye,
        Decrease_Jetpack_Acceleration,
        Increase_Jetpack_Acceleration,
        Decrease_Ship_Acceleration,
        Increase_Ship_Acceleration,
        Give_Warp_Core_Vessel,
        Give_Warp_Core_Broken,
        Give_Warp_Core_Black,
        Give_Warp_Core_White,
        Give_Warp_Core_None,
        Give_Lantern_Basic,
        Give_Lantern_Broken,
        Give_Lantern_Invalid,
        Give_Lantern_Gen1,
        Give_Lantern_Gen2,
        Give_Lantern_Gen3,
        Give_Slide_Story_1,
        Give_Slide_Story_2,
        Give_Slide_Story_3,
        Give_Slide_Story_4,
        Give_Slide_Story_5,
        Give_Slide_Path_1,
        Give_Slide_Path_2,
        Give_Slide_Path_3,
        Give_Slide_Seal_1,
        Give_Slide_Seal_2,
        Give_Slide_Seal_3,
        Give_Slide_Rule_1,
        Give_Slide_Rule_2,
        Give_Slide_Rule_3,
        Give_Slide_Rule_4,
        Give_Slide_Burning,
        Give_Slide_Experiment,
        Give_Slide_DamageReport,
        Give_Slide_LanternSecret,
        Give_Slide_Prisoner,
        Give_Slide_PrisonerFarewell,
        Give_Slide_Tower,
        Give_Slide_SignalJammer,
        Give_Slide_Homeworld,
        Give_Slide_SupernovaEscape,
        Toggle_Fog,
        Give_Vision_Torch,
        Give_Identify_Conversation_Stone,
        Give_Explain_Conversation_Stone,
        Give_Eye_Conversation_Stone,
        Give_Quantum_Moon_Conversation_Stone,
        Give_You_Conversation_Stone,
        Give_Me_Conversation_Stone,
        Give_Nomai_Conversation_Stone,
        Give_Sun_Station_Projection_Stone,
        Give_Time_Loop_Projection_Stone,
        Give_Eye_Locator_Projection_Stone,
        Give_Mine_Projection_Stone,
        Give_Observatory_Projection_Stone,
        Give_Gravity_Cannon_Projection_Stone,
        Give_Quantum_Fragment_Projection_Stone,
        Give_Black_Hole_Forge_Projection_Stone,
        Give_Construction_Yard_1st_Projection_Stone,
        Give_Construction_Yard_2nd_Projection_Stone,
        Give_Statue_Projection_Stone,
        Give_Tracking_Module_Projection_Stone,
        Give_Launch_Module_Projection_Stone,
        Give_Control_Module_Projection_Stone,
        Give_Volcanic_Projection_Stone,
        Give_High_Energy_Lab_Projection_Stone,
        Give_North_Pole_Projection_Stone,
        Eject_Ship,
        Explode_Ship
    }

    [HarmonyPatch]
    public class MainClass : ModBehaviour
    {
        private const string version = "0.7.6";
        private ScreenPrompt cheatsTagger = new ScreenPrompt("");

        bool cheatsEnabled = true;
        bool watermark = true;
        bool thrustLimit = true;
        InputMapping<CheatOptions> inputs = new InputMapping<CheatOptions>();

        private static MainClass instance;
        public static MainClass Instance => instance;
        private static GameObject body;
        public static GameObject Body => body;
        public static IModHelper ModHelperInstance => instance.ModHelper;
        public static IModConsole Console => instance.ModHelper.Console;
        public static IHarmonyHelper HarmonyHelper => instance.ModHelper.HarmonyHelper;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //new Harmony("MegaPiggy.CheatsMod").PatchAll(Assembly.GetExecutingAssembly());
            ModHelper.Events.Player.OnPlayerAwake += (player) => onAwake();
            ModHelper.HarmonyHelper.AddPrefix(AccessTools.Method(typeof(OWExtensions), "GetAttachedOWRigidbody", new Type[2] { typeof(Component), typeof(bool) }), typeof(MainClass), nameof(OWExtensions_GetAttachedOWRigidbody));
            ModHelper.HarmonyHelper.AddPrefix<HighSpeedImpactSensor>("FixedUpdate", typeof(MainClass), nameof(HighSpeedImpactSensor_FixedUpdate));
            ModHelper.HarmonyHelper.AddPrefix<PlayerResources>("OnImpact", typeof(MainClass), nameof(PlayerResources_OnImpact));
            ModHelper.HarmonyHelper.AddPostfix<ThrustRuleset>("GetThrustLimit", typeof(MainClass), nameof(ThrustRuleset_GetThrustLimit));
            ModHelper.Console.WriteLine("CheatMods ready!");
            Position.Awake();
            Items.Awake();
            Items.Start();
            Fog.Start();
            Anglerfish.Start();
            Inhabitants.Start();
            SceneManager.sceneLoaded += (s, l) => OnExitDreamWorld();
            SceneManager.sceneUnloaded += (s) => OnExitDreamWorld();
            GlobalMessenger.AddListener("EnterDreamWorld", OnEnterDreamWorld);
            GlobalMessenger.AddListener("ExitDreamWorld", OnExitDreamWorld);
            body = new GameObject("CheatsMod_Body", typeof(OWRigidbody));
            var sectorObj = new GameObject("Sector_CheatsMod");
            sectorObj.SetActive(false);
            sectorObj.transform.SetParent(body.transform);
            var sector = sectorObj.AddComponent<Sector>();
            sector._name = Sector.Name.Unnamed;
            sector._subsectors = new List<Sector>();
            sectorObj.SetActive(true);
            GameObject.DontDestroyOnLoad(body);
            SceneManager.sceneUnloaded += (s) => Anglerfish.Clear();
        }

        public static void ThrustRuleset_GetThrustLimit(ref float __result)
        {
            if (Instance != null && !Instance.thrustLimit)
            {
                __result *= 100;
            }
        }

        void Destory()
        {
            ModHelper.Console.WriteLine("CheatMods 清理完毕!");
        }

        public static bool HighSpeedImpactSensor_FixedUpdate() => !Player.isInvincible;
        public static bool PlayerResources_OnImpact() => !Player.isInvincible;

        public static bool OWExtensions_GetAttachedOWRigidbody(ref OWRigidbody __result, Component cmpt)
        {
            if (cmpt == null || cmpt.gameObject == null || cmpt.transform == null)
            {
                __result = null;
                return false;
            }
            return true;
        }

        public override void Configure(IModConfig config)
        {
            cheatsEnabled = config.Enabled;

            watermark = ConfigHelper.getConfigOrDefault<bool>(config, "Watermark", true);

            Player.isInvincible = ConfigHelper.getConfigOrDefault<bool>(config, "Player Invincible", false);
            Ship.isInvincible = ConfigHelper.getConfigOrDefault<bool>(config, "Ship Invincible", false);

            thrustLimit = ConfigHelper.getConfigOrDefault<bool>(config, "Thrust Limit", false);

            Player.hasUnlimitedFuel = ConfigHelper.getConfigOrDefault<bool>(config, "Unlimited Fuel", false);
            Ship.hasUnlimitedFuel = Player.hasUnlimitedFuel;

            Player.hasUnlimitedOxygen = ConfigHelper.getConfigOrDefault<bool>(config, "Unlimited Oxygen", false);
            Ship.hasUnlimitedOxygen = Player.hasUnlimitedOxygen;

            Player.hasUnlimitedHealth = ConfigHelper.getConfigOrDefault<bool>(config, "Unlimited Health", false);
            Player.hasUnlimitedBoost = ConfigHelper.getConfigOrDefault<bool>(config, "Unlimited Boost", false);

            Anglerfish.enabledAI = ConfigHelper.getConfigOrDefault<bool>(config, "Anglerfish AI", true);
            Inhabitants.enabledAI = ConfigHelper.getConfigOrDefault<bool>(config, "Inhabitants AI", true);
            Fog.enabled = ConfigHelper.getConfigOrDefault<bool>(config, "Fog", true);

            inputs.Clear();
            inputs.addInput(config, CheatOptions.Fill_Fuel_and_Health, "C,J");
            inputs.addInput(config, CheatOptions.Toggle_Launch_Codes, "C,L");
            inputs.addInput(config, CheatOptions.Toggle_Eye_Coordinates, "C,E");
            inputs.addInput(config, CheatOptions.Toggle_All_Frequencies, "C,F");
            inputs.addInput(config, CheatOptions.Toggle_Rumors, "C,R");
            inputs.addInput(config, CheatOptions.Toggle_Helmet, "C,H");
            inputs.addInput(config, CheatOptions.Toggle_Invincibility, "C,I");
            inputs.addInput(config, CheatOptions.Toggle_Ship_Invincibility, "C,Q");
            inputs.addInput(config, CheatOptions.Toggle_Spacesuit, "C,G");
            inputs.addInput(config, CheatOptions.Toggle_Training_Suit, "C,Digit1");
            inputs.addInput(config, CheatOptions.Toggle_Player_Gravity, "P,G");
            inputs.addInput(config, CheatOptions.Toggle_Ship_Gravity, "S,G");
            inputs.addInput(config, CheatOptions.Toggle_Player_Collision, "P,C");
            inputs.addInput(config, CheatOptions.Toggle_Ship_Collision, "S,C");
            inputs.addInput(config, CheatOptions.Toggle_Player_Fluid_Collision, "P,F");
            inputs.addInput(config, CheatOptions.Toggle_Ship_Fluid_Collision, "S,F");
            inputs.addInput(config, CheatOptions.Toggle_Unlimited_Boost, "C,T");
            inputs.addInput(config, CheatOptions.Toggle_Unlimited_Fuel, "C,Y");
            inputs.addInput(config, CheatOptions.Toggle_Unlimited_Oxygen, "C,O");
            inputs.addInput(config, CheatOptions.Toggle_Unlimited_Health, "C,U");
            inputs.addInput(config, CheatOptions.Toggle_Fog, "F,O,G");

            inputs.addInput(config, CheatOptions.Teleport_To_Sun, "T,Digit1");
            inputs.addInput(config, CheatOptions.Teleport_To_SunStation, "T,Digit2");
            inputs.addInput(config, CheatOptions.Teleport_To_EmberTwin, "T,Digit3");
            inputs.addInput(config, CheatOptions.Teleport_To_AshTwin, "T,Digit4");
            inputs.addInput(config, CheatOptions.Teleport_To_TimberHearth, "T,Digit5");
            inputs.addInput(config, CheatOptions.Teleport_To_Attlerock, "T,Digit6");
            inputs.addInput(config, CheatOptions.Teleport_To_BrittleHollow, "T,Digit7");
            inputs.addInput(config, CheatOptions.Teleport_To_HollowsLantern, "T,Digit8");
            inputs.addInput(config, CheatOptions.Teleport_To_GiantsDeep, "T,Digit9");
            inputs.addInput(config, CheatOptions.Teleport_To_DarkBramble, "T,Digit0");

            inputs.addInput(config, CheatOptions.Teleport_To_Interloper, "T,Numpad0");
            inputs.addInput(config, CheatOptions.Teleport_To_SkyShutterSatellite, "T,Numpad1");
            inputs.addInput(config, CheatOptions.Teleport_To_ProbeCannon, "T,Numpad2");
            inputs.addInput(config, CheatOptions.Teleport_To_WhiteHole, "T,Numpad3");
            inputs.addInput(config, CheatOptions.Teleport_To_WhiteHoleStation, "T,Numpad4");
            inputs.addInput(config, CheatOptions.Teleport_To_Stranger, "T,Numpad5");
            inputs.addInput(config, CheatOptions.Teleport_To_DreamWorld, "T,Numpad6");
            inputs.addInput(config, CheatOptions.Teleport_To_QuantumMoon, "T,Numpad7");
            inputs.addInput(config, CheatOptions.Teleport_To_AshTwinProject, "T,Numpad8");
            inputs.addInput(config, CheatOptions.Teleport_To_Ship, "T,Numpad9");
            inputs.addInput(config, CheatOptions.Teleport_Ship_To_Player, "T,NumpadDivide");
            inputs.addInput(config, CheatOptions.Teleport_To_Probe, "T,NumpadMultiply");
            inputs.addInput(config, CheatOptions.Teleport_To_Nomai_Probe, "T,NumpadMinus");
            inputs.addInput(config, CheatOptions.Teleport_To_Vessel, "T,NumpadPlus");
            inputs.addInput(config, CheatOptions.Teleport_To_ProbeCannonTrackingModule, "T,NumpadPeriod");

            inputs.addInput(config, CheatOptions.Teleport_To_Backer_Satellite, "T,B");
            inputs.addInput(config, CheatOptions.Teleport_To_Mapping_Satellite, "T,M");

            inputs.addInput(config, CheatOptions.Toggle_Anglerfish_AI, "V,I");
            inputs.addInput(config, CheatOptions.Toggle_Inhabitants_AI, "V,O");
            inputs.addInput(config, CheatOptions.Toggle_Inhabitants_Hostility, "V,H");
            inputs.addInput(config, CheatOptions.Toggle_Supernova_Timer, "V,0");
            inputs.addInput(config, CheatOptions.Decrease_Supernova_Timer, "V,Minus");
            inputs.addInput(config, CheatOptions.Increase_Supernova_Timer, "V,Equals");

            inputs.addInput(config, CheatOptions.Quantum_Moon_Collapse, "Q,Digit0");
            inputs.addInput(config, CheatOptions.Quantum_Moon_EmberTwin, "Q,Digit1");
            inputs.addInput(config, CheatOptions.Quantum_Moon_TimberHearth, "Q,Digit2");
            inputs.addInput(config, CheatOptions.Quantum_Moon_BrittleHollow, "Q,Digit3");
            inputs.addInput(config, CheatOptions.Quantum_Moon_GiantsDeep, "Q,Digit4");
            inputs.addInput(config, CheatOptions.Quantum_Moon_DarkBramble, "Q,Digit5");
            inputs.addInput(config, CheatOptions.Quantum_Moon_Eye, "Q,Digit6");
            inputs.addInput(config, CheatOptions.Decrease_Jetpack_Acceleration, "P,Minus");
            inputs.addInput(config, CheatOptions.Increase_Jetpack_Acceleration, "P,Equals");
            inputs.addInput(config, CheatOptions.Decrease_Ship_Acceleration, "O,Minus");
            inputs.addInput(config, CheatOptions.Increase_Ship_Acceleration, "O,Equals");

            inputs.addInput(config, CheatOptions.Give_Warp_Core_Vessel, "G,T,Digit1");
            inputs.addInput(config, CheatOptions.Give_Warp_Core_Broken, "G,T,Digit2");
            inputs.addInput(config, CheatOptions.Give_Warp_Core_Black, "G,T,Digit3");
            inputs.addInput(config, CheatOptions.Give_Warp_Core_White, "G,T,Digit4");
            inputs.addInput(config, CheatOptions.Give_Warp_Core_None, "G,T,Digit5");
            inputs.addInput(config, CheatOptions.Give_Lantern_Basic, "G,L,Digit1");
            inputs.addInput(config, CheatOptions.Give_Lantern_Broken, "G,L,Digit2");
            inputs.addInput(config, CheatOptions.Give_Lantern_Gen1, "G,L,Digit3");
            inputs.addInput(config, CheatOptions.Give_Lantern_Gen2, "G,L,Digit4");
            inputs.addInput(config, CheatOptions.Give_Lantern_Gen3, "G,L,Digit5");
            inputs.addInput(config, CheatOptions.Give_Vision_Torch, "G,L,Digit6");
            inputs.addInput(config, CheatOptions.Give_Slide_Story_1, "G,R,S,Digit1");
            inputs.addInput(config, CheatOptions.Give_Slide_Story_2, "G,R,S,Digit2");
            inputs.addInput(config, CheatOptions.Give_Slide_Story_3, "G,R,S,Digit3");
            inputs.addInput(config, CheatOptions.Give_Slide_Story_4, "G,R,S,Digit4");
            inputs.addInput(config, CheatOptions.Give_Slide_Story_5, "G,R,S,Digit5");
            inputs.addInput(config, CheatOptions.Give_Slide_Burning, "G,R,S,Digit6");
            inputs.addInput(config, CheatOptions.Give_Slide_Experiment, "G,R,S,Digit7");
            inputs.addInput(config, CheatOptions.Give_Slide_DamageReport, "G,R,S,Digit8");
            inputs.addInput(config, CheatOptions.Give_Slide_Prisoner, "G,R,S,Numpad0");
            inputs.addInput(config, CheatOptions.Give_Slide_PrisonerFarewell, "G,R,S,Numpad1");
            inputs.addInput(config, CheatOptions.Give_Slide_Tower, "G,R,S,Numpad2");
            inputs.addInput(config, CheatOptions.Give_Slide_SignalJammer, "G,R,S,Numpad3");
            inputs.addInput(config, CheatOptions.Give_Slide_Homeworld, "G,R,S,Numpad4");
            inputs.addInput(config, CheatOptions.Give_Slide_SupernovaEscape, "G,R,S,Numpad5");
            inputs.addInput(config, CheatOptions.Give_Slide_Path_1, "G,R,H,Digit1");
            inputs.addInput(config, CheatOptions.Give_Slide_Path_2, "G,R,H,Digit2");
            inputs.addInput(config, CheatOptions.Give_Slide_Path_3, "G,R,H,Digit3");
            inputs.addInput(config, CheatOptions.Give_Slide_Seal_1, "G,R,H,Digit4");
            inputs.addInput(config, CheatOptions.Give_Slide_Seal_2, "G,R,H,Digit5");
            inputs.addInput(config, CheatOptions.Give_Slide_Seal_3, "G,R,H,Digit6");
            inputs.addInput(config, CheatOptions.Give_Slide_Rule_1, "G,R,H,Digit7");
            inputs.addInput(config, CheatOptions.Give_Slide_Rule_2, "G,R,H,Digit8");
            inputs.addInput(config, CheatOptions.Give_Slide_Rule_3, "G,R,H,Digit9");
            inputs.addInput(config, CheatOptions.Give_Slide_Rule_4, "G,R,H,Digit0");

            inputs.addInput(config, CheatOptions.Give_Identify_Conversation_Stone, "C,Digit2");
            inputs.addInput(config, CheatOptions.Give_Explain_Conversation_Stone, "C,Digit3");
            inputs.addInput(config, CheatOptions.Give_Eye_Conversation_Stone, "C,Digit4");
            inputs.addInput(config, CheatOptions.Give_Quantum_Moon_Conversation_Stone, "C,Digit5");
            inputs.addInput(config, CheatOptions.Give_You_Conversation_Stone, "C,Digit6");
            inputs.addInput(config, CheatOptions.Give_Me_Conversation_Stone, "C,Digit7");
            inputs.addInput(config, CheatOptions.Give_Nomai_Conversation_Stone, "C,Digit8");

            inputs.addInput(config, CheatOptions.Give_Sun_Station_Projection_Stone, "P,S,Digit1");
            inputs.addInput(config, CheatOptions.Give_Time_Loop_Projection_Stone, "P,S,Digit2");
            inputs.addInput(config, CheatOptions.Give_Eye_Locator_Projection_Stone, "P,S,Digit3");
            inputs.addInput(config, CheatOptions.Give_Mine_Projection_Stone, "P,S,Digit4");
            inputs.addInput(config, CheatOptions.Give_Observatory_Projection_Stone, "P,S,Digit5");
            inputs.addInput(config, CheatOptions.Give_Gravity_Cannon_Projection_Stone, "P,S,Digit6");
            inputs.addInput(config, CheatOptions.Give_Quantum_Fragment_Projection_Stone, "P,S,Digit7");
            inputs.addInput(config, CheatOptions.Give_Black_Hole_Forge_Projection_Stone, "P,S,Digit8");
            inputs.addInput(config, CheatOptions.Give_Construction_Yard_1st_Projection_Stone, "P,S,Digit9");
            inputs.addInput(config, CheatOptions.Give_Construction_Yard_2nd_Projection_Stone, "P,S,Digit0");
            inputs.addInput(config, CheatOptions.Give_Statue_Projection_Stone, "P,S,Numpad1");
            inputs.addInput(config, CheatOptions.Give_Tracking_Module_Projection_Stone, "P,S,Numpad2");
            inputs.addInput(config, CheatOptions.Give_Launch_Module_Projection_Stone, "P,S,Numpad3");
            inputs.addInput(config, CheatOptions.Give_Control_Module_Projection_Stone, "P,S,Numpad4");
            inputs.addInput(config, CheatOptions.Give_Volcanic_Projection_Stone, "P,S,Numpad5");
            inputs.addInput(config, CheatOptions.Give_High_Energy_Lab_Projection_Stone, "P,S,Numpad6");
            inputs.addInput(config, CheatOptions.Give_North_Pole_Projection_Stone, "P,S,Numpad7");

            inputs.addInput(config, CheatOptions.Eject_Ship, "LeftAlt,Digit1");
            inputs.addInput(config, CheatOptions.Explode_Ship, "LeftAlt,Digit2");

            ModHelper.Console.WriteLine("CheatMods 配置完成!");
        }

        public bool inDreamWorld = false;

        public void OnEnterDreamWorld()
        {
            inDreamWorld = true;
        }

        public void OnExitDreamWorld()
        {
            inDreamWorld = false;
        }

        void onAwake()
        {
            ModHelper.Console.WriteLine("CheatMods: 玩家苏醒");
            Position.Awake();
            Items.Awake();
            //GameObject.DontDestroyOnLoad(new GameObject("LudicrousSpeed", typeof(LudicrousSpeed)));
        }

        void OnGUI()
        {
            cheatsTagger.SetText("CheatsMod v" + version + ": " + (cheatsEnabled ? "Enabled" : "Disabled"));
            if (watermark)
            {
                if (Locator.GetPromptManager()?.GetScreenPromptList(PromptPosition.LowerLeft)?.Contains(cheatsTagger) == false)
                {
                    Locator.GetPromptManager().AddScreenPrompt(cheatsTagger, PromptPosition.LowerLeft, true);
                }
            }
            else
            {
                if (Locator.GetPromptManager()?.GetScreenPromptList(PromptPosition.LowerLeft)?.Contains(cheatsTagger) == true)
                {
                    Locator.GetPromptManager().RemoveScreenPrompt(cheatsTagger, PromptPosition.LowerLeft);
                }
            }
        }

        void Update()
        {
            inputs.Update();
            if (cheatsEnabled)
            {
                var currentFrame = inputs.getPressedThisFrame();
                currentFrame = currentFrame.FindAll(x => x.Item2.keyMatchCount() == currentFrame[0].Item2.keyMatchCount());

                foreach (var input in currentFrame)
                {
                    Console.WriteLine(input.Item1.ToString());
                    switch (input.Item1)
                    {
                        case CheatOptions.Fill_Fuel_and_Health:
                            Player.oxygenSeconds = Player.maxOxygenSeconds;
                            Player.fuelSeconds = Player.maxFuelSeconds;
                            Player.health = Player.maxHealth;
                            Player.boostSeconds = Player.maxBoostSeconds;
                            Ship.repair();
                            break;
                        case CheatOptions.Eject_Ship:
                            Ship.eject();
                            break;
                        case CheatOptions.Explode_Ship:
                            Ship.explode();
                            break;
                        case CheatOptions.Toggle_Launch_Codes:
                            Data.launchCodes = !Data.launchCodes;
                            ModHelper.Console.WriteLine("CheatsMod: Launch Codes Known " + Data.launchCodes);
                            break;
                        case CheatOptions.Toggle_Eye_Coordinates:
                            Data.eyeCoordinates = !Data.eyeCoordinates;
                            ModHelper.Console.WriteLine("CheatsMod: Eye Coordinates Known " + Data.eyeCoordinates);
                            break;
                        case CheatOptions.Toggle_All_Frequencies:
                            toggleFrequencies();
                            break;
                        case CheatOptions.Toggle_Rumors:
                            toggleFacts();
                            break;
                        case CheatOptions.Teleport_To_Sun:
                            Teleportation.teleportPlayerToSun();
                            break;
                        case CheatOptions.Teleport_To_SunStation:
                            Teleportation.teleportPlayerToSunStation();
                            break;
                        case CheatOptions.Teleport_To_EmberTwin:
                            Teleportation.teleportPlayerToEmberTwin();
                            break;
                        case CheatOptions.Teleport_To_AshTwin:
                            Teleportation.teleportPlayerToAshTwin();
                            break;
                        case CheatOptions.Teleport_To_AshTwinProject:
                            Teleportation.teleportPlayerToAshTwinProject();
                            break;
                        case CheatOptions.Teleport_To_TimberHearth:
                            Teleportation.teleportPlayerToTimberHearth();
                            break;
                        case CheatOptions.Teleport_To_SkyShutterSatellite:
                            Teleportation.teleportPlayerToSkyShutterSatellite();
                            break;
                        case CheatOptions.Teleport_To_Attlerock:
                            Teleportation.teleportPlayerToAttlerock();
                            break;
                        case CheatOptions.Teleport_To_BrittleHollow:
                            Teleportation.teleportPlayerToBlackHoleForgeTeleporter();
                            break;
                        case CheatOptions.Teleport_To_HollowsLantern:
                            Teleportation.teleportPlayerToHollowsLantern();
                            break;
                        case CheatOptions.Teleport_To_GiantsDeep:
                            Teleportation.teleportPlayerToGiantsDeep();
                            break;
                        case CheatOptions.Teleport_To_ProbeCannon:
                            Teleportation.teleportPlayerToProbeCannon();
                            break;
                        case CheatOptions.Teleport_To_ProbeCannonTrackingModule:
                            Teleportation.teleportPlayerToProbeCannonTrackingModule();
                            break;
                        case CheatOptions.Teleport_To_DarkBramble:
                            Teleportation.teleportPlayerToDarkBramble();
                            break;
                        case CheatOptions.Teleport_To_Vessel:
                            Teleportation.teleportPlayerToVessel();
                            break;
                        case CheatOptions.Teleport_To_Ship:
                            Teleportation.teleportPlayerToShip();
                            break;
                        case CheatOptions.Teleport_Ship_To_Player:
                            Teleportation.teleportShipToPlayer();
                            break;
                        case CheatOptions.Teleport_To_Probe:
                            Teleportation.teleportPlayerToProbe();
                            break;
                        case CheatOptions.Teleport_To_Nomai_Probe:
                            Teleportation.teleportPlayerToNomaiProbe();
                            break;
                        case CheatOptions.Teleport_To_Interloper:
                            Teleportation.teleportPlayerToInterloper();
                            break;
                        case CheatOptions.Teleport_To_WhiteHole:
                            Teleportation.teleportPlayerToWhiteHole();
                            break;
                        case CheatOptions.Teleport_To_WhiteHoleStation:
                            Teleportation.teleportPlayerToWhiteHoleStation();
                            break;
                        case CheatOptions.Teleport_To_Stranger:
                            Teleportation.teleportPlayerToStranger();
                            break;
                        case CheatOptions.Teleport_To_DreamWorld:
                            Teleportation.teleportPlayerToDreamWorld();
                            break;
                        case CheatOptions.Teleport_To_QuantumMoon:
                            Teleportation.teleportPlayerToQuantumMoon();
                            break;
                        case CheatOptions.Teleport_To_Mapping_Satellite:
                            Teleportation.teleportPlayerToMappingSatellite();
                            break;
                        case CheatOptions.Teleport_To_Backer_Satellite:
                            Teleportation.teleportPlayerToBackerSatellite();
                            break;
                        case CheatOptions.Toggle_Helmet:
                            Player.helmet = !Player.helmet;
                            ModHelper.Console.WriteLine("CheatsMod: Player Helmet " + Player.helmet);
                            break;
                        case CheatOptions.Toggle_Player_Gravity:
                            Player.gravity = !Player.gravity;
                            ModHelper.Console.WriteLine("CheatsMod: Player Gravity " + Player.gravity);
                            break;
                        case CheatOptions.Toggle_Ship_Gravity:
                            Ship.gravity = !Ship.gravity;
                            ModHelper.Console.WriteLine("CheatsMod: Ship Gravity " + Ship.gravity);
                            break;
                        case CheatOptions.Toggle_Player_Collision:
                            Player.collision = !Player.collision;
                            ModHelper.Console.WriteLine("CheatsMod: Player Collision " + Player.collision);
                            break;
                        case CheatOptions.Toggle_Ship_Collision:
                            Ship.collision = !Ship.collision;
                            ModHelper.Console.WriteLine("CheatsMod: Ship Collision " + Ship.collision);
                            break;
                        case CheatOptions.Toggle_Player_Fluid_Collision:
                            Player.fluidCollision = !Player.fluidCollision;
                            ModHelper.Console.WriteLine("CheatsMod: Player Fluid Collision " + Player.fluidCollision);
                            break;
                        case CheatOptions.Toggle_Ship_Fluid_Collision:
                            Ship.fluidCollision = !Ship.fluidCollision;
                            ModHelper.Console.WriteLine("CheatsMod: Ship Fluid Collision " + Ship.fluidCollision);
                            break;
                        case CheatOptions.Toggle_Training_Suit:
                            Player.trainingSuit = !Player.trainingSuit;
                            ModHelper.Console.WriteLine("CheatsMod: Training Suit " + Player.trainingSuit);
                            break;
                        case CheatOptions.Toggle_Spacesuit:
                            Player.spaceSuit = !Player.spaceSuit;
                            ModHelper.Console.WriteLine("CheatsMod: Space Suit " + Player.spaceSuit);
                            break;
                        case CheatOptions.Toggle_Invincibility:
                            Player.isInvincible = !Player.isInvincible;
                            ModHelper.Console.WriteLine("CheatsMod: Player Invincible " + Player.isInvincible);
                            break;
                        case CheatOptions.Toggle_Ship_Invincibility:
                            Ship.isInvincible = !Ship.isInvincible;
                            ModHelper.Console.WriteLine("CheatsMod: Ship Invincible " + Ship.isInvincible);
                            break;
                        case CheatOptions.Toggle_Unlimited_Fuel:
                            Player.hasUnlimitedFuel = !Player.hasUnlimitedFuel;
                            Ship.hasUnlimitedFuel = Player.hasUnlimitedFuel;
                            ModHelper.Console.WriteLine("CheatsMod: Unlimited Fuel " + Player.hasUnlimitedFuel);
                            break;
                        case CheatOptions.Toggle_Unlimited_Boost:
                            Player.hasUnlimitedBoost = !Player.hasUnlimitedBoost;
                            ModHelper.Console.WriteLine("CheatsMod: Unlimited Boost " + Player.hasUnlimitedBoost);
                            break;
                        case CheatOptions.Toggle_Unlimited_Health:
                            Player.hasUnlimitedHealth = !Player.hasUnlimitedHealth;
                            ModHelper.Console.WriteLine("CheatsMod: Unlimited Health " + Player.hasUnlimitedHealth);
                            break;
                        case CheatOptions.Toggle_Unlimited_Oxygen:
                            Player.hasUnlimitedOxygen = !Player.hasUnlimitedOxygen;
                            Ship.hasUnlimitedOxygen = Player.hasUnlimitedOxygen;
                            ModHelper.Console.WriteLine("CheatsMod: Unlimited Oxygen " + Player.hasUnlimitedOxygen);
                            break;
                        case CheatOptions.Toggle_Anglerfish_AI:
                            Anglerfish.enabledAI = !Anglerfish.enabledAI;
                            ModHelper.Console.WriteLine("CheatsMod: Anglerfish AI " + Anglerfish.enabledAI);
                            break;
                        case CheatOptions.Toggle_Inhabitants_AI:
                            Inhabitants.enabledAI = !Inhabitants.enabledAI;
                            ModHelper.Console.WriteLine("CheatsMod: Inhabitants AI " + Inhabitants.enabledAI);
                            break;
                        case CheatOptions.Toggle_Inhabitants_Hostility:
                            Inhabitants.enabledHostility = !Inhabitants.enabledHostility;
                            ModHelper.Console.WriteLine("CheatsMod: Inhabitants Hostility " + Inhabitants.enabledHostility);
                            break;
                        case CheatOptions.Toggle_Supernova_Timer:
                            SuperNova.freeze = !SuperNova.freeze;
                            ModHelper.Console.WriteLine("CheatsMod: SuperNova Frozen " + SuperNova.freeze);
                            break;
                        case CheatOptions.Decrease_Supernova_Timer:
                            SuperNova.remaining -= 60f;
                            ModHelper.Console.WriteLine("CheatsMod: Remaining Time " + SuperNova.remaining);
                            break;
                        case CheatOptions.Increase_Supernova_Timer:
                            SuperNova.remaining += 60f;
                            ModHelper.Console.WriteLine("CheatsMod: Remaining Time " + SuperNova.remaining);
                            break;
                        case CheatOptions.Decrease_Jetpack_Acceleration:
                            Player.thrust = Player.thrust / 2f;
                            ModHelper.Console.WriteLine("CheatsMod: JetPack Acceleration Multiplier " + (Player.thrust / 6f));
                            break;
                        case CheatOptions.Increase_Jetpack_Acceleration:
                            Player.thrust = Player.thrust * 2f;
                            ModHelper.Console.WriteLine("CheatsMod: JetPack Acceleration Multiplier " + (Player.thrust / 6f));
                            break;
                        case CheatOptions.Decrease_Ship_Acceleration:
                            Ship.thrust = Ship.thrust / 2f;
                            ModHelper.Console.WriteLine("CheatsMod: Ship Acceleration Multiplier " + (Ship.thrust / 50f));
                            break;
                        case CheatOptions.Increase_Ship_Acceleration:
                            Ship.thrust = Ship.thrust * 2f;
                            ModHelper.Console.WriteLine("CheatsMod: Ship Acceleration Multiplier " + (Ship.thrust / 50f));
                            break;
                        case CheatOptions.Quantum_Moon_Collapse:
                            QuantumMoonHelper.collapse();
                            break;
                        case CheatOptions.Quantum_Moon_EmberTwin:
                            QuantumMoonHelper.setState(AstroObject.Name.HourglassTwins);
                            break;
                        case CheatOptions.Quantum_Moon_TimberHearth:
                            QuantumMoonHelper.setState(AstroObject.Name.TimberHearth);
                            break;
                        case CheatOptions.Quantum_Moon_BrittleHollow:
                            QuantumMoonHelper.setState(AstroObject.Name.BrittleHollow);
                            break;
                        case CheatOptions.Quantum_Moon_GiantsDeep:
                            QuantumMoonHelper.setState(AstroObject.Name.GiantsDeep);
                            break;
                        case CheatOptions.Quantum_Moon_DarkBramble:
                            QuantumMoonHelper.setState(AstroObject.Name.DarkBramble);
                            break;
                        case CheatOptions.Quantum_Moon_Eye:
                            QuantumMoonHelper.setState(AstroObject.Name.Eye);
                            break;
                        case CheatOptions.Give_Warp_Core_Vessel:
                            Possession.pickUpWarpCore(WarpCoreType.Vessel);
                            break;
                        case CheatOptions.Give_Warp_Core_Broken:
                            Possession.pickUpWarpCore(WarpCoreType.VesselBroken);
                            break;
                        case CheatOptions.Give_Warp_Core_Black:
                            Possession.pickUpWarpCore(WarpCoreType.Black);
                            break;
                        case CheatOptions.Give_Warp_Core_White:
                            Possession.pickUpWarpCore(WarpCoreType.White);
                            break;
                        case CheatOptions.Give_Warp_Core_None:
                            Possession.pickUpWarpCore(WarpCoreType.SimpleBroken);
                            break;
                        case CheatOptions.Give_Lantern_Basic:
                            Possession.pickUpLantern(false, true);
                            break;
                        case CheatOptions.Give_Lantern_Broken:
                            Possession.pickUpLantern(true, false);
                            break;
                        case CheatOptions.Give_Lantern_Gen1:
                            Possession.pickUpDreamLantern(DreamLanternType.Nonfunctioning, false);
                            break;
                        case CheatOptions.Give_Lantern_Gen2:
                            Possession.pickUpDreamLantern(DreamLanternType.Malfunctioning, true);
                            break;
                        case CheatOptions.Give_Lantern_Gen3:
                            Possession.pickUpDreamLantern(DreamLanternType.Functioning, false);
                            break;
                        case CheatOptions.Give_Slide_Story_1:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Story_1, false);
                            break;
                        case CheatOptions.Give_Slide_Story_2:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Story_2, false);
                            break;
                        case CheatOptions.Give_Slide_Story_3:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Story_3, false);
                            break;
                        case CheatOptions.Give_Slide_Story_4:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Story_4, false);
                            break;
                        case CheatOptions.Give_Slide_Story_5:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Story_5_Complete, false);
                            break;
                        case CheatOptions.Give_Slide_Path_1:
                            Possession.pickUpSlideReel(Items.SlideReelStory.LibraryPath_1, false);
                            break;
                        case CheatOptions.Give_Slide_Path_2:
                            Possession.pickUpSlideReel(Items.SlideReelStory.LibraryPath_2, false);
                            break;
                        case CheatOptions.Give_Slide_Path_3:
                            Possession.pickUpSlideReel(Items.SlideReelStory.LibraryPath_3, false);
                            break;
                        case CheatOptions.Give_Slide_Seal_1:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Seal_1, false);
                            break;
                        case CheatOptions.Give_Slide_Seal_2:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Seal_2, false);
                            break;
                        case CheatOptions.Give_Slide_Seal_3:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Seal_3, false);
                            break;
                        case CheatOptions.Give_Slide_Rule_1:
                            Possession.pickUpSlideReel(Items.SlideReelStory.DreamRule_1, false);
                            break;
                        case CheatOptions.Give_Slide_Rule_2:
                            Possession.pickUpSlideReel(Items.SlideReelStory.DreamRule_2_v1, false);
                            break;
                        case CheatOptions.Give_Slide_Rule_3:
                            Possession.pickUpSlideReel(Items.SlideReelStory.DreamRule_2_v2, false);
                            break;
                        case CheatOptions.Give_Slide_Rule_4:
                            Possession.pickUpSlideReel(Items.SlideReelStory.DreamRule_3, false);
                            break;
                        case CheatOptions.Give_Slide_Burning:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Burning, false);
                            break;
                        case CheatOptions.Give_Slide_Experiment:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Experiment, false);
                            break;
                        case CheatOptions.Give_Slide_DamageReport:
                            Possession.pickUpSlideReel(Items.SlideReelStory.DamageReport, false);
                            break;
                        case CheatOptions.Give_Slide_LanternSecret:
                            Possession.pickUpSlideReel(Items.SlideReelStory.LanternSecret, false);
                            break;
                        case CheatOptions.Give_Slide_Prisoner:
                            Possession.pickUpSlideReel(Items.SlideReelStory.PrisonPeephole_Vision, false);
                            break;
                        case CheatOptions.Give_Slide_PrisonerFarewell:
                            Possession.pickUpSlideReel(Items.SlideReelStory.PrisonerFarewellVision, false);
                            break;
                        case CheatOptions.Give_Slide_Tower:
                            Possession.pickUpSlideReel(Items.SlideReelStory.TowerVision, false);
                            break;
                        case CheatOptions.Give_Slide_SignalJammer:
                            Possession.pickUpSlideReel(Items.SlideReelStory.SignalJammer, false);
                            break;
                        case CheatOptions.Give_Slide_Homeworld:
                            Possession.pickUpSlideReel(Items.SlideReelStory.Homeworld, false);
                            break;
                        case CheatOptions.Give_Slide_SupernovaEscape:
                            Possession.pickUpSlideReel(Items.SlideReelStory.SupernovaEscape, false);
                            break;
                        case CheatOptions.Toggle_Fog:
                            Fog.enabled = !Fog.enabled;
                            ModHelper.Console.WriteLine("CheatsMod: Fog " + Fog.enabled);
                            break;
                        case CheatOptions.Give_Vision_Torch:
                            Possession.pickUpVisionTorch();
                            break;
                        case CheatOptions.Give_Identify_Conversation_Stone:
                            Possession.pickUpConversationStone(NomaiWord.Identify);
                            break;
                        case CheatOptions.Give_Explain_Conversation_Stone:
                            Possession.pickUpConversationStone(NomaiWord.Explain);
                            break;
                        case CheatOptions.Give_Eye_Conversation_Stone:
                            Possession.pickUpConversationStone(NomaiWord.Eye);
                            break;
                        case CheatOptions.Give_Quantum_Moon_Conversation_Stone:
                            Possession.pickUpConversationStone(NomaiWord.QuantumMoon);
                            break;
                        case CheatOptions.Give_You_Conversation_Stone:
                            Possession.pickUpConversationStone(NomaiWord.You);
                            break;
                        case CheatOptions.Give_Me_Conversation_Stone:
                            Possession.pickUpConversationStone(NomaiWord.Me);
                            break;
                        case CheatOptions.Give_Nomai_Conversation_Stone:
                            Possession.pickUpConversationStone(NomaiWord.TheNomai);
                            break;
                        case CheatOptions.Give_Sun_Station_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.SunStation);
                            break;
                        case CheatOptions.Give_Time_Loop_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.HGT_TimeLoop);
                            break;
                        case CheatOptions.Give_Eye_Locator_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.THM_EyeLocator);
                            break;
                        case CheatOptions.Give_Mine_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.TH_Mine);
                            break;
                        case CheatOptions.Give_Observatory_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.BH_Observatory);
                            break;
                        case CheatOptions.Give_Gravity_Cannon_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.BH_GravityCannon);
                            break;
                        case CheatOptions.Give_Quantum_Fragment_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.BH_QuantumFragment);
                            break;
                        case CheatOptions.Give_Black_Hole_Forge_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.BH_BlackHoleForge);
                            break;
                        case CheatOptions.Give_Construction_Yard_1st_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.GD_ConstructionYardIsland1);
                            break;
                        case CheatOptions.Give_Construction_Yard_2nd_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.GD_ConstructionYardIsland2);
                            break;
                        case CheatOptions.Give_Statue_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.GD_StatueIsland);
                            break;
                        case CheatOptions.Give_Tracking_Module_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.GD_ProbeCannonSunkenModule);
                            break;
                        case CheatOptions.Give_Launch_Module_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.GD_ProbeCannonDamagedModule);
                            break;
                        case CheatOptions.Give_Control_Module_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.GD_ProbeCannonIntactModule);
                            break;
                        case CheatOptions.Give_Volcanic_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.VM_Interior);
                            break;
                        case CheatOptions.Give_High_Energy_Lab_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.HGT_TLE);
                            break;
                        case CheatOptions.Give_North_Pole_Projection_Stone:
                            Possession.pickUpShareStone(NomaiRemoteCameraPlatform.ID.BH_NorthPole);
                            break;
                        default:
                            ModHelper.Console.WriteLine("CheatsMod: Input not mapped " + input.Item1, MessageType.Warning);
                            break;
                    }
                }
            }

            SuperNova.Update();
            Player.Update();
            Ship.Update();
            Anglerfish.Update();
        }

        public static Sector getSector(Sector.Name name) => getSectors(name).FirstOrDefault();

        public static Sector getSector(Sector.Name name, Func<Sector, bool> predicate) => getSectors(name).FirstOrDefault(predicate);

        public static List<Sector> getSectors(Sector.Name name)
        {
            var sectors = new List<Sector>();
            foreach (Sector sector in SectorManager.GetRegisteredSectors())
            {
                if (name.Equals(sector.GetName()))
                {
                    sectors.Add(sector);
                }
            }
            return sectors;
        }

        private void toggleFacts()
        {
            if (Data.knowAllRumors && Data.knowAllFacts)
            {
                Data.knowAllRumors = false;
                Data.knowAllFacts = false;
            }
            else if (Data.knowAllRumors)
            {
                Data.knowAllFacts = true;
            }
            else
            {
                Data.knowAllRumors = true;
            }

            ModHelper.Console.WriteLine("Cheat Mods: All Rumors " + Data.knowAllRumors + " All Fact " + Data.knowAllFacts);
        }

        private void toggleFrequencies()
        {
            if (Data.knowAllFrequencies && Data.knowAllSignals)
            {
                Data.knowAllSignals = false;
                Data.knowAllFrequencies = false;
            }
            else if (Data.knowAllFrequencies)
            {
                Data.knowAllSignals = true;
            }
            else
            {
                Data.knowAllFrequencies = true;
            }

            ModHelper.Console.WriteLine("Cheat Mods: All Frequencies " + Data.knowAllFrequencies + " All Signals " + Data.knowAllSignals);
        }
    }

    internal static class Extensions
    {
        public static GameObject InstantiateInactive(this GameObject original)
        {
            if (!original.activeSelf)
            {
                return UnityEngine.Object.Instantiate(original);
            }

            original.SetActive(false);
            var copy = UnityEngine.Object.Instantiate(original);
            copy.name = original.name;
            original.SetActive(true);
            return copy;
        }

        public static GameObject InstantiateInactive(this GameObject original, Transform parent)
        {
            if (!original.activeSelf)
            {
                return UnityEngine.Object.Instantiate(original, parent);
            }

            original.SetActive(false);
            var copy = UnityEngine.Object.Instantiate(original, parent);
            copy.name = original.name;
            original.SetActive(true);
            return copy;
        }

        public static T InstantiateInactive<T>(this T original) where T : Component => InstantiateInactive(original.gameObject).GetComponent<T>();
        public static T InstantiateInactive<T>(this T original, Transform parent) where T : Component => InstantiateInactive(original.gameObject, parent).GetComponent<T>();

        public static GameObject Instantiate(this GameObject original)
        {
            var copy = UnityEngine.Object.Instantiate(original);
            copy.name = original.name;
            original.SetActive(true);
            return copy;
        }

        public static GameObject Instantiate(this GameObject original, Transform parent)
        {
            var copy = UnityEngine.Object.Instantiate(original, parent);
            copy.name = original.name;
            original.SetActive(true);
            return copy;
        }

        public static T Instantiate<T>(this T original) where T : Component => Instantiate(original.gameObject).GetComponent<T>();
        public static T Instantiate<T>(this T original, Transform parent) where T : Component => Instantiate(original.gameObject, parent).GetComponent<T>();

        public static void SetActive<T>(this T original, bool value) where T : Component => original.gameObject.SetActive(value);
    }
}
