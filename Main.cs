using System;
using System.Linq;
using System.Reflection;
using Corrections_Callouts.Callouts;
using LSPD_First_Response.Mod.API;
using Rage;

namespace Corrections_Callouts
{
    public class Main: Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Plugin Corrections Callouts " + Assembly.GetExecutingAssembly().GetName().Version + " by SneakySteve has been initialised.");
            Game.LogTrivial("Go on duty to fully load Corrections Callouts");

            AppDomain.CurrentDomain.AssemblyResolve += LspdfrResolveEventHandler;
        }

        public override void Finally()
        {
            throw new NotImplementedException();
        }

        private static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (!onDuty) return;
            
            RegisterCallouts();
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "SteveOhByte Corrections Callouts", "~g~Version 1.1.1 ~r~Alpha~w~", "Corrections Callouts Plugin ~g~successfully~w~ loaded!");
        }

        private static void RegisterCallouts()
        {
            Functions.RegisterCallout(typeof(ContrabandExchange));
            Functions.RegisterCallout(typeof(Stabbing));
            Functions.RegisterCallout(typeof(BodyFound));
            Functions.RegisterCallout(typeof(Escape));
            Functions.RegisterCallout(typeof(GuardTakenHostage));
            Functions.RegisterCallout(typeof(PrisonRiot));
        }

        private static Assembly LspdfrResolveEventHandler(object sender, ResolveEventArgs args)
        {
            return Functions.GetAllUserPlugins().FirstOrDefault(assembly => args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()));
        }

        public static bool IsLspdfrPluginRunning(string plugin, Version minversion = null)
        {
            return Functions.GetAllUserPlugins().Select(assembly => assembly.GetName())
                .Where(an => string.Equals(an.Name, plugin, StringComparison.CurrentCultureIgnoreCase))
                .Any(an => minversion == null || an.Version.CompareTo(minversion) >= 0);
        }

    }
}
