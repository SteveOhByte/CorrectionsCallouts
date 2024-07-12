using System;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace Corrections_Callouts.Callouts
{
    [CalloutInfo("Escape", CalloutProbability.High)]

    public class Escape : Callout
    {
        private Ped suspect;

        private Blip suspectBlip;

        private Vector3 suspectSpawn;

        private LHandle pursuit;

        private readonly Random rnd = new Random();

        private string escapeType;

        private int counter;

        public override bool OnBeforeCalloutDisplayed()
        {
            int typeOfEscape = rnd.Next(1, 5);

            switch (typeOfEscape)
            {
                case 1:
                    escapeType = "FleeOnSight";
                    suspectSpawn = new Vector3(1426.7539029126224f, 2423.431376083757f, 57f);
                    break;
                case 2:
                    escapeType = "GiveUp";
                    suspectSpawn = new Vector3(2032.3654000000008f, 2772.4248278839896f, 50f);
                    break;
                case 3:
                    escapeType = "Suicide";
                    suspectSpawn = new Vector3(1420.6826097087387f, 2091.1289154565793f, 110f);
                    break;
                case 4:
                    escapeType = "Fight";
                    suspectSpawn = new Vector3(1256.7576932038844f, 2707.411222316451f, 5f);
                    break;
                case 5:
                    typeOfEscape = rnd.Next(1, 5);
                    break;
            }  

            ShowCalloutAreaBlipBeforeAccepting(suspectSpawn, 30f);
            AddMinimumDistanceCheck(20f, suspectSpawn);
            CalloutMessage = "Escaped Convict Nearby";
            CalloutPosition = suspectSpawn;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_SUSPECT_ON_THE_RUN_01 IN_OR_ON_POSITION", suspectSpawn);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            suspect = new Ped("u_m_y_prisoner_01", suspectSpawn, 180f)
            {
                IsPersistent = true,
                BlockPermanentEvents = true
            };

            suspectBlip = suspect.AttachBlip();
            suspectBlip.IsRouteEnabled = true;

            counter = 0;

            if (escapeType == "GiveUp")
            {
                suspect.Inventory.GiveNewWeapon("WEAPON_PISTOL", -1, true);
            }

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if(Game.LocalPlayer.Character.DistanceTo(suspect) < 20)
            {
                switch (escapeType)
                {
                    case "FleeOnSight":
                        suspect.Inventory.GiveNewWeapon("WEAPON_MICROSMG", -1, true);
                        suspect.Tasks.Wander();
                        suspect.Tasks.FireWeaponAt(Game.LocalPlayer.Character, 10, FiringPattern.FullAutomatic);

                        suspectBlip.IsRouteEnabled = false;
                        suspectBlip.Delete();

                        pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(pursuit, suspect);
                        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                        break;
                    case "GiveUp":
                        suspect.Inventory.GiveNewWeapon("WEAPON_MICROSMG", -1, true);
                        Game.DisplaySubtitle("SUSPECT: Okay okay! Don't shoot me I surrender! I should've know this wouldn't work...");
                        break;
                    case "Suicide":
                    {
                        suspect.Inventory.GiveNewWeapon("WEAPON_MICROSMG", -1, true);
                        Game.DisplaySubtitle("SUSPECT: Don't come any close! Back up or I'll kill myself!");

                        Game.DisplayHelp("Press 'Y' to talk to the suspect", false);
                        if (Game.IsKeyDown(Keys.Y))
                        {
                            counter += 1;
                        }
                        switch (counter)
                        {
                            case 1:
                                Game.DisplaySubtitle("YOU: You don't want to do this, just come quietly, maybe we can get your sentance reduced?");
                                break;
                            case 2:
                                Game.DisplaySubtitle("SUSPECT: I don't think so, there's nothing left for me now. I can't go back to jail.");
                                break;
                            case 3:
                                suspect.Tasks.FireWeaponAt(Game.LocalPlayer.Character, 10000, FiringPattern.FullAutomatic);
                                break;
                        }

                        break;
                    }
                    case "Fight":
                        suspect.Inventory.GiveNewWeapon("WEAPON_HAMMER", -1, true);

                        Game.DisplaySubtitle("SUSPECT: You should've stayed in bed pig!");

                        suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        break;
                }

                if (suspect.IsCuffed || suspect.IsDead || Game.LocalPlayer.Character.IsDead)
                {
                    End();
                }
            }

            if (suspect.IsCuffed || suspect.IsDead || Game.LocalPlayer.Character.IsDead)
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

            Game.LogTrivial("Corrections Callouts - Escape cleaned up");
        }

    }
}
