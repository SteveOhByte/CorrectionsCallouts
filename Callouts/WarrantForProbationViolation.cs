using System;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace Corrections_Callouts.Callouts
{

    [CalloutInfo("WarrantForProbationViolation", CalloutProbability.High)]
    public class WarrantForProbationViolation : Callout
    {

        private Blip suspectBlip;

        private Ped suspect;

        private Vector3 suspectSpawn;

        private readonly Random rnd = new Random();

        public override bool OnBeforeCalloutDisplayed()
        {

            int callLocation = rnd.Next(1, 5);

            switch (callLocation)
            {
                case 1:
                    suspectSpawn = new Vector3(2361.7330563106843f, 3229.0517543300334f, 0f);
                    break;
                case 2:
                    suspectSpawn = new Vector3(914.8521921723118f, 3596.253560137234f, 0f);
                    break;
                case 3:
                    suspectSpawn = new Vector3(2458.4784892596886f, 5026.600151037692f, 0f);
                    break;
                case 4:
                    suspectSpawn = new Vector3(902.7096057645447f, 2192.1663772862403f, 0f);
                    break;
                case 5:
                    callLocation = rnd.Next(1, 5);
                    break;
            }

            ShowCalloutAreaBlipBeforeAccepting(suspectSpawn, 30f);
            AddMinimumDistanceCheck(20f, suspectSpawn);
            CalloutMessage = "Warrant for Parole Violation";
            CalloutPosition = suspectSpawn;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_DISTURBING_THE_PEACE_02 IN_OR_ON_POSITION", suspectSpawn);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            suspect = new Ped(suspectSpawn, 90f);

            suspectBlip = suspect.AttachBlip();
            suspectBlip.IsRouteEnabled = true;

            suspect.Inventory.GiveNewWeapon("WEAPON_PISTOL", -1, false);
            suspect.Tasks.Wander();

            Persona pedPersona = Persona.FromExistingPed(suspect);
            pedPersona.Wanted = true;
            Functions.SetPersonaForPed(suspect, pedPersona);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.DistanceTo(suspect) < 20)
            {
                suspectBlip.IsRouteEnabled = false;
                suspectBlip.Delete();
            }
            if (suspect.IsCuffed || suspect.IsDead || Game.LocalPlayer.Character.IsDead)
            {
                End();
            }
        }

        public override void End()
        {
            base.End();

            if (suspectBlip.Exists())
            {
                suspectBlip.Delete();
            }
            if (suspect.Exists())
            {
                suspect.Dismiss();
            }

            Game.LogTrivial("Corrections Callouts - Warrant For Probation Violation cleaned up");
        }

    }
}
