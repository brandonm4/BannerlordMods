using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace BMSaveFixer
{
    public class BMSaveFixerMain : MBSubModuleBase
    {
        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

          

            Campaign campaign = ((Campaign)game.GameType);
            if (campaign != null)
            {

                CorruptedCharFix(campaign);

            }
        }
        private void CorruptedCharFix(Campaign campaign)
        {
            {
                foreach (var c in campaign.Characters)
                {
                    if (c.IsHero && c.HeroObject != null)
                    {
                        if (c.HeroObject.IsWanderer && c.HeroObject.CompanionOf != Hero.MainHero.Clan)
                        {
                            //Set non-companions that are wanderers back to stock
                            //The problems chars have IsArcher, IsInfantry and IsMounted as Exception - not null, true or false.  Basically just trying to access to force an exception, then murdering the char.
                            var bHadIssue = false;
                            try
                            {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
#pragma warning disable RCS1166 // Value type object is never equal to null.
                                if (c.IsArcher == null)
#pragma warning restore RCS1166 // Value type object is never equal to null.
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                                {
                                    //   typeof(CharacterObject).GetProperty("IsArcher").SetValue(c, false);
                                }
                            }
                            catch
                            {
                                //      typeof(CharacterObject).GetProperty("IsArcher").SetValue(c, false);
                                bHadIssue = true;
                            }
                            try
                            {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
#pragma warning disable RCS1166 // Value type object is never equal to null.
                                if (c.IsMounted == null)
#pragma warning restore RCS1166 // Value type object is never equal to null.
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                                {
                                    //    typeof(CharacterObject).GetProperty("IsMounted").SetValue(c, false);
                                }
                            }
                            catch
                            {
                                //   typeof(CharacterObject).GetProperty("IsMounted").SetValue(c, false);
                                bHadIssue = true;
                            }
                            try
                            {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
#pragma warning disable RCS1166 // Value type object is never equal to null.
                                if (c.IsInfantry == null)
#pragma warning restore RCS1166 // Value type object is never equal to null.
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                                {
                                    //     typeof(CharacterObject).GetProperty("IsInfantry").SetValue(c, false);
                                }
                            }
                            catch
                            {
                                //   typeof(CharacterObject).GetProperty("IsInfantry").SetValue(c, false);
                                bHadIssue = true;
                            }

                            if (bHadIssue)
                            {
                                //var urban = ((TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.UrbanCharactersCampaignBehavior)campaign.GetCampaignBehavior<TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.UrbanCharactersCampaignBehavior>());
                                //List<CharacterObject> _companionTemplates = (List<CharacterObject>)Traverse.Create(urban).Field("_companionTemplates").GetValue();
                                //var civEquip = (CharacterObject)Traverse.Create(c).Field("_civilianEquipmentTemplate").GetValue();
                                // var batEquip = (CharacterObject)Traverse.Create(c).Field("_battleEquipmentTemplate").GetValue();
                                //var _template = _companionTemplates.Where(x => x.Name == c.Name).FirstOrDefault();

                                //Traverse.Create(c).Field("_heroObject").SetValue(HeroCreator.CreateSpecialHero(c.HeroObject.Template));
                                var equipment = MBObjectManager.Instance.GetObject<CharacterObject>("tournament_template_battania_four_participant_set_v1").BattleEquipments.ToList();
                                c.InitializeEquipmentsOnLoad(equipment);
                                typeof(Hero).GetProperty("BattleEquipment").SetValue(c.HeroObject, equipment[0]);
                                //   Traverse.Create(c.HeroObject).Method("SetInitialValuesFromCharacter").GetValue(new object[] { c });
                                //Murder the char
                                //c.HeroObject.IsDead = true;
                                //c.HeroObject.AlwaysDie = true;

                                //    c.HeroObject.ChangeState(Hero.CharacterStates.Dead);
                                //KillCharacterAction.ApplyByRemove(c.HeroObject, true);
                                //         if (c.HeroObject.CurrentSettlement != null)
                                //         {
                                //            // Traverse.Create(c.HeroObject.CurrentSettlement).Method("RemoveHero").GetValue(new object[] { c.HeroObject });
                                //         }

                                ////         ApplyInternal(c.HeroObject, null, KillCharacterAction.KillCharacterActionDetail.Lost, true);

                                //         if (c.HeroObject.PartyBelongedTo != null)
                                //         {

                                //         }
                            }

                        }
                    }
                }
            }
        }
        private static void ApplyInternal(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail actionDetail, bool showNotification)
        {
            if (!victim.IsAlive)
            {
                return;
            }
            //victim.EncyclopediaText = KillCharacterAction.CreateObituary(victim, actionDetail);
            victim.EncyclopediaText = (TextObject)Traverse.Create(typeof(KillCharacterAction)).Method("CreateObituary").GetValue(new object[] { victim, actionDetail });
            //KillCharacterAction.MakeDead(victim, true);
            Traverse.Create(typeof(KillCharacterAction)).Method("MakeDead").GetValue(new object[] { victim, true });
            victim.MakeWounded(actionDetail);
        }
    }
}
