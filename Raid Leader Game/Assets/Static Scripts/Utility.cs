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
    }

    public static string GetRandomCharacterName() {
        int index = Random.Range(0, names.Count - 1);
        return names[index];
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
                    return 2.2f;
                else if (type == Enums.AttackValueTypes.BaseDamageMultiplier)
                    return 2.0f;
                break;
            case Enums.CharacterAttack.MeleeStab:
                if (type == Enums.AttackValueTypes.CastTime)
                    return 0.5f;
                else if (type == Enums.AttackValueTypes.BaseDamageMultiplier)
                    return 1.0f;
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
}
