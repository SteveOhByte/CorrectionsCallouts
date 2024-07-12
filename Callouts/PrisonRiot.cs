using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace Corrections_Callouts.Callouts
{

    [CalloutInfo("PrisonRiot", CalloutProbability.High)]
    public class PrisonRiot : Callout
    {
        private static readonly Vector3 spawn1 = new Vector3(2, 2, 2);
        private static readonly Vector3 spawn2 = new Vector3(2, 2, 2);
        private static readonly Vector3 spawn3 = new Vector3(2, 2, 2);
        private static readonly Vector3 spawn4 = new Vector3(2, 2, 2);
        private static readonly Vector3 spawn5 = new Vector3(2, 2, 2);
        private static readonly Vector3 spawn6 = new Vector3(2, 2, 2);
        private static readonly Vector3 spawn7 = new Vector3(2, 2, 2);
        private static readonly Vector3 spawn8 = new Vector3(2, 2, 2);
        private static readonly Vector3 spawn9 = new Vector3(2, 2, 2);
        private static readonly Vector3 spawn10 = new Vector3(2, 2, 2);

        private Ped prisoner1 = new Ped("S_M_Y_PRISONER_01", spawn1, 90f);
        private Ped prisoner2 = new Ped("S_M_Y_PRISONER_01", spawn2, 90f);
        private Ped prisoner3 = new Ped("S_M_Y_PRISONER_01", spawn3, 90f);
        private Ped prisoner4 = new Ped("U_M_Y_PRISONER_01", spawn4, 90f);
        private Ped prisoner5 = new Ped("U_M_Y_PRISONER_01", spawn5, 90f);
        private Ped prisoner6 = new Ped("U_M_Y_PRISONER_01", spawn6, 90f);
        private Ped prisoner7 = new Ped("S_M_Y_PRISMUSCL_01", spawn7, 90f);
        private Ped prisoner8 = new Ped("S_M_Y_PRISMUSCL_01", spawn8, 90f);
        private Ped prisoner9 = new Ped("S_M_Y_PRISMUSCL_01", spawn9, 90f);
        private Ped prisoner10 = new Ped("S_M_Y_PRISONER_01", spawn10, 90f);

        private Blip blip1;
        private Blip blip2;
        private Blip blip3;
        private Blip blip4;
        private Blip blip5;
        private Blip blip6;
        private Blip blip7;
        private Blip blip8;
        private Blip blip9;
        private Blip blip10;

        public bool Stopped1 = false;
        public bool Stopped2 = false;
        public bool Stopped3 = false;
        public bool Stopped4 = false;
        public bool Stopped5 = false;
        public bool Stopped6 = false;
        public bool Stopped7 = false;
        public bool Stopped8 = false;
        public bool Stopped9 = false;
        public bool Stopped10 = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            ShowCalloutAreaBlipBeforeAccepting(spawn1, 30f);
            AddMinimumDistanceCheck(40f, spawn1);
            CalloutMessage = "Prison Riot in Progress";
            CalloutPosition = spawn1;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_SUSPECT_ON_THE_RUN_01 IN_OR_ON_POSITION", spawn1);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            prisoner1 = new Ped("S_M_Y_PRISONER_01", spawn1, 90f);
            prisoner2 = new Ped("S_M_Y_PRISONER_01", spawn2, 90f);
            prisoner3 = new Ped("S_M_Y_PRISONER_01", spawn3, 90f);
            prisoner4 = new Ped("U_M_Y_PRISONER_01", spawn4, 90f);
            prisoner5 = new Ped("U_M_Y_PRISONER_01", spawn5, 90f);
            prisoner6 = new Ped("U_M_Y_PRISONER_01", spawn6, 90f);
            prisoner7 = new Ped("S_M_Y_PRISMUSCL_01", spawn7, 90f);
            prisoner8 = new Ped("S_M_Y_PRISMUSCL_01", spawn8, 90f);
            prisoner9 = new Ped("S_M_Y_PRISMUSCL_01", spawn9, 90f);
            prisoner10 = new Ped("S_M_Y_PRISONER_01", spawn10, 90f);

            blip1 = prisoner1.AttachBlip();
            blip2 = prisoner2.AttachBlip();
            blip3 = prisoner3.AttachBlip();
            blip4 = prisoner4.AttachBlip();
            blip5 = prisoner5.AttachBlip();
            blip6 = prisoner6.AttachBlip();
            blip7 = prisoner7.AttachBlip();
            blip8 = prisoner8.AttachBlip();
            blip9 = prisoner9.AttachBlip();
            blip10 = prisoner10.AttachBlip();

            blip1.Scale = 1;
            blip2.Scale = 1;
            blip3.Scale = 1;
            blip4.Scale = 1;
            blip5.Scale = 1;
            blip6.Scale = 1;
            blip7.Scale = 1;
            blip8.Scale = 1;
            blip9.Scale = 1;
            blip10.Scale = 1;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(spawn1) < 20)
            {
                Functions.PlayScannerAudio("PRISON_ALARMS");
            }
        }

        public override void End()
        {
            base.End();


        }

    }
}
