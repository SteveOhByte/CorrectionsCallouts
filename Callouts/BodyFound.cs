using System;
using System.Windows.Forms;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace Corrections_Callouts.Callouts
{
    [CalloutInfo("BodyFound", CalloutProbability.High)]
    public class BodyFound : Callout
    {
        public Persona PedPersona;

        private string suspectForename;
        private string suspectSurname;

        private Ped victim;
        private Ped officer;
        private Ped witness1;
        private Ped suspect;

        private Blip officerBlip;
        private Blip witness1Blip;
        private Blip suspectBlip;

        private Vector3 victimSpawn;
        private Vector3 officerSpawn;
        private Vector3 witness1Spawn;
        private Vector3 suspectSpawn;

        private int counter;

        private bool pursuitStarted;

        public override bool OnBeforeCalloutDisplayed()
        {
            victimSpawn = new Vector3(1663.5343378640785f, 2522.0599602881703f, 0f);
            officerSpawn = new Vector3(1663.5343378640785f, 2525.0946859559986f, 0f);
            witness1Spawn = new Vector3(1660.4986912621368f, 2525.0946859559986f, 0f);
            suspectSpawn = new Vector3(1672.6412776699037f, 2535.716225793397f, 0f);

            ShowCalloutAreaBlipBeforeAccepting(victimSpawn, 30f);
            AddMinimumDistanceCheck(20f, victimSpawn);
            CalloutMessage = "Body Found";
            CalloutPosition = victimSpawn;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_AMBULANCE_REQUESTED_03 IN_OR_ON_POSITION", victimSpawn);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            victim = new Ped("s_m_y_prisoner_01", victimSpawn, 90f);
            officer = new Ped("s_m_m_prisguard_01", officerSpawn, victim.Heading - 180f);
            witness1 = new Ped("u_m_y_prisoner_01", witness1Spawn, 180f);
            suspect = new Ped("s_m_y_prisoner_01", suspectSpawn, 90f);

            victim.IsPersistent = true;
            victim.BlockPermanentEvents = true;
            victim.Kill();

            witness1.IsPersistent = true;
            witness1.BlockPermanentEvents = true;

            officerBlip = officer.AttachBlip();
            officerBlip.IsFriendly = true;
            officerBlip.IsRouteEnabled = true;

            witness1Blip = witness1.AttachBlip();
            witness1Blip.IsFriendly = true;

            Persona pedPersona = Persona.FromExistingPed(suspect);
            pedPersona.Wanted = true;
            Functions.SetPersonaForPed(suspect, pedPersona);
            suspectForename = Convert.ToString(pedPersona.Forename);
            suspectSurname = Convert.ToString(pedPersona.Surname);

            pursuitStarted = false;

            counter = 0;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (pursuitStarted == false && Game.LocalPlayer.Character.DistanceTo(officer) < 20 || Game.LocalPlayer.Character.DistanceTo(victim) < 20)
            {
                Game.DisplayHelp("Press 'Y' to talk to the guard");
                if (Game.IsKeyDown(Keys.Y))
                {
                    counter = counter + 1;

                    if (counter == 1)
                    {
                        Game.DisplaySubtitle("GUARD: Thanks for the quick response. This person was discoverd about 15 minutes ago and we're just waiting on CSI. I'll stay here while you go interview witnesses, one showed up over there.");
                    }

                    if (counter == 2)
                    {
                        Game.DisplaySubtitle("YOU: What exactly did you see happen?");
                    }

                    if (counter == 3)
                    {
                        Game.DisplaySubtitle("WITNESS: I saw it all. It was " + suspectForename + " " + suspectSurname + ". You can usually find him over there.");
                    }

                    if (counter >= 4)
                    {
                        Game.DisplaySubtitle("The person is no longer saying anything.");

                        suspectBlip = suspect.AttachBlip();
                        suspectBlip.IsFriendly = false;
                        suspect.Inventory.GiveNewWeapon("WEAPON_KNIFE", -1, false);
                        suspect.Tasks.StandStill(800);

                        if (Game.LocalPlayer.Character.DistanceTo(suspect) < 10)
                        {
                            suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);

                            if (suspect.IsDead || suspect.IsCuffed || Game.LocalPlayer.Character.IsDead)
                            {
                                End();
                            }
                        }

                        if (suspect.IsDead || suspect.IsCuffed || Game.LocalPlayer.Character.IsDead)
                        {
                            End();
                        }
                    }

                    if (suspect.IsDead || suspect.IsCuffed || Game.LocalPlayer.Character.IsDead)
                    {
                        End();
                    }

                }

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

            if (officer.Exists())
            {
                officer.Dismiss();
            }

            if (officerBlip.Exists())
            {
                officerBlip.Delete();
            }

            if(witness1.Exists())
            {
                witness1.Dismiss();
            }

            if(witness1Blip.Exists())
            {
                witness1Blip.Delete();
            }

            if(victim.Exists())
            {
                victim.Dismiss();
            }

            Game.LogTrivial("Corrections Callouts - Body Found Cleaned up.");
        }

    }
}
