using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace Corrections_Callouts.Callouts
{

    [CalloutInfo("ContrabandExchange", CalloutProbability.High)]
    public class ContrabandExchange : Callout
    {
        private Ped seller;
        private Ped buyer;

        private Blip sellerBlip;
        private Blip buyerBlip;

        private Vector3 sellerSpawn;
        private Vector3 buyerSpawn;

        private LHandle pursuit;

        private int counter;

        public override bool OnBeforeCalloutDisplayed()
        {
            sellerSpawn = new Vector3(1633.177871844661f, 2530.231471777001f, 0f);
            buyerSpawn = new Vector3(1633.177871844661f, 2524.162020441345f, 0f);
            
            ShowCalloutAreaBlipBeforeAccepting(sellerSpawn, 30f);
            AddMinimumDistanceCheck(20f, sellerSpawn);
            CalloutMessage = "Contraband Exchange In Progress";
            CalloutPosition = sellerSpawn;
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION", sellerSpawn);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            seller = new Ped("s_m_y_prisoner_01", sellerSpawn, Game.LocalPlayer.Character.Heading);
            buyer = new Ped("u_m_y_prisoner_01", buyerSpawn, seller.Heading - 180f);

            sellerBlip = seller.AttachBlip();
            sellerBlip.IsFriendly = false;
            sellerBlip.IsRouteEnabled = true;
            buyerBlip = buyer.AttachBlip();
            buyerBlip.IsFriendly = false;

            counter = 0;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (false && Game.LocalPlayer.Character.DistanceTo(seller) < 20 || Game.LocalPlayer.Character.DistanceTo(buyer) < 20)
            {
                sellerBlip.IsRouteEnabled = false;

                Game.DisplayHelp("Press 'Y' to talk to the suspects", false);
                if(Game.IsKeyDown(Keys.Y))
                {
                    counter = counter + 1;

                    if(counter == 1)
                    {
                        Game.DisplaySubtitle("YOU: Alright, what's going on here?");
                    }

                    if(counter == 2)
                    {
                        Game.DisplaySubtitle("Suspect: It's the fuzz! Run!");
                        seller.Inventory.GiveNewWeapon("WEAPON_KNIFE", -1, true);

                        pursuit = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(pursuit, seller);
                        Functions.AddPedToPursuit(pursuit, buyer);
                        Functions.SetPursuitIsActiveForPlayer(pursuit, true);

                        sellerBlip.Delete();
                        buyerBlip.Delete();

                        if(buyer.IsDead || buyer.IsCuffed || Game.LocalPlayer.Character.IsDead && seller.IsCuffed || seller.IsDead || Game.LocalPlayer.Character.IsDead)
                        {
                            End();
                        }
                    }

                    if (buyer.IsDead || buyer.IsCuffed || Game.LocalPlayer.Character.IsDead && seller.IsCuffed || seller.IsDead || Game.LocalPlayer.Character.IsDead)
                    {
                        End();
                    }
                }

                if (buyer.IsDead || buyer.IsCuffed || Game.LocalPlayer.Character.IsDead && seller.IsCuffed || seller.IsDead || Game.LocalPlayer.Character.IsDead)
                {
                    End();
                }
            }

            if (buyer.IsDead || buyer.IsCuffed || Game.LocalPlayer.Character.IsDead && seller.IsCuffed || seller.IsDead || Game.LocalPlayer.Character.IsDead)
            {
                End();
            }

        }

        public override void End()
        {
            base.End();

            if(seller.Exists())
            {
                seller.Dismiss();
            }

            if(buyer.Exists())
            {
                buyer.Dismiss();
            }

            if(sellerBlip.Exists())
            {
                sellerBlip.Delete();
            }

            if(buyerBlip.Exists())
            {
                buyerBlip.Delete();
            }

            Game.LogTrivial("Corrections Callouts - Contraband Exchange Cleaned up.");
        }

    }
}
