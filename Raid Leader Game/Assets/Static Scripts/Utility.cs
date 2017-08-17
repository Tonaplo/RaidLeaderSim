using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    static BaseEncounter m_currentEncounter;
    public static BaseEncounter CurrentEncounter{ get { return m_currentEncounter; } }
    static List<string> names;

    public static void DebugInitalize()
    {
        Initialize();
        PlayerData.GenerateDebugRoster();
    }

    public static void Initialize() {
        names = new List<string>
        {
            "Praerend",
            "Everett",
            "Morifa",
            "Amranar",
            "Farahn",
            "Faerand",
            "Fimwack",
            "Miriyal",
            "Greybone",
            "Earendil",
            "Kalithumos",
            "Kaligon",
            "Granjior",
            "Rahran",
            "Novgorod",
            "Kaldorath",
            "Arkator",
            "Andoe",
            "Niry",
            "Gethi",
            "Ene",
            "Assol",
            "Onun",
            "Gamec",
            "Nossu",
            "Volni",
            "Esu",
            "Uliok",
            "Rhilnush",
            "Kiozon",
            "Linnas",
            "Uthmid",
            "Weldy",
            "Zezob",
            "Ilgral",
            "Phiathon",
            "Phimnar",
            "Muroes",
            "Selar",
            "Tumu",
            "Walel",
            "Gelnis",
            "Shidru",
            "Diassi",
            "Murshal",
            "Kulnun",
            "Lasal",
            "Nulnu",
            "Phamru",
            "Daldu",
            "Renmu",
            "Jella",
            "Zulab",
            "Lullosh",
            "Wardi",
            "Shynmo",
            "Vordoecy",
            "Omulgra",
            "Renamron",
            "Muthmime",
            "Hanunras",
            "Larialnu",
            "Rhorlano",
            "Vamodra",
            "Resissys",
            "Zosilneb",
            "Rillosse",
            "Arana",
            "Onnutha",
            "Orune",
            "Oshasa",
            "Ocilmir",
            "Shanralla",
            "Unarnel",
            "Worusos",
            "Sonmarnu",
            "Gylese",
            "Zyrliro",
            "Chalnony",
            "Phamnullo",
            "Tardino",
            "Rimenil"
        };

        PlayerData.Initialize();

    }

    #region Character Related
    public static void GetRandomCharacterName(ref List<string> outNames, int numNamesNeeded) {
        List<string> namePool = new List<string>(names);

        if (outNames.Count > 0)
        {
            List<string> removables = new List<string>(outNames);
            namePool.RemoveAll(x => removables.Contains(x));
        }

        Debug.Assert(namePool.Count >= numNamesNeeded);

        for (int i = 0; i < numNamesNeeded; i++)
        {
            int index = Random.Range(0, namePool.Count - 1);
            outNames.Add(namePool[index]);
            namePool.RemoveAt(index);
        }
    }

    public static float GetFussyCastTime(float baseCastTime)
    {
        return Random.Range(baseCastTime * 0.8f, 1.2f * baseCastTime);
    }

    public static string GetRoleString(Enums.CharacterRole role)
    {
        if (role == Enums.CharacterRole.RangedDPS)
            return "RDPS";

        if (role == Enums.CharacterRole.MeleeDPS)
            return "MDPS";

        return role.ToString();
    }

    public static Color GetColorFromClass(Enums.CharacterClass Class)
    {
        switch (Class)
        {
            case Enums.CharacterClass.Fighter:
                return new Color(0.65f, 0.32f, 0.18f);
            case Enums.CharacterClass.Shadow:
                return Color.yellow;
            case Enums.CharacterClass.Totemic:
                return new Color(0.05f, 0.5f, 0.05f);
            case Enums.CharacterClass.Sorcerer:
                return Color.cyan;
            case Enums.CharacterClass.Paladin:
                return Color.magenta;
            case Enums.CharacterClass.Occultist:
                return Color.red;
            default:
                return Color.red;
        }
    }

    public static string GetDescriptionOfClass(Enums.CharacterClass Class)
    {
        switch (Class)
        {
            case Enums.CharacterClass.Fighter:
                return "A fearsome tribe, the Warriors use aggression and rage in battle. They can either protect as a mighty Guardian or ravage their foes as the unstoppable Berserker.";
            case Enums.CharacterClass.Shadow:
                return "The Shadows operates in secret and shyes away from the spotlight. They inflict terrible agony as the stealthy Assassin, or pick off their foes from afar as Rangers.";
            case Enums.CharacterClass.Totemic:
                return "The Totemics live in harmony with Mother Nature draw their power from her. They can mend their allies as Naturalists or unleash the fury of their Mother as Elementalists.";
            case Enums.CharacterClass.Sorcerer:
                return "Schooled in the magical arts, the Sorcerers use the Arcane in combat. They can heal wounds as Diviners, or decimate their foes as fire-wielding Wizards.";
            case Enums.CharacterClass.Paladin:
                return "Believers in truth and the mercy of God, the Paladins are a noble class. They ask the help of their Savior to save their allies from death as Clerics, or protect them as Knights.";
            case Enums.CharacterClass.Occultist:
                return "The Occultists wield of dark and obscure magic and the only thing they desire is more power. They obliterate their enemies, either from afar as Necromancers or up close as Scourges.";
            default:
                return "Class not implemented!";
        }
    }

    public static string GetDescriptionOfSpec(Enums.CharacterSpec spec)
    {

        BaseHealOrAttackScript attackScript = new GuardianAttack();
        BaseHealScript healScript = new ClericHealScript();
        bool isHealScript = false;
        switch (spec)
        {
            case Enums.CharacterSpec.Guardian:
                attackScript = new GuardianAttack();
                break;
            case Enums.CharacterSpec.Knight:
                attackScript = new KnightAttack();
                break;
            case Enums.CharacterSpec.Berserker:
                attackScript = new BerserkerAttack();
                break;
            case Enums.CharacterSpec.Assassin:
                attackScript = new AssasinAttack();
                break;
            case Enums.CharacterSpec.Scourge:
                attackScript = new ScourgeAttack();
                break;
            case Enums.CharacterSpec.Ranger:
                attackScript = new RangerAttack();
                break;
            case Enums.CharacterSpec.Wizard:
                attackScript = new WizardAttack();
                break;
            case Enums.CharacterSpec.Elementalist:
                attackScript = new ElementalistAttack();
                break;
            case Enums.CharacterSpec.Necromancer:
                attackScript = new NecromancerAttack();
                break;

            case Enums.CharacterSpec.Cleric:
                healScript = new ClericHealScript();
                isHealScript = true;
                break;
            case Enums.CharacterSpec.Diviner:
                healScript = new DivinerHealScript();
                isHealScript = true;
                break;
            case Enums.CharacterSpec.Naturalist:
                healScript = new NaturalistHealScript();
                isHealScript = true;
                break;
            default:
                return "No spec found!";
        }

        if (!isHealScript) {
            attackScript.Setup();
            return attackScript.GetDescription();
        } else {
            healScript.Setup();
            return healScript.GetDescription();
        }
    }

    public static Enums.CharacterSpec GetOtherSpec(Enums.CharacterSpec spec)
    {
        switch (spec)
        {
            case Enums.CharacterSpec.Guardian:
                return Enums.CharacterSpec.Berserker;
            case Enums.CharacterSpec.Knight:
                return Enums.CharacterSpec.Cleric;
            case Enums.CharacterSpec.Cleric:
                return Enums.CharacterSpec.Knight;
            case Enums.CharacterSpec.Diviner:
                return Enums.CharacterSpec.Wizard;
            case Enums.CharacterSpec.Naturalist:
                return Enums.CharacterSpec.Elementalist;
            case Enums.CharacterSpec.Berserker:
                return Enums.CharacterSpec.Guardian;
            case Enums.CharacterSpec.Assassin:
                return Enums.CharacterSpec.Ranger;
            case Enums.CharacterSpec.Scourge:
                return Enums.CharacterSpec.Necromancer;
            case Enums.CharacterSpec.Ranger:
                return Enums.CharacterSpec.Assassin;
            case Enums.CharacterSpec.Wizard:
                return Enums.CharacterSpec.Diviner;
            case Enums.CharacterSpec.Elementalist:
                return Enums.CharacterSpec.Naturalist;
            case Enums.CharacterSpec.Necromancer:
            default:
                return Enums.CharacterSpec.Scourge;
        }
    }

    public static Enums.CharacterRole GetRoleFromSpec(Enums.CharacterSpec spec)
    {
        switch (spec)
        {
            case Enums.CharacterSpec.Guardian:
                return Enums.CharacterRole.Tank;
            case Enums.CharacterSpec.Knight:
                return Enums.CharacterRole.Tank;
            case Enums.CharacterSpec.Cleric:
                return Enums.CharacterRole.Healer;
            case Enums.CharacterSpec.Diviner:
                return Enums.CharacterRole.Healer;
            case Enums.CharacterSpec.Naturalist:
                return Enums.CharacterRole.Healer;
            case Enums.CharacterSpec.Berserker:
                return Enums.CharacterRole.MeleeDPS;
            case Enums.CharacterSpec.Assassin:
                return Enums.CharacterRole.MeleeDPS;
            case Enums.CharacterSpec.Scourge:
                return Enums.CharacterRole.MeleeDPS;
            case Enums.CharacterSpec.Ranger:
                return Enums.CharacterRole.RangedDPS;
            case Enums.CharacterSpec.Wizard:
                return Enums.CharacterRole.RangedDPS;
            case Enums.CharacterSpec.Elementalist:
                return Enums.CharacterRole.RangedDPS;
            default:
            case Enums.CharacterSpec.Necromancer:
                return Enums.CharacterRole.RangedDPS;
        }
    }

    public static Enums.CharacterClass GetClassFromSpec(Enums.CharacterSpec spec)
    {
        switch (spec)
        {
            case Enums.CharacterSpec.Guardian:
            case Enums.CharacterSpec.Berserker:
                return Enums.CharacterClass.Fighter;
            case Enums.CharacterSpec.Knight:
            case Enums.CharacterSpec.Cleric:
                return Enums.CharacterClass.Paladin;
            case Enums.CharacterSpec.Diviner:
            case Enums.CharacterSpec.Wizard:
                return Enums.CharacterClass.Sorcerer;
            case Enums.CharacterSpec.Naturalist:
            case Enums.CharacterSpec.Elementalist:
                return Enums.CharacterClass.Totemic;
            case Enums.CharacterSpec.Assassin:
            case Enums.CharacterSpec.Ranger:
                return Enums.CharacterClass.Shadow;
            case Enums.CharacterSpec.Scourge:
            case Enums.CharacterSpec.Necromancer:
                return Enums.CharacterClass.Occultist;
            default:
                return Enums.CharacterClass.Fighter;
        }
    }

    public static Enums.CharacterClass GenerateClassFromRole(Enums.CharacterRole role)
    {
        int randomValue = 0;
        switch (role)
        {
            case Enums.CharacterRole.Tank:
                randomValue = UnityEngine.Random.Range(0, 2);
                if (randomValue == 0)
                    return Enums.CharacterClass.Fighter;
                else
                    return Enums.CharacterClass.Paladin;
            case Enums.CharacterRole.MeleeDPS:
                randomValue = UnityEngine.Random.Range(0, 3);
                if (randomValue == 0)
                    return Enums.CharacterClass.Fighter;
                else if (randomValue == 1)
                    return Enums.CharacterClass.Shadow;
                else
                    return Enums.CharacterClass.Occultist;
            case Enums.CharacterRole.Healer:

                randomValue = UnityEngine.Random.Range(0, 3);
                if (randomValue == 0)
                    return Enums.CharacterClass.Totemic;
                else if (randomValue == 1)
                    return Enums.CharacterClass.Sorcerer;
                else
                    return Enums.CharacterClass.Paladin;

            default:
            case Enums.CharacterRole.RangedDPS:

                randomValue = UnityEngine.Random.Range(0, 4);
                if (randomValue == 0)
                    return Enums.CharacterClass.Totemic;
                else if (randomValue == 1)
                    return Enums.CharacterClass.Sorcerer;
                else if (randomValue == 2)
                    return Enums.CharacterClass.Shadow;
                else
                    return Enums.CharacterClass.Occultist;
        }
    }

    public static Enums.CharacterRole GenerateRoleFromClass(Enums.CharacterClass Class)
    {
        //implement this correctly later
        int randomValue = UnityEngine.Random.Range(0, 2);
        switch (Class)
        {
            case Enums.CharacterClass.Fighter:
                if (randomValue == 0)
                    return Enums.CharacterRole.Tank;
                else
                    return Enums.CharacterRole.MeleeDPS;

            case Enums.CharacterClass.Shadow:
                if (randomValue == 0)
                    return Enums.CharacterRole.RangedDPS;
                else
                    return Enums.CharacterRole.MeleeDPS;
            case Enums.CharacterClass.Totemic:
                if (randomValue == 0)
                    return Enums.CharacterRole.Healer;
                else
                    return Enums.CharacterRole.RangedDPS;
            case Enums.CharacterClass.Sorcerer:
                if (randomValue == 0)
                    return Enums.CharacterRole.Healer;
                else
                    return Enums.CharacterRole.RangedDPS;
            case Enums.CharacterClass.Paladin:
                if (randomValue == 0)
                    return Enums.CharacterRole.Healer;
                else
                    return Enums.CharacterRole.Tank;
            default:
            case Enums.CharacterClass.Occultist:
                if (randomValue == 0)
                    return Enums.CharacterRole.RangedDPS;
                else
                    return Enums.CharacterRole.MeleeDPS;
        }
    }

    public static Enums.CharacterSpec GetSpecFromRoleAndClass(Enums.CharacterClass Class, Enums.CharacterRole role)
    {
        switch (Class)
        {
            case Enums.CharacterClass.Fighter:
                if (role == Enums.CharacterRole.Tank)
                    return Enums.CharacterSpec.Guardian;
                else
                    return Enums.CharacterSpec.Berserker;
                

            case Enums.CharacterClass.Shadow:
                if (role == Enums.CharacterRole.RangedDPS)
                    return Enums.CharacterSpec.Ranger;
                else
                    return Enums.CharacterSpec.Assassin;
                

            case Enums.CharacterClass.Totemic:
                if (role == Enums.CharacterRole.Healer)
                    return Enums.CharacterSpec.Naturalist;
                else
                    return Enums.CharacterSpec.Elementalist;
                

            case Enums.CharacterClass.Sorcerer:
                if (role == Enums.CharacterRole.Healer)
                    return Enums.CharacterSpec.Diviner;
                else
                    return Enums.CharacterSpec.Wizard;
                

            case Enums.CharacterClass.Paladin:
                if (role == Enums.CharacterRole.Healer)
                    return Enums.CharacterSpec.Cleric;
                else
                    return Enums.CharacterSpec.Knight;
                

            case Enums.CharacterClass.Occultist:
                if (role == Enums.CharacterRole.MeleeDPS)
                    return Enums.CharacterSpec.Scourge;
                else
                    return Enums.CharacterSpec.Necromancer;
                

            default:
                return Enums.CharacterSpec.Berserker;

        }
    }
    #endregion

    #region Encounter related

    public static void SetCurrentEncounter(BaseEncounter e) { m_currentEncounter = e; }

    #endregion

    #region Varios
    public static string GetPercentIncreaseString(float multiplier)
    {
        int percentIncrease = Mathf.RoundToInt((multiplier - 1.0f) * 100.0f);

        return percentIncrease + "%";
    }
    #endregion
}
