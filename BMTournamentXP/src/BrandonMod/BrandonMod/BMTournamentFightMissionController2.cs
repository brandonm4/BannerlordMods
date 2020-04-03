using SandBox.Source.Missions.AgentControllers;
using SandBox.TournamentMissions.Missions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.Source.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace BMTournamentXP
{
    public class BMTournamentFightMissionController2 : MissionLogic, ITournamentGameBehavior
    {
        private readonly string[] _weaponTemplatesIdTeamSizeOne = new String[] { "tournament_template_aserai_one_participant_set_v1", "tournament_template_battania_one_participant_set_v1", "tournament_template_battania_one_participant_set_v2", "tournament_template_empire_one_participant_set_v1", "tournament_template_khuzait_one_participant_set_v1", "tournament_template_khuzait_one_participant_set_v2", "tournament_template_vlandia_one_participant_set_v1", "tournament_template_vlandia_one_participant_set_v2", "tournament_template_vlandia_one_participant_set_v3", "tournament_template_sturgia_one_participant_set_v1", "tournament_template_sturgia_one_participant_set_v2" };

        private readonly string[] _weaponTemplatesIdTeamSizeTwo = new String[] { "tournament_template_aserai_two_participant_set_v1", "tournament_template_aserai_two_participant_set_v2", "tournament_template_aserai_two_participant_set_v3", "tournament_template_battania_two_participant_set_v1", "tournament_template_battania_two_participant_set_v2", "tournament_template_battania_two_participant_set_v3", "tournament_template_battania_two_participant_set_v4", "tournament_template_battania_two_participant_set_v5", "tournament_template_empire_two_participant_set_v1", "tournament_template_empire_two_participant_set_v2", "tournament_template_empire_two_participant_set_v3", "tournament_template_khuzait_two_participant_set_v1", "tournament_template_khuzait_two_participant_set_v2", "tournament_template_khuzait_two_participant_set_v3", "tournament_template_vlandia_two_participant_set_v1", "tournament_template_vlandia_two_participant_set_v2", "tournament_template_vlandia_two_participant_set_v3", "tournament_template_vlandia_two_participant_set_v4", "tournament_template_sturgia_two_participant_set_v1", "tournament_template_sturgia_two_participant_set_v2", "tournament_template_sturgia_two_participant_set_v3" };

        private readonly string[] _weaponTemplatesIdTeamSizeFour = new String[] { "tournament_template_aserai_four_participant_set_v1", "tournament_template_aserai_four_participant_set_v2", "tournament_template_aserai_four_participant_set_v3", "tournament_template_aserai_four_participant_set_v4", "tournament_template_battania_four_participant_set_v1", "tournament_template_battania_four_participant_set_v2", "tournament_template_battania_four_participant_set_v3", "tournament_template_empire_four_participant_set_v1", "tournament_template_empire_four_participant_set_v2", "tournament_template_empire_four_participant_set_v3", "tournament_template_khuzait_four_participant_set_v1", "tournament_template_khuzait_four_participant_set_v2", "tournament_template_khuzait_four_participant_set_v3", "tournament_template_vlandia_four_participant_set_v1", "tournament_template_vlandia_four_participant_set_v2", "tournament_template_vlandia_four_participant_set_v3", "tournament_template_vlandia_four_participant_set_v4", "tournament_template_sturgia_four_participant_set_v1", "tournament_template_sturgia_four_participant_set_v2", "tournament_template_sturgia_four_participant_set_v3" };

        private TournamentMatch _match;

        private bool _isLastRound;

        private BasicTimer _endTimer;

        private BasicTimer _cheerTimer;

        private List<GameEntity> _spawnPoints;

        private bool _isSimulated;

        private bool _forceEndMatch;

        private CultureObject _culture;

        private List<TournamentParticipant> _aliveParticipants;

        private List<TournamentTeam> _aliveTeams;

        public BMTournamentFightMissionController2(CultureObject culture)
        {
            this._match = null;
            this._culture = culture;
        }

        private void AddRandomClothes(CultureObject culture, TournamentParticipant participant)
        {
            Equipment randomElement = participant.Character.BattleEquipments.GetRandomElement<Equipment>();
            for (int i = 5; i < 10; i++)
            {
                EquipmentElement equipmentFromSlot = randomElement.GetEquipmentFromSlot((EquipmentIndex)i);
                if (equipmentFromSlot.Item != null)
                {
                    participant.MatchEquipment.AddEquipmentToSlotWithoutAgent((EquipmentIndex)i, equipmentFromSlot);
                }
            }
        }

        private void AddScoreToRemainingTeams()
        {
            foreach (TournamentTeam _aliveTeam in this._aliveTeams)
            {
                foreach (TournamentParticipant participant in _aliveTeam.Participants)
                {
                    participant.AddScore(1);
                }
            }
        }

        public override void AfterStart()
        {
            TournamentBehavior.DeleteTournamentSetsExcept(base.Mission.Scene.FindEntityWithTag("tournament_fight"));
            this._spawnPoints = new List<GameEntity>();
            for (int i = 0; i < 4; i++)
            {
                GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag(String.Concat((object)"sp_arena_", i + 1));
                if (gameEntity != null)
                {
                    this._spawnPoints.Add(gameEntity);
                }
            }
            if (this._spawnPoints.Count < 4)
            {
                this._spawnPoints = base.Mission.Scene.FindEntitiesWithTag("sp_arena").ToList<GameEntity>();
            }
        }

        public bool CheckIfIsThereAnyEnemies()
        {
            bool flag;
            Team team = null;
            using (IEnumerator<Agent> enumerator = base.Mission.Agents.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Agent current = enumerator.Current;
                    if (!current.IsHuman || current.Team == null)
                    {
                        continue;
                    }
                    if (team != null)
                    {
                        if (team == current.Team)
                        {
                            continue;
                        }
                        flag = true;
                        return flag;
                    }
                    else
                    {
                        team = current.Team;
                    }
                }
                return false;
            }
            return flag;
        }

        private bool CheckIfTeamIsDead(TournamentTeam affectedParticipantTeam)
        {
            bool flag = true;
            foreach (TournamentParticipant _aliveParticipant in this._aliveParticipants)
            {
                if (_aliveParticipant.Team != affectedParticipantTeam)
                {
                    continue;
                }
                flag = false;
                return flag;
            }
            return flag;
        }

        private List<Equipment> GetTeamWeaponEquipmentList(int teamSize)
        {
            List<string> list;
            List<Equipment> equipment = new List<Equipment>();
            string stringId = PlayerEncounter.Settlement.Culture.StringId;
            if (teamSize == 4)
            {
                list = this._weaponTemplatesIdTeamSizeFour.ToList<string>();
            }
            else
            {
                list = (teamSize == 2 ? this._weaponTemplatesIdTeamSizeTwo.ToList<string>() : this._weaponTemplatesIdTeamSizeOne.ToList<string>());
            }
            List<string> strs = list;
            strs = strs.FindAll((string x) => x.Contains(stringId));
            foreach (Equipment battleEquipment in MBObjectManager.Instance.GetObject<CharacterObject>(strs[MBRandom.RandomInt(strs.Count)]).BattleEquipments)
            {
                Equipment equipment1 = new Equipment();
                equipment1.FillFrom(battleEquipment, true);
                equipment.Add(equipment1);
            }
            return equipment;
        }

        public bool IsMatchEnded()
        {
            if (this._isSimulated || this._match == null)
            {
                return true;
            }
            if (this._endTimer != null && this._endTimer.ElapsedTime > 6f || this._forceEndMatch)
            {
                this._forceEndMatch = false;
                this._endTimer = null;
                return true;
            }
            if (this._cheerTimer == null || this._cheerTimer.ElapsedTime <= 1f)
            {
                if (this._endTimer == null && !this.CheckIfIsThereAnyEnemies())
                {
                    this._endTimer = new BasicTimer(MBCommon.TimeType.Mission);
                    this._cheerTimer = new BasicTimer(MBCommon.TimeType.Mission);
                }
                return false;
            }
            this.OnMatchResultsReady();
            this._cheerTimer = null;
            AgentVictoryLogic missionBehaviour = base.Mission.GetMissionBehaviour<AgentVictoryLogic>();
            foreach (Agent agent in base.Mission.Agents)
            {
                if (!agent.IsAIControlled)
                {
                    continue;
                }
                missionBehaviour.SetTimersOfVictoryReactions(agent, 1f, 3f);
            }
            return false;
        }

        private bool IsThereAnyPlayerAgent()
        {
            if (base.Mission.MainAgent != null && base.Mission.MainAgent.IsActive())
            {
                return true;
            }
            return base.Mission.Agents.Any<Agent>((Agent agent) => agent.IsPlayerControlled);
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
        {
            if (!GameNetwork.IsClientOrReplay && !this.IsMatchEnded() && affectorAgent != null && affectedAgent != affectorAgent && affectedAgent.IsHuman && affectorAgent.IsHuman)
            {
                TournamentParticipant participant = this._match.GetParticipant(affectedAgent.Origin.UniqueSeed);
                this._aliveParticipants.Remove(participant);
                if (this.CheckIfTeamIsDead(participant.Team))
                {
                    this._aliveTeams.Remove(participant.Team);
                    this.AddScoreToRemainingTeams();
                }
            }
        }

        public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
        {
            InquiryData inquiryDatum = null;
            canPlayerLeave = true;
            TournamentBehavior missionBehaviour = Mission.Current.GetMissionBehaviour<TournamentBehavior>();
            if (this._match != null)
            {
                if (this._match.IsPlayerParticipating())
                {
                    MBTextManager.SetTextVariable("SETTLEMENT_NAME", Hero.MainHero.CurrentSettlement.EncyclopediaLinkWithName, false);
                    if (!this.IsThereAnyPlayerAgent())
                    {
                        if (!this.CheckIfIsThereAnyEnemies())
                        {
                            this._forceEndMatch = true;
                            canPlayerLeave = false;
                        }
                        else
                        {
                            inquiryDatum = new InquiryData(GameTexts.FindText("str_tournament", null).ToString(), GameTexts.FindText("str_tournament_skip", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(missionBehaviour.SkipMatch), null, "");
                        }
                    }
                    else if (base.Mission.IsPlayerCloseToAnEnemy(5f))
                    {
                        canPlayerLeave = false;
                        InformationManager.AddQuickInformation(GameTexts.FindText("str_can_not_retreat", null), 0, null, "");
                    }
                    else if (!this.CheckIfIsThereAnyEnemies())
                    {
                        this._forceEndMatch = true;
                        canPlayerLeave = false;
                    }
                    else
                    {
                        inquiryDatum = new InquiryData(GameTexts.FindText("str_tournament", null).ToString(), GameTexts.FindText("str_tournament_forfeit_game", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(missionBehaviour.SkipMatch), null, "");
                    }
                }
                else if (!this.CheckIfIsThereAnyEnemies())
                {
                    this._forceEndMatch = true;
                    canPlayerLeave = false;
                }
                else
                {
                    inquiryDatum = new InquiryData(GameTexts.FindText("str_tournament", null).ToString(), GameTexts.FindText("str_tournament_skip", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(missionBehaviour.SkipMatch), null, "");
                }
            }
            return inquiryDatum;
        }

        public void OnMatchEnded()
        {
            base.Mission.GetMissionBehaviour<AgentVictoryLogic>().ClearAgentList();
            for (int i = base.Mission.Agents.Count - 1; i >= 0; i--)
            {
                base.Mission.Agents[i].FadeOut(true);
            }
            base.Mission.ClearCorpses();
            base.Mission.Teams.Clear();
            base.Mission.RemoveSpawnedItemsAndMissiles();
            this._match = null;
            this._endTimer = null;
            this._cheerTimer = null;
            this._isSimulated = false;
        }

        public void OnMatchResultsReady()
        {
            if (!this._match.IsPlayerParticipating())
            {
                InformationManager.AddQuickInformation(new TextObject("{=UBd0dEPp}Match is over", null), 0, null, "");
                return;
            }
            if (!this._match.IsPlayerWinner())
            {
                if (this._match.QualificationMode == TournamentGame.QualificationMode.IndividualScore)
                {
                    InformationManager.AddQuickInformation(new TextObject("{=lcVauEKV}Round is over, you are disqualified from the tournament.", null), 0, null, "");
                    return;
                }
                InformationManager.AddQuickInformation(new TextObject("{=MLyBN51z}Round is over, your team is disqualified from the tournament.", null), 0, null, "");
                return;
            }
            if (this._isLastRound)
            {
                if (this._match.QualificationMode == TournamentGame.QualificationMode.IndividualScore)
                {
                    InformationManager.AddQuickInformation(new TextObject("{=Jn0k20c3}Round is over, you survived the final round of the tournament.", null), 0, null, "");
                    return;
                }
                InformationManager.AddQuickInformation(new TextObject("{=wOqOQuJl}Round is over, your team survived the final round of the tournament.", null), 0, null, "");
                return;
            }
            if (this._match.QualificationMode == TournamentGame.QualificationMode.IndividualScore)
            {
                InformationManager.AddQuickInformation(new TextObject("{=uytwdSVH}Round is over, you are qualified for the next stage of the tournament.", null), 0, null, "");
                return;
            }
            InformationManager.AddQuickInformation(new TextObject("{=fkOYvnVG}Round is over, your team is qualified for the next stage of the tournament.", null), 0, null, "");
        }

        public void PrepareForMatch()
        {
            if (!GameNetwork.IsClientOrReplay)
            {
                List<Equipment> teamWeaponEquipmentList = this.GetTeamWeaponEquipmentList(this._match.Teams.First<TournamentTeam>().Participants.Count<TournamentParticipant>());
                foreach (TournamentTeam team in this._match.Teams)
                {
                    int num = 0;
                    foreach (TournamentParticipant participant in team.Participants)
                    {
                        participant.MatchEquipment = teamWeaponEquipmentList[num].Clone(false);
                        this.AddRandomClothes(this._culture, participant);
                        num++;
                    }
                }
            }
        }

        private void Simulate()
        {
            float single;
            float single1;
            int num;
            TournamentParticipant item;
            this._isSimulated = false;
            if (base.Mission.Agents.Count == 0)
            {
                this._aliveParticipants = this._match.Participants.ToList<TournamentParticipant>();
                this._aliveTeams = this._match.Teams.ToList<TournamentTeam>();
            }
            TournamentParticipant tournamentParticipant = this._aliveParticipants.FirstOrDefault<TournamentParticipant>((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter);
            if (tournamentParticipant != null)
            {
                TournamentTeam team = tournamentParticipant.Team;
                foreach (TournamentParticipant participant in team.Participants)
                {
                    participant.ResetScore();
                    this._aliveParticipants.Remove(participant);
                }
                this._aliveTeams.Remove(team);
                this.AddScoreToRemainingTeams();
            }
            Dictionary<TournamentParticipant, Tuple<float, float>> tournamentParticipants = new Dictionary<TournamentParticipant, Tuple<float, float>>();
            foreach (TournamentParticipant _aliveParticipant in this._aliveParticipants)
            {
                _aliveParticipant.Character.GetSimulationAttackPower(out single1, out single, _aliveParticipant.MatchEquipment);
                tournamentParticipants.Add(_aliveParticipant, new Tuple<float, float>(single1, single));
            }
            int count = 0;
            while (this._aliveParticipants.Count > 1 && this._aliveTeams.Count > 1)
            {
                count++;
                count %= this._aliveParticipants.Count;
                TournamentParticipant item1 = this._aliveParticipants[count];
                do
                {
                    num = MBRandom.RandomInt(this._aliveParticipants.Count);
                    item = this._aliveParticipants[num];
                }
                while (item1 == item || item1.Team == item.Team);
                if (tournamentParticipants[item].Item2 - tournamentParticipants[item1].Item1 <= 0f)
                {
                    tournamentParticipants.Remove(item);
                    this._aliveParticipants.Remove(item);
                    if (this.CheckIfTeamIsDead(item.Team))
                    {
                        this._aliveTeams.Remove(item.Team);
                        this.AddScoreToRemainingTeams();
                    }
                    if (num >= count)
                    {
                        continue;
                    }
                    count--;
                }
                else
                {
                    tournamentParticipants[item] = new Tuple<float, float>(tournamentParticipants[item].Item1, tournamentParticipants[item].Item2 - tournamentParticipants[item1].Item1);
                }
            }
            this._isSimulated = true;
        }

        public void SkipMatch(TournamentMatch match)
        {
            this._match = match;
            this.PrepareForMatch();
            this.Simulate();
        }

        private void SpawnAgentWithRandomItems(TournamentParticipant participant, Team team, MatrixFrame frame)
        {
            frame.Strafe((float)MBRandom.RandomInt(-2, 2) * 1f);
            frame.Advance((float)MBRandom.RandomInt(0, 2) * 1f);
            CharacterObject character = participant.Character;
            AgentBuildData agentBuildDatum = (new AgentBuildData(new SimpleAgentOrigin(character, -1, null, participant.Descriptor))).Team(team).InitialFrame(frame).Equipment(participant.MatchEquipment).ClothingColor1(team.Color).Banner(team.Banner).Controller((character.IsPlayerCharacter ? Agent.ControllerType.Player : Agent.ControllerType.AI));
            Agent hitPoints = base.Mission.SpawnAgent(agentBuildDatum, false, 0);
            if (!character.IsPlayerCharacter)
            {
                hitPoints.AddController(typeof(FighterAgentController));
                hitPoints.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
            }
            else
            {
                hitPoints.Health = (float)character.HeroObject.HitPoints;
                base.Mission.PlayerTeam = team;
            }
            hitPoints.WieldInitialWeapons();
        }

        private void SpawnTournamentParticipant(GameEntity spawnPoint, TournamentParticipant participant, Team team)
        {
            MatrixFrame globalFrame = spawnPoint.GetGlobalFrame();
            globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
            this.SpawnAgentWithRandomItems(participant, team, globalFrame);
        }

        public void StartMatch(TournamentMatch match, bool isLastRound)
        {
            this._match = match;
            this._isLastRound = isLastRound;
            this.PrepareForMatch();
            base.Mission.SetMissionMode(MissionMode.Battle, true);
            if (!GameNetwork.IsClientOrReplay)
            {
                List<Team> teams = new List<Team>();
                int count = this._spawnPoints.Count;
                int num = 0;
                foreach (TournamentTeam team in this._match.Teams)
                {
                    Team team1 = base.Mission.Teams.Add(BattleSideEnum.None, team.TeamColor, 0, team.TeamBanner, true, false, true);
                    GameEntity item = this._spawnPoints[num % count];
                    foreach (TournamentParticipant participant in team.Participants)
                    {
                        if (!participant.Character.IsPlayerCharacter)
                        {
                            continue;
                        }
                        this.SpawnTournamentParticipant(item, participant, team1);
                      //  goto Label0;
                    }
                    foreach (TournamentParticipant tournamentParticipant in team.Participants)
                    {
                        if (tournamentParticipant.Character.IsPlayerCharacter)
                        {
                            continue;
                        }
                        this.SpawnTournamentParticipant(item, tournamentParticipant, team1);
                    }
                    num++;
                    teams.Add(team1);
                }
                for (int i = 0; i < teams.Count; i++)
                {
                    for (int j = i + 1; j < teams.Count; j++)
                    {
                        teams[i].SetIsEnemyOf(teams[j], true);
                    }
                }
                this._aliveParticipants = this._match.Participants.ToList<TournamentParticipant>();
                this._aliveTeams = this._match.Teams.ToList<TournamentTeam>();
            }
        }
    }
}
