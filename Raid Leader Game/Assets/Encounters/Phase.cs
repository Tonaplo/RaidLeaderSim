using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase : MonoBehaviour {
    Enums.Difficulties difficulty = Enums.Difficulties.Normal;
    List<EncounterAbility> encounterAbilities = new List<EncounterAbility>();
    List<BaseCooldown> encounterCooldowns = new List<BaseCooldown>();
}
