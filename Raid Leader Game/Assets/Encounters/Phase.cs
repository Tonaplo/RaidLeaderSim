using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase : MonoBehaviour {
    int baseAoEDPSNeeded = 0;
    int baseSingleTargetDPSNeeded = 0;
    int baseAoEHealingNeeded = 0;
    int baseSingleTargetHealingNeeded = 0;
    int actualAoEDPSNeeded = 0;
    int actualSingleTargetDPSNeeded = 0;
    int actualAoEHealingNeeded = 0;
    int actualSingleTargetHealingNeeded = 0;
    Enums.Difficulties difficulty = Enums.Difficulties.Normal;
    List<EncounterAbility> encounterAbilities = new List<EncounterAbility>();
    List<BaseCooldown> encounterCooldowns = new List<BaseCooldown>();
}
