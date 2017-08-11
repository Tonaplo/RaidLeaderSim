using System.Collections.Generic;
using UnityEngine;

public class BaseHealScript : BaseHealOrAttackScript
{

    int heavyDamageCutoff = 40;
    int mediumDamageCutoff = 70;

    public class Priority
    {
        int m_priority;
        Enums.RaidHealingState m_state;

        public Priority(int p, Enums.RaidHealingState s) { m_priority = p; m_state = s; }

        public void SetState(int p, Enums.RaidHealingState s) { m_priority = p; m_state = s; }

        public int GetPriority() { return m_priority; }
        public Enums.RaidHealingState GetState() { return m_state; }
    }

    protected List<RaiderScript> m_raid;

    protected List<RaiderScript> Raid
    {
        get { return m_raid; }
        set { m_raid = value; }
    }

    protected List<Priority> m_priorityList;

    protected List<Priority> PriorityList
    {
        get { return m_priorityList; }
        set { m_priorityList = value; }
    }

    protected void GetBestTargets(ref List<RaiderScript> targets)
    {
        PriorityList.Sort(delegate (Priority x, Priority y)
        {
            if (x.GetPriority() < y.GetPriority())
                return -1;
            else return 1;
        });

        int stateMask = 0;
        GetRaidState(out stateMask);

        for (int i = 0; i < PriorityList.Count; i++)
        {
            if (((int)PriorityList[i].GetState() & stateMask) != 0)
            {
                GetTargetsFromState(PriorityList[i].GetState(), ref targets);
                return;
            }
        }
    }

    protected List<RaiderScript> GetRandomTargets()
    {
        List<RaiderScript> targets = new List<RaiderScript>();
        int type = UnityEngine.Random.Range(0, 3);
        switch (type)
        {
            case 0:
                {
                    RaiderScript lowest = Raid[0];
                    int lowestDiff = 0;
                    for (int i = 1; i < Raid.Count; i++)
                    {
                        if (Raid[i].IsDead())
                            continue;

                        int thisDiff = Raid[i].GetMaxHealth() - Raid[i].GetHealth();
                        if (thisDiff > lowestDiff) {
                            lowest = Raid[i];
                            lowestDiff = Raid[i].GetMaxHealth() - Raid[i].GetHealth();
                        }
                    }

                    targets.Add(lowest);
                }
                break;
            case 1:
                {
                    int numHealed = 3;
                    targets = new List<RaiderScript>(Raid);
                    TrimDeadFromList(ref targets);
                    numHealed = numHealed > targets.Count ? targets.Count : numHealed;
                    TrimToLowestXFromList(ref targets, numHealed);
                }
                break;
            case 2:
                {

                    int numHealed = Raid.Count / 2;
                    targets = new List<RaiderScript>(Raid);
                    TrimDeadFromList(ref targets);
                    numHealed = numHealed > targets.Count ? targets.Count : numHealed;
                    TrimToLowestXFromList(ref targets, numHealed);
                }
                break;
            default:
                break;
        }

        return targets;
    }

