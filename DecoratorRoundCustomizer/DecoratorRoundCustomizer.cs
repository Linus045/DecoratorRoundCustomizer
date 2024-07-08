using HarmonyLib;
using Kitchen;
using Kitchen.Modules;
using KitchenData;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Logging;
using KitchenLib.Preferences;
using KitchenLib.Utils;
using KitchenMods;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DecoratorRoundCustomizer
{
    public class DecoratorRoundCustomizer : BaseMod
    {
        internal const string MOD_ID = "decorator_round_customizer_mod";
        internal const string MOD_NAME = "Decorator Round Customizer";
        internal const string MOD_AUTHOR = "Linus045 and Sim3xx";
        internal const string MOD_VERSION = "0.0.1";
        internal const string MOD_BETA_VERSION = null;
        internal const string MOD_COMPATIBLE_VERSIONS = ">=1.1.7";

        new public static DecoratorRoundCustomizer instance;
        internal static KitchenLogger Logger;
        internal static PreferenceManager manager;

        internal const string PREFERRENCE_CUSTOMIZER_ROUND_ENABLED = "Customizer Round Enabled";

        public DecoratorRoundCustomizer() : base(MOD_ID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_BETA_VERSION, MOD_COMPATIBLE_VERSIONS, Assembly.GetExecutingAssembly())
        {
            if (instance == null)
                instance = this;
            else
                Logger.LogError("REASSIGNED SINGLETON IN DecoratorRoundCustomizer::DecoratorRoundCustomizer()");
        }

        protected override void OnInitialise()
        {
            Logger.LogInfo("DecoratorRoundCustomizer Initialized");
        }

        protected override void OnPostActivate(Mod mod)
        {
            Logger = InitLogger();
            manager = new PreferenceManager(MOD_ID);
            manager.RegisterPreference(new PreferenceBool(PREFERRENCE_CUSTOMIZER_ROUND_ENABLED, true));
            manager.Load();
            manager.Save();

            Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(DecoratorRoundCustomizerMenu<MainMenuAction>), new DecoratorRoundCustomizerMainMenu(args.Container, args.Module_list));
            };
            ModsPreferencesMenu<MainMenuAction>.RegisterMenu("Decorator Round Customizer", typeof(DecoratorRoundCustomizerMenu<MainMenuAction>), typeof(MainMenuAction));

            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(DecoratorRoundCustomizerMenu<PauseMenuAction>), new DecoratorRoundCustomizerPauseMenu(args.Container, args.Module_list));
            };

            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu("Decorator Round Customizer", typeof(DecoratorRoundCustomizerMenu<PauseMenuAction>), typeof(PauseMenuAction));
        }

        public enum DecoratorRoundCustomizerMenuAction
        {
            DoStuff
        }

        public class DecoratorRoundCustomizerPauseMenu : DecoratorRoundCustomizerMenu<PauseMenuAction>
        {
            public DecoratorRoundCustomizerPauseMenu(Transform container, ModuleList module_list) : base(container, module_list)
            { }
            public override void Setup(int player_id)
            {
                CreateMenu(player_id);
                AddActionButton("Back", PauseMenuAction.Back, ElementStyle.MainMenuBack);
            }
        }

        public class DecoratorRoundCustomizerMainMenu : DecoratorRoundCustomizerMenu<MainMenuAction>
        {
            public DecoratorRoundCustomizerMainMenu(Transform container, ModuleList module_list) : base(container, module_list)
            {
            }

            public override void Setup(int player_id)
            {
                CreateMenu(player_id);
                AddActionButton("Back", MainMenuAction.Back, ElementStyle.MainMenuBack);
            }
        }

        public abstract class DecoratorRoundCustomizerMenu<T> : KLMenu<T>
        {
            Option<bool> customizerRoundEnabledOption;

            public DecoratorRoundCustomizerMenu(Transform container, ModuleList module_list) : base(container, module_list)
            {
                customizerRoundEnabledOption = Add(new Option<bool>(new List<bool> { false, true }, manager.GetPreference<PreferenceBool>(PREFERRENCE_CUSTOMIZER_ROUND_ENABLED).Get(), new List<string> {
                    Localisation["SETTING_DISABLED"],
                    Localisation["SETTING_ENABLED"]
                }));
            }

            public void CreateMenu(int player_id)
            {
                PreferenceBool customizerRoundEnabled = manager.GetPreference<PreferenceBool>(PREFERRENCE_CUSTOMIZER_ROUND_ENABLED);
                AddLabel("Customizer Round Enabled:");
                AddSelect(customizerRoundEnabledOption);
                customizerRoundEnabledOption.OnChanged += delegate (object _, bool f)
                {
                    customizerRoundEnabled.Set(f);
                };
                AddInfo("This enables/disables the customizer round.");

                AddLabel("Do stuff");
                AddButton("Do Stuff", (a) =>
                {
                    Logger.LogInfo("Congratulations you did stuff!");
                });
                AddInfo("This does stuff.");
            }
        }
    }
}
