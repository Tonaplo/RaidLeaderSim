using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    static List<string> names;

    public static void Initialize() {
        names = new List<string>
        {
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
            "Phiagrolm",
            "Jella",
            "Zulab",
            "Lullosh",
            "Wardi",
            "Shynmo",
            "Vordoecy",
            "Shynronna",
            "Nigresseb",
            "Omulgra",
            "Vososseln",
            "Shernylni",
            "Wiolmoesa",
            "Phymrilma",
            "Phathmendolm",
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
            "Memnerdulm",
            "Orune",
            "Kondessub",
            "Oshasa",
            "Ocilmir",
            "Zidrurshe",
            "Werdossik",
            "Shanralla",
            "Phothimdaln",
            "Unarnel",
            "Worusos",
            "Sonmarnu",
            "Gylese",
            "Zyrliro",
            "Kedriathmac",
            "Chalnony",
            "Jirlutholm",
            "Phamnullo",
            "Tardino",
            "Rimenil"
        };

        PlayerData.Initialize();

    }

    public static void GetRandomCharacterName(ref List<string> outNames, int numNamesNeeded) {
        List<string> namePool = new List<string>(names);
        outNames = new List<string>();

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
            case Enums.CharacterClass.Sorcerous:
                return Color.cyan;
            case Enums.CharacterClass.Paladin:
                return Color.magenta;
            case Enums.CharacterClass.Occultist:
                return Color.red;
            default:
                return Color.red;
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
            case Enums.CharacterSpec.WitchDoctor:
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
                return Enums.CharacterSpec.WitchDoctor;
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
            case Enums.CharacterSpec.WitchDoctor:
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
}