    protected void GetTanks(ref List<RaiderScript> tanks)
    {
        tanks.AddRange(Raid.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Tank));
        TrimDeadFromList(ref tanks);
    }

    protected void GetHealers(ref List<RaiderScript> healers)
    {
        healers.AddRange(Raid.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.Healer));
        TrimDeadFromList(ref healers);
    }

    protected void GetDPS(ref List<RaiderScript> dps)
    {
        dps.AddRange(Raid.FindAll(x => x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.MeleeDPS || x.Raider.RaiderStats.GetRole() == Enums.CharacterRole.RangedDPS));
        TrimDeadFromList(ref dps);
    }

    protected void TrimToLowestXFromList(ref List<RaiderScript> inputList, int numMembers)
    {
        Debug.Assert(inputList.Count > numMembers);
        
        inputList.Sort(delegate (RaiderScript x, RaiderScript y)
        {
            if (x.GetHealthPercent() > y.GetHealthPercent())
                return -1;
            else return 1;
        });

        inputList.RemoveRange(numMembers, inputList.Count - numMembers);
    }

    protected void TrimDeadFromList(ref List<RaiderScript> targets)
    {
        targets.RemoveAll(x => x.IsDead());
    }

    void GetRaidState(out int stateMask)
    {
        stateMask = (int)Enums.RaidHealingState.RandomTargets;

        List<RaiderScript> tanks = new List<RaiderScript>();
        GetTanks(ref tanks);

        for (int i = 0; i < tanks.Count; i++)
        {
            int percent = Mathf.RoundToInt(tanks[i].GetHealthPercent());
            if (percent < heavyDamageCutoff)
                stateMask |= (int)Enums.RaidHealingState.TankHeavyDamage;

            if (percent < mediumDamageCutoff)
                stateMask |= (int)Enums.RaidHealingState.TankMediumDamage;
        }

        List<RaiderScript> restOfRaid = new List<RaiderScript>();
        GetDPS(ref restOfRaid);
        GetHealers(ref restOfRaid);

        int raidHealthPercent = 0;

        for (int i = 0; i < restOfRaid.Count; i++)
        {
            int percent = Mathf.RoundToInt(restOfRaid[i].GetHealthPercent());
            if (percent < heavyDamageCutoff)
                stateMask |= (int)Enums.RaidHealingState.RaidSingleHeavyDamage;

            if (percent < mediumDamageCutoff)
                stateMask |= (int)Enums.RaidHealingState.RaidSingleMediumDamage;

            raidHealthPercent += percent;
        }

        raidHealthPercent /= restOfRaid.Count;
        if (raidHealthPercent < heavyDamageCutoff)
            stateMask |= (int)Enums.RaidHealingState.RaidSingleHeavyDamage;

        if (raidHealthPercent < mediumDamageCutoff)
            stateMask |= (int)Enums.RaidHealingState.RaidSingleMediumDamage;
    }

    void GetTargetsFromState(Enums.RaidHealingState state, ref List<RaiderScript> targets)
    {
        targets = new List<RaiderScript>();

        switch (state)
        {
            case Enums.RaidHealingState.TankMediumDamage:
            case Enums.RaidHealingState.TankHeavyDamage:
                {
                    List<RaiderScript> tanks = new List<RaiderScript>();
                    GetTanks(ref tanks);
                    int lowestIndex = 0;
                    for (int i = 1; i < tanks.Count; i++)
                    {
                        if (tanks[i].GetHealthPercent() < tanks[lowestIndex].GetHealthPercent())
                            lowestIndex = i;
                    }

                    targets.Add(tanks[lowestIndex]);
                }
                break;
            case Enums.RaidHealingState.RaidSingleMediumDamage:
            case Enums.RaidHealingState.RaidSingleHeavyDamage:
                {
                    List<RaiderScript> restOfRaid = new List<RaiderScript>();
                    GetDPS(ref restOfRaid);
                    GetHealers(ref restOfRaid);
                    int lowestIndex = 0;
                    for (int i = 1; i < restOfRaid.Count; i++)
                    {
                        if (restOfRaid[i].GetHealthPercent() < restOfRaid[lowestIndex].GetHealthPercent())
                            lowestIndex = i;
                    }
                    targets.Add(restOfRaid[lowestIndex]);
                }
                break;
            case Enums.RaidHealingState.RaidMultiMediumDamage:
                {
                    List<RaiderScript> restOfRaid = new List<RaiderScript>();
                    GetDPS(ref restOfRaid);
                    GetHealers(ref restOfRaid);
                    for (int i = 1; i < restOfRaid.Count; i++)
                    {
                        if (restOfRaid[i].GetHealthPercent() < mediumDamageCutoff && restOfRaid[i].GetHealthPercent() > heavyDamageCutoff)
                            targets.Add(restOfRaid[i]);
                    }
                }
                break;
            case Enums.RaidHealingState.RaidMultiHeavyDamage:
                {
                    List<RaiderScript> restOfRaid = new List<RaiderScript>();
                    GetDPS(ref restOfRaid);
                    GetHealers(ref restOfRaid);
                    for (int i = 1; i < restOfRaid.Count; i++)
                    {
                        if (restOfRaid[i].GetHealthPercent() < mediumDamageCutoff)
                            targets.Add(restOfRaid[i]);
                    }
                }
                break;
            case Enums.RaidHealingState.RandomTargets:
                targets = GetRandomTargets();
                break;
            default:
                break;
        }

        Debug.AssertFormat(targets.Count > 0, "Got 0 targets from raidstate" + state);
    }
}
