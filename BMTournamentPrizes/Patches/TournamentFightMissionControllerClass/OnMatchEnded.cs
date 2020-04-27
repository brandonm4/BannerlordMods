using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TournamentsXPanded.Behaviors;
using TournamentsXPanded.Common.Patches;
using TournamentsXPanded.Models;
using XPanded.Common.Diagnostics;
using XPanded.Common.Extensions;

namespace TournamentsXPanded.Patches.TournamentFightMissionControllerClass
{
    public class OnMatchResultsReady : PatchBase<OnMatchResultsReady>
    {
        public override bool Applied { get; protected set; }

        private static readonly MethodInfo TargetMethodInfo = typeof(TournamentFightMissionController).GetMethod("OnMatchResultsReady", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        private static readonly MethodInfo PatchMethodInfo = typeof(OnMatchResultsReady).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override bool IsApplicable(Game game)
        {
            return true;
        }

        public override void Reset()
        {
        }

        public override void Apply(Game game)
        {
            if (Applied)
            {
                return;
            }

            TournamentsXPandedSubModule.Harmony.Patch(
                TargetMethodInfo,
                prefix: new HarmonyMethod(PatchMethodInfo));

            Applied = true;
        }
        private static bool Prefix(TournamentFightMissionController __instance)
        {
            if (TournamentXPandedTournamentBehavior._match.IsPlayerParticipating())
            {
                //InformationManager.AddQuickInformation(new TextObject("{=UBd0dEPp}Match is over", null), 0, null, "");
                var achievments = TournamentXPandedTournamentBehavior.achievements;
                var m = TournamentXPandedTournamentBehavior._match;
                var pc = m.Participants.Where(p => p.Character == CharacterObject.PlayerCharacter).FirstOrDefault();
                //Did we get first kill
                var message = "";
                float renown = 0;
                if (TournamentXPSettings.Instance.BonusRenownFirstKill > 0 && TournamentXPandedTournamentBehavior.firstKiller == pc )
                {
                    message += "{=tourn018}QUICK ACTION! You got the first kill: " + TournamentXPSettings.Instance.BonusRenownFirstKill.ToString() + " renown.\n";
                    InformationManager.AddQuickInformation(new TextObject(message));
                    renown += TournamentXPSettings.Instance.BonusRenownFirstKill;
                }

                if (TournamentXPSettings.Instance.BonusRenownMostKills > 0)
                {
                    var numberofkills = TournamentXPandedTournamentBehavior.MostKills();
                    if (numberofkills.Contains(pc))
                    {
                        var bonus = TournamentXPSettings.Instance.BonusRenownMostKills / numberofkills.Count;
                        if (bonus > 0)
                        {
                            message = "";
                            if (numberofkills.Count == 1)
                            {
                                message += "{=tourn019}AGGRESSIVE! You got the most kills: " + bonus.ToString() + " renown.\n";
                            }
                            else
                            {
                                message += "{=tourn020}You tied for the most kills: " + bonus.ToString() + " renown.\n";
                            }
                            InformationManager.AddQuickInformation(new TextObject(message));
                            renown += bonus;
                        }
                    }
                }
                if (TournamentXPSettings.Instance.BonusRenownMostDamage > 0)
                {
                    var mostdamage = TournamentXPandedTournamentBehavior.MostDamage();
                    if (mostdamage.Contains(pc))
                    {

                        var bonusmd = TournamentXPSettings.Instance.BonusRenownMostDamage / mostdamage.Count;
                        if (bonusmd > 0)
                        {
                            message = "";
                            if (mostdamage.Count == 1)
                            {
                                message += "{=tourn021}SAVAGE! You inflicted the most damage: " + bonusmd.ToString() + " renown.\n";
                            }
                            else
                            {
                                message += "{=tourn022}You tied for inflicting the most damage: " + bonusmd.ToString() + " renown.\n";
                            }
                            InformationManager.AddQuickInformation(new TextObject(message));
                            renown += bonusmd;
                        }
                    }
                }
                if (TournamentXPSettings.Instance.BonusRenownLeastDamage > 0)
                {
                    var leastDamage = TournamentXPandedTournamentBehavior.LeastDamage();
                    if (leastDamage.Contains(pc))
                    {
                        var bonusld = TournamentXPSettings.Instance.BonusRenownLeastDamage / leastDamage.Count;
                        if (bonusld > 0)
                        {
                            message = "";
                            if (leastDamage.Count == 1)
                            {
                                message += "{=tourn023}DEFENSIVE! You took the least damage: " + bonusld.ToString() + " renown.\n";
                            }
                            else
                            {
                                message += "{=tourn024}You tied for the least damage taken: " + bonusld.ToString() + " renown.\n";
                            }
                            InformationManager.AddQuickInformation(new TextObject(message));
                            renown += bonusld;
                        }
                    }
                }
                if (renown > 0)
                {
                    TournamentPrizePoolBehavior.TournamentReward.BonusRenown += renown;
                }

            }
            return true;
        }
    }
}


