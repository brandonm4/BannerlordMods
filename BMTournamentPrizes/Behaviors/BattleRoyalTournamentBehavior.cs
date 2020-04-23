using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace TournamentsXPanded.Behaviors
{
    public class BattleRoyalTournamentBehavior : MissionLogic, ICameraModeLogic
    {
        public const int RoundCount = 4;

        public const int ParticipantCount = 16;

        public const float EndMatchTimerDuration = 6f;

        public const float CheerTimerDuration = 1f;

        private TournamentGame _tournamentGame;

        private ITournamentGameBehavior _gameBehavior;

        private TournamentParticipant[] _participants;

        public const int MaximumBet = 150;

        public const float MaximumOdd = 4f;

        public float BetOdd
        {
            get;
            private set;
        }

        public int BettedDenars
        {
            get;
            private set;
        }

        public TournamentMatch CurrentMatch
        {
            get
            {
                return this.CurrentRound.CurrentMatch;
            }
        }

        public TournamentRound CurrentRound
        {
            get
            {
                return this.Rounds[this.CurrentRoundIndex];
            }
        }

        public int CurrentRoundIndex
        {
            get;
            private set;
        }

        public bool IsPlayerEliminated
        {
            get;
            private set;
        }

        public bool IsPlayerParticipating
        {
            get;
            private set;
        }

        public TournamentMatch LastMatch
        {
            get;
            private set;
        }

        public int MaximumBetInstance
        {
            get
            {
                return Math.Min(150, this.PlayerDenars);
            }
        }

        public TournamentRound NextRound
        {
            get
            {
                if (this.CurrentRoundIndex == 3)
                {
                    return null;
                }
                return this.Rounds[this.CurrentRoundIndex + 1];
            }
        }

        public int OverallExpectedDenars
        {
            get;
            private set;
        }

        public int PlayerDenars
        {
            get
            {
                return Hero.MainHero.Gold;
            }
        }

        public TournamentRound[] Rounds
        {
            get;
            private set;
        }

        public Settlement Settlement
        {
            get;
            private set;
        }

        public TournamentGame TournamentGame
        {
            get
            {
                return this._tournamentGame;
            }
        }

        public TournamentParticipant Winner
        {
            get;
            private set;
        }

        public BattleRoyalTournamentBehavior(TournamentGame tournamentGame, Settlement settlement, ITournamentGameBehavior gameBehavior, bool isPlayerParticipating)
        {
            this.Settlement = settlement;
            this._tournamentGame = tournamentGame;
            this._gameBehavior = gameBehavior;
            this.Rounds = new TournamentRound[4];
            this.CreateParticipants(isPlayerParticipating);
            this.CurrentRoundIndex = -1;
            this.LastMatch = null;
            this.Winner = null;
            this.IsPlayerParticipating = isPlayerParticipating;
        }

        public override void AfterStart()
        {
            this.CurrentRoundIndex = 0;
            this.CreateTorunamentTree();
            this.FillParticipants(this._participants.ToList<TournamentParticipant>());
            this.CalculateBet();
        }

        private void CalculateBet()
        {
            if (this.IsPlayerParticipating)
            {
                if (this.CurrentRound.CurrentMatch == null)
                {
                    this.BetOdd = 0f;
                    return;
                }
                if (this.IsPlayerEliminated || !this.IsPlayerParticipating)
                {
                    this.OverallExpectedDenars = 0;
                    this.BetOdd = 0f;
                    return;
                }
                List<KeyValuePair<Hero, int>> leaderboard = Campaign.Current.TournamentManager.GetLeaderboard();
                int value = 0;
                int num = 0;
                for (int i = 0; i < leaderboard.Count; i++)
                {
                    if (leaderboard[i].Key == Hero.MainHero)
                    {
                        value = leaderboard[i].Value;
                    }
                    if (leaderboard[i].Value > num)
                    {
                        num = leaderboard[i].Value;
                    }
                }
                float level = 30f + (float)Hero.MainHero.Level + (float)Math.Max(0, value * 12 - num * 2);
                float single = 0f;
                float single1 = 0f;
                float level1 = 0f;
                TournamentMatch[] matches = this.CurrentRound.Matches;
                for (int j = 0; j < (int)matches.Length; j++)
                {
                    TournamentMatch tournamentMatch = matches[j];
                    foreach (TournamentTeam team in tournamentMatch.Teams)
                    {
                        float level2 = 0f;
                        foreach (TournamentParticipant participant in team.Participants)
                        {
                            if (participant.Character == CharacterObject.PlayerCharacter)
                            {
                                continue;
                            }
                            int value1 = 0;
                            if (participant.Character.IsHero)
                            {
                                for (int k = 0; k < leaderboard.Count; k++)
                                {
                                    if (leaderboard[k].Key == participant.Character.HeroObject)
                                    {
                                        value1 = leaderboard[k].Value;
                                    }
                                }
                            }
                            level2 += (float)(participant.Character.Level + Math.Max(0, value1 * 8 - num * 2));
                        }
                        if (team.Participants.Any<TournamentParticipant>((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter))
                        {
                            single1 = level2;
                            foreach (TournamentTeam tournamentTeam in tournamentMatch.Teams)
                            {
                                if (team == tournamentTeam)
                                {
                                    continue;
                                }
                                foreach (TournamentParticipant tournamentParticipant in tournamentTeam.Participants)
                                {
                                    int num1 = 0;
                                    if (tournamentParticipant.Character.IsHero)
                                    {
                                        for (int l = 0; l < leaderboard.Count; l++)
                                        {
                                            if (leaderboard[l].Key == tournamentParticipant.Character.HeroObject)
                                            {
                                                num1 = leaderboard[l].Value;
                                            }
                                        }
                                    }
                                    level1 += (float)(tournamentParticipant.Character.Level + Math.Max(0, num1 * 8 - num * 2));
                                }
                            }
                        }
                        single += level2;
                    }
                }
                float single2 = (single1 + level) / (level1 + single1 + level);
                float single3 = level / (single1 + level + 0.5f * (single - (single1 + level1)));
                float single4 = single2 * single3;
                float single5 = MathF.Clamp((float)Math.Pow((double)(1f / single4), 0.75), 1.1f, 4f);
                this.BetOdd = (float)((int)(single5 * 10f)) / 10f;
            }
        }

        private void CreateParticipants(bool includePlayer)
        {
            this._participants = new TournamentParticipant[16];
            List<CharacterObject> participantCharacters = TournamentGame.GetParticipantCharacters(this.Settlement, 16, includePlayer, true);
            int num = 0;
            while (participantCharacters.Count > 0)
            {
                CharacterObject item = participantCharacters[MBRandom.RandomInt(participantCharacters.Count)];
                this._participants[num] = new TournamentParticipant(item, new UniqueTroopDescriptor());
                participantCharacters.Remove(item);
                num++;
            }
        }

        private void CreateTorunamentTree()
        {
            int num = 32;
            for (int i = 0; i < 4; i++)
            {
                int numWin = 16;
                switch (i)
                {
                    case 0:
                        numWin = 16; break;
                    case 1:
                        numWin = 8; break;
                    case 2:
                        numWin = 4; break;
                    case 3:
                        numWin = 1; break;

                }
                this.Rounds[i] = new TournamentRound(num, 1, num, numWin, TournamentGame.QualificationMode.IndividualScore);
                num /= 2;
            }
        }

        public static void DeleteAllTournamentSets()
        {
            foreach (GameEntity list in Mission.Current.Scene.FindEntitiesWithTag("arena_set").ToList<GameEntity>())
            {
                list.Remove();
            }
        }

        public static void DeleteTournamentSetsExcept(GameEntity selectedSetEntity)
        {
            List<GameEntity> list = Mission.Current.Scene.FindEntitiesWithTag("arena_set").ToList<GameEntity>();
            list.Remove(selectedSetEntity);
            foreach (GameEntity gameEntity in list)
            {
                gameEntity.Remove();
            }
        }

        private void EndCurrentMatch()
        {
            this.LastMatch = this.CurrentMatch;
            this.CurrentRound.EndMatch();
            this._gameBehavior.OnMatchEnded();
            if (this.LastMatch.IsPlayerParticipating())
            {
                if (!this.LastMatch.Winners.All<TournamentParticipant>((TournamentParticipant x) => x.Character != CharacterObject.PlayerCharacter))
                {
                    this.OnPlayerWinMatch();
                }
                else
                {
                    this.OnPlayerEliminated();
                }
            }
            if (this.NextRound != null)
            {
                while (true)
                {
                    if (!(
                        from x in this.LastMatch.Winners
                        where !x.IsAssigned
                        select x).Any<TournamentParticipant>())
                    {
                        break;
                    }
                    foreach (TournamentParticipant winner in this.LastMatch.Winners)
                    {
                        if (winner.IsAssigned)
                        {
                            continue;
                        }
                        this.NextRound.AddParticipant(winner, false);
                        winner.IsAssigned = true;
                    }
                }
            }
            if (this.CurrentRound.CurrentMatch == null)
            {
                if (this.CurrentRoundIndex < 3)
                {
                    this.CurrentRoundIndex = this.CurrentRoundIndex + 1;
                    this.CalculateBet();
                    return;
                }
                this.CalculateBet();
                InformationManager.AddQuickInformation(new TextObject("{=tWzLqegB}Tournament is over.", null), 0, null, "");
                this.Winner = this.LastMatch.Winners.FirstOrDefault<TournamentParticipant>();
                foreach (TournamentParticipant tournamentParticipant in this.LastMatch.Winners)
                {
                    if (tournamentParticipant.Character == CharacterObject.PlayerCharacter)
                    {
                        this.OnPlayerWinTournament();
                    }
                    if (!tournamentParticipant.Character.IsHero)
                    {
                        continue;
                    }
                    Campaign.Current.TournamentManager.AddLeaderboardEntry(tournamentParticipant.Character.HeroObject);
                    CampaignEventDispatcher.Instance.OnTournamentWon(tournamentParticipant.Character, this.Settlement.Town);
                }
                if (this.TournamentEnd != null)
                {
                    this.TournamentEnd();
                }
            }
        }

        private void FillParticipants(List<TournamentParticipant> participants)
        {
            foreach (TournamentParticipant participant in participants)
            {
                this.Rounds[this.CurrentRoundIndex].AddParticipant(participant, true);
            }
        }

        public int GetExpectedDenarsForBet(int bet)
        {
            return (int)(this.BetOdd * (float)bet);
        }

        public SpectatorCameraTypes GetMissionCameraLockMode(bool lockedToMainPlayer)
        {
            if (!this.IsPlayerParticipating)
            {
                return SpectatorCameraTypes.LockToAnyAgent;
            }
            return SpectatorCameraTypes.Invalid;
        }

        public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
        {
            canPlayerLeave = false;
            return null;
        }

        public override void OnMissionTick(float dt)
        {
            if (this.CurrentMatch != null && this.CurrentMatch.State == TournamentMatch.MatchState.Started && this._gameBehavior.IsMatchEnded())
            {
                this.EndCurrentMatch();
            }
        }

        private void OnPlayerEliminated()
        {
            this.IsPlayerEliminated = true;
            this.BetOdd = 0f;
            if (this.BettedDenars > 0)
            {
                GiveGoldAction.ApplyForCharacterToSettlement(null, Settlement.CurrentSettlement, this.BettedDenars, false);
            }
            this.OverallExpectedDenars = 0;
        }

        private void OnPlayerWinMatch()
        {
            Campaign.Current.TournamentManager.OnPlayerWinMatch(this._tournamentGame.GetType());
        }

        private void OnPlayerWinTournament()
        {
            if (Campaign.Current.GameMode != CampaignGameMode.Campaign)
            {
                return;
            }
            GainRenownAction.Apply(Hero.MainHero, this._tournamentGame.TournamentWinRenown, false);
            if (Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction.Leader != Hero.MainHero)
            {
                GainKingdomInfluenceAction.ApplyForDefault(Hero.MainHero, 1f);
            }
            Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(this._tournamentGame.Prize, 1, true);
            if (this.OverallExpectedDenars > 0)
            {
                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.OverallExpectedDenars, false);
            }
            Campaign.Current.TournamentManager.OnPlayerWinTournament(this._tournamentGame.GetType());
        }

        public void PlaceABet(int bet)
        {
            this.BettedDenars = this.BettedDenars + bet;
            this.OverallExpectedDenars = this.OverallExpectedDenars + this.GetExpectedDenarsForBet(bet);
            GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, bet, true);
        }

        public void SkipMatch()
        {
            this.CurrentMatch.Start();
            this._gameBehavior.SkipMatch(this.CurrentMatch);
            this.EndCurrentMatch();
        }

        public void StartMatch()
        {
            if (this.CurrentMatch.IsPlayerParticipating())
            {
                Campaign.Current.TournamentManager.OnPlayerJoinMatch(this._tournamentGame.GetType());
            }
            this.CurrentMatch.Start();
            base.Mission.SetMissionMode(MissionMode.Tournament, true);
            this._gameBehavior.StartMatch(this.CurrentMatch, this.NextRound == null);
        }

        public event Action TournamentEnd;
    }
}

