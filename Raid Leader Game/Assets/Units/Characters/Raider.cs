using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raider : BaseCharacter {

    
    RaiderStats stats;
    public RaiderStats RaiderStats() { return stats; }

    public Raider(string _name, RaiderStats _stats) : base(_name) {
        stats = _stats;
    }

    //Calculate Max Health based on Class
    public override void CalculateMaxHealth()
    {
        SetBaseHealth((int)Enums.StaticValues.baseRaiderHealth);
        switch (stats.GetRole())
        {
            case Enums.CharacterRole.Tank:
                SetMaxHealth((int)Enums.StaticValues.baseRaiderHealth * 2);
                break;
            case Enums.CharacterRole.Healer:
                SetMaxHealth((int)((int)Enums.StaticValues.baseRaiderHealth * 0.8));
                break;
            case Enums.CharacterRole.RangedDPS:
                SetMaxHealth((int)((int)Enums.StaticValues.baseRaiderHealth * 0.9));
                break;
            case Enums.CharacterRole.MeleeDPS:
                SetMaxHealth((int)((int)Enums.StaticValues.baseRaiderHealth * 1.1));
                break;
            default:
                break;
        }
    }
}
