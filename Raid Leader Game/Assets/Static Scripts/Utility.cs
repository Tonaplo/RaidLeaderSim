using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    static List<string> names;

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

    public static float GetAttackBaseValue(Enums.CharacterAttack attack, Enums.AttackValueTypes type)
    {
        switch (attack)
        {
            case Enums.CharacterAttack.TankStrike:
                if (type == Enums.AttackValueTypes.CastTime)
                    return 1.0f;
                else if (type == Enums.AttackValueTypes.BaseDamageMultiplier)
                    return 0.5f;
                break;
            case Enums.CharacterAttack.RangedFireball:
                if (type == Enums.AttackValueTypes.CastTime)
                    return 2.0f;
                else if (type == Enums.AttackValueTypes.BaseDamageMultiplier)
                    return 2.9f;
                break;
            case Enums.CharacterAttack.MeleeStab:
                if (type == Enums.AttackValueTypes.CastTime)
                    return 0.5f;
                else if (type == Enums.AttackValueTypes.BaseDamageMultiplier)
                    return 0.7f;
                break;
            case Enums.CharacterAttack.HealerSmite:
                if (type == Enums.AttackValueTypes.CastTime)
                    return 1.5f;
                else if (type == Enums.AttackValueTypes.BaseDamageMultiplier)
                    return 0.3f;
                break;
            default:
                break;
        }

        Debug.LogError("No attack of type " + attack + " or base type " + type + " found!");
        return 1.0f;
    }

    public static Color GetColorFromClass(Enums.CharacterClass Class)
    {
        switch (Class)
        {
            case Enums.CharacterClass.Fighter:
                return Color.grey;
            case Enums.CharacterClass.Shadow:
                return Color.yellow;
            case Enums.CharacterClass.Totemic:
                return Color.green;
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
        switch (spec)
        {
            case Enums.CharacterSpec.Guardian:
            case Enums.CharacterSpec.Knight:
            case Enums.CharacterSpec.Cleric:
            case Enums.CharacterSpec.Diviner:
            case Enums.CharacterSpec.Naturalist:
            case Enums.CharacterSpec.Berserker:
            case Enums.CharacterSpec.Assassin:
            case Enums.CharacterSpec.Scourge:
            case Enums.CharacterSpec.Ranger:
            case Enums.CharacterSpec.Wizard:
            case Enums.CharacterSpec.Elementalist:
            case Enums.CharacterSpec.Necromancer:
                return "Cool description of the " + spec.ToString() + " is still being worked on";
            default:
                return "No spec found!";
        }
    }

    public static string GetAttackName(Enums.CharacterAttack attack) {
        switch (attack)
        {
            case Enums.CharacterAttack.TankStrike:
                return "Strike";
            case Enums.CharacterAttack.RangedFireball:
                return "Fireball";
            case Enums.CharacterAttack.MeleeStab:
                return "Stab";
            case Enums.CharacterAttack.HealerSmite:
                return "Smite";
            default:
                return "Undefined!";
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
}
