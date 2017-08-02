using System.Collections.Generic;
using UnityEngine;

public class BaseHealScript : BaseHealOrAttackScript
{
    protected List<RaiderScript> m_raid;

    protected List<RaiderScript> Raid
    {
        get { return m_raid; }
        set { m_raid = value; }
    }

    protected void TrimDeadFromList(ref List<RaiderScript> targets)
    {
        targets.RemoveAll(x => x.IsDead());
    }

    protected List<RaiderScript> GetRandomTargets()
    {
        List<RaiderScript> targets = new List<RaiderScript>();
        int type = UnityEngine.Random.Range(0, 3);
        switch (type)
        {
            case 0:
                {
                    RaiderScript lowest = null;
                    int lowestDiff = 0;
                    for (int i = 0; i < Raid.Count; i++)
                    {
                        if (Raid[i].IsDead())
                            continue;

                        int thisDiff = Raid[i].GetMaxHealth() - Raid[i].GetHealth();
                        if (thisDiff > lowestDiff) {
                            lowest = Raid[i];
                            lowestDiff = Raid[i].GetMaxHealth() - Raid[i].GetHealth();
                        }
                    }

                    if (lowest)
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
        tanks.AddRange(Raid.FindAll(x => x.Raider.RaiderStats().GetRole() == Enums.CharacterRole.Tank));
        TrimDeadFromList(ref tanks);
    }

    protected void GetHealers(ref List<RaiderScript> healers)
    {
        healers.AddRange(Raid.FindAll(x => x.Raider.RaiderStats().GetRole() == Enums.CharacterRole.Healer));
        TrimDeadFromList(ref healers);
    }

    protected void GetDPS(ref List<RaiderScript> dps)
    {
        dps.AddRange(Raid.FindAll(x => x.Raider.RaiderStats().GetRole() == Enums.CharacterRole.MeleeDPS || x.Raider.RaiderStats().GetRole() == Enums.CharacterRole.RangedDPS));
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
}
