using System;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace Corrections_Callouts.Callouts
{
    [CalloutInfo("GuardTakenHostage", CalloutProbability.High)]
    public class GuardTakenHostage : Callout
    {

        private Ped suspect;
        private Ped guard;

        private Blip suspectBlip;

        private Vector3 suspectSpawn;
        private Vector3 guardSpawn;

        private int counter;

        private readonly Random rnd = new Random();

        public int Random;

        private string typeofcall;

        private Timer timer1;
        private int count = 60;

        public override bool OnBeforeCalloutDisplayed()
        {
            suspectSpawn = new Vector3(1724.2472699029136f, 2528.362407914593f, 0f);
            guardSpawn = new Vector3(1724.2472699029136f, 2526.362407914593f, 0f);

            ShowCalloutAreaBlipBeforeAccepting(suspectSpawn, 30f);
            AddMinimumDistanceCheck(20f, suspectSpawn);
            CalloutMessage = "Guard Taken Hostage";
            CalloutPosition = suspectSpawn;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_THREATEN_OFFICER_WITH_FIREARM_01 IN_OR_ON_POSITION", suspectSpawn);

            int random1 = rnd.Next(1, 3);

            switch (random1)
            {
                case 1:
                    typeofcall = "PlayerFirst";
                    break;
                case 2:
                    typeofcall = "GuardFirst";
                    break;
                case 3:
                    random1 = rnd.Next(1, 3);
                    break;
            }

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            suspect = new Ped("s_m_y_prisoner_01", suspectSpawn, 180f);
            guard = new Ped("s_m_m_prisguard_01", guardSpawn, 90f);

            suspectBlip = suspect.AttachBlip();
            suspectBlip.IsRouteEnabled = true;

            suspect.IsPersistent = true;
            suspect.BlockPermanentEvents = true;
            guard.IsPersistent = true;
            guard.BlockPermanentEvents = true;

            suspect.Inventory.GiveNewWeapon("WEAPON_PISTOL", -1, true);

            counter = 0;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            suspect.Tasks.AimWeaponAt(guard, -1);
            guard.Tasks.PutHandsUp(999999, suspect);
            if (typeofcall == "PlayerFirst")
            {
                if (Game.LocalPlayer.Character.DistanceTo(suspect) < 20)
                {

                    Game.DisplaySubtitle("SUSPECT: STAY BACK! I'll Kill him!");
                    Game.DisplayHelp("Press 'Y' to talk to the suspect", false);

                    if (Game.IsKeyDown(Keys.Y))
                    {
                        counter += 1;
                    }
                    switch (counter)
                    {
                        case 1:
                            Game.DisplaySubtitle("YOU: Calm down sir, you don't need to hurt anyone, just put the weapon down.");
                            break;
                        case 2:
                            Game.DisplaySubtitle("SUSPECT: Shutup! I'm the one giving the demands here!");
                            break;
                        case 3:
                            Game.DisplaySubtitle("YOU: Okay sir, what are your demands?");
                            break;
                        case 4:
                            Game.DisplaySubtitle("SUSPECT: I want outta here. NOW! And I want my record completley cleared.");
                            break;
                        case 5:
                            Game.DisplaySubtitle("YOU: C'mon sir, you know I can't do that.");
                            break;
                        case 6:
                            Game.DisplaySubtitle("SUSPECT: Well you had better find a way, and fast! I'm running out of patience here...");
                            timer1 = new Timer();
                            timer1.Tick += timer1_Tick;
                            timer1.Interval = 30000; // 30 seconds
                            timer1.Start();
                            break;
                    }

                    if (Game.LocalPlayer.Character.IsDead || suspect.IsDead || suspect.IsCuffed)
                    {
                        End();
                    }

                }
            }

            if (typeofcall != "GuardFirst") return;
            
            if (!(Game.LocalPlayer.Character.DistanceTo(suspect) < 20)) return;
                
            Game.DisplaySubtitle("SUSPECT: STAY BACK! I'll Kill him!");
            Game.DisplayHelp("Press 'Y' to talk to the suspect", false);

            if (Game.IsKeyDown(Keys.Y))
            {
                counter += 1;
            }
            switch (counter)
            {
                case 1:
                    Game.DisplaySubtitle("YOU: Calm down sir, you don't need to hurt anyone, just put the weapon down.");
                    break;
                case 2:
                    Game.DisplaySubtitle("SUSPECT: Shutup! I'm the one giving the demands here!");
                    break;
                case 3:
                    Game.DisplaySubtitle("YOU: Okay sir, what are your demands?");
                    break;
                case 4:
                    Game.DisplaySubtitle("SUSPECT: I want outta here. NOW! And I want my record completley cleared.");
                    break;
                case 5:
                    Game.DisplaySubtitle("YOU: C'mon sir, you know I can't do that.");
                    break;
                case 6:
                    Game.DisplaySubtitle("SUSPECT: Well I guess you'll just have to get the coroner down here, DIE PIG!!");
                    suspect.Tasks.FireWeaponAt(Game.LocalPlayer.Character, 100, FiringPattern.FullAutomatic).WaitForCompletion();
                    suspect.Tasks.FireWeaponAt(guard, 100, FiringPattern.FullAutomatic).WaitForCompletion();
                    break;
            }

            if (Game.LocalPlayer.Character.IsDead || suspect.IsDead || suspect.IsCuffed)
            {
                End();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            count--;
            if (count != 0) return;
            
            timer1.Stop();
            suspect.Tasks.FireWeaponAt(guard, 500, FiringPattern.FullAutomatic);
            suspect.Tasks.TakeCoverFrom(Game.LocalPlayer.Character, 10000);
        }


        public override void End()
        {
            base.End();

            if (suspect.Exists())
            {
                suspect.Dismiss();
            }
            if (guard.Exists())
            {
                guard.Dismiss();
            }
            if (suspectBlip.Exists())
            {
                suspectBlip.Delete();
            }

            Game.LogTrivial("Corrections Callouts - Guard Taken Hostage cleaned up");
        }
    }
}
