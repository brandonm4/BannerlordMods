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
                if (TournamentXPandedTournamentBehavior.firstKiller == pc)
                {
                    message += "You got the first kill: " + TournamentXPSettings.Instance.BonusRenownFirstKill.ToString() + " renown.\n";
                    renown += TournamentXPSettings.Instance.BonusRenownFirstKill;
                }
                var numberofkills = TournamentXPandedTournamentBehavior.MostKills();
                if (numberofkills.Contains(pc))
                {
                    var bonus = TournamentXPSettings.Instance.BonusRenownMostKills / numberofkills.Count;
                    if (bonus > 0)
                    {
                        if (numberofkills.Count == 1)
                        {
                            message += "You got the most kills: " + bonus.ToString() + " renown.\n";
                        }
                        else
                        {
                            message += "AGGRESSIVE! You tied for the most kills: " + bonus.ToString() + " renown.\n";
                        }
                        renown += bonus;
                    }
                }
                var mostdamage = TournamentXPandedTournamentBehavior.MostDamage();
                if (mostdamage.Contains(pc))
                {
                    var bonusmd = TournamentXPSettings.Instance.BonusRenownMostDamage / mostdamage.Count;
                    if (bonusmd > 0)
                    {
                        if (mostdamage.Count == 1)
                        {
                            message += "SAVAGE! You inflicted the most damage: " + bonusmd.ToString() + " renown.\n";
                        }
                        else
                        {
                            message += "You tied for the most damage: " + bonusmd.ToString() + " renown.\n";
                        }
                        renown += bonusmd;
                    }
                }
                var leastDamage = TournamentXPandedTournamentBehavior.LeastDamage();
                if (leastDamage.Contains(pc))
                {
                    var bonusld = TournamentXPSettings.Instance.BonusRenownLeastDamage / leastDamage.Count;
                    if (bonusld > 0)
                    {
                        if (leastDamage.Count == 1)
                        {
                            message += "DEFENSIVE! You took the least damage: " + bonusld.ToString() + " renown.\n";
                        }
                        else
                        {
                            message += "You tied for the least damage: " + bonusld.ToString() + " renown.\n";
                        }
                        renown += bonusld;
                    }
                }
                if (renown > 0)
                {
                    InformationManager.AddQuickInformation(new TextObject(message));
                    GainRenownAction.Apply(Hero.MainHero, renown, false);
                }

            }
            return true;
        }
    }
}


