using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace Corrections_Callouts.Callouts
{

    [CalloutInfo("Stabbing", CalloutProbability.High)]

    public class Stabbing : Callout
    {

        private Ped suspect;
        private Ped victim;

        private Blip suspectBlip;
        private Blip victimBlip;

        private Vector3 suspectSpawn;
        private Vector3 victimSpawn;

        public LHandle Pursuit;

        public bool PursuitStarted;

        public override bool OnBeforeCalloutDisplayed()
        {
            suspectSpawn = new Vector3(1657.463044660195f, 2543.8877372822276f, 0f);
            victimSpawn = new Vector3(1651.3917514563116f, 2543.8877372822276f, 0f);

            ShowCalloutAreaBlipBeforeAccepting(suspectSpawn, 30f);
            AddMinimumDistanceCheck(20f, suspectSpawn);
            CalloutMessage = "Stabbing In Progress";
            CalloutPosition = suspectSpawn;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_ASSAULT_WITH_A_DEADLY_WEAPON_01 IN_OR_ON_POSITION", suspectSpawn);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            suspect = new Ped("u_m_y_prisoner_01", suspectSpawn, Game.LocalPlayer.Character.Heading);
            victim = new Ped("s_m_y_prisoner_01", victimSpawn, suspect.Heading - 180f);

            suspectBlip = suspect.AttachBlip();
            suspectBlip.IsFriendly = false;
            suspectBlip.IsRouteEnabled = true;
            victimBlip = victim.AttachBlip();
            victimBlip.IsFriendly = true;

            PursuitStarted = false;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (PursuitStarted = false && Game.LocalPlayer.Character.DistanceTo(suspect) < 30 || Game.LocalPlayer.Character.DistanceTo(victim) < 30)
            {
                suspectBlip.IsRouteEnabled = false;

                suspect.Inventory.GiveNewWeapon("WEAPON_KNIFE", -1, true);
                suspect.Tasks.FightAgainst(victim);

            }    

            if (suspect.IsDead || suspect.IsCuffed || Game.LocalPlayer.Character.IsDead)
            {
                End();
            }

        }

        public override void End()
        {
            base.End();

            if (suspect.Exists())
            {
                suspect.Dismiss();
            }

            if (suspectBlip.Exists())
            {
                suspectBlip.Delete();
            }

            if(victim.Exists())
            {
                victim.Dismiss();
            }

            if(victimBlip.Exists())
            {
                victimBlip.Delete();
            }

            Game.LogTrivial("Corrections Callouts - Stabbing Cleaned up.");
        }

    }
}
