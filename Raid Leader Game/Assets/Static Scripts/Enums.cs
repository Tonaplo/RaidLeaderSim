using UnityEngine;
using System.Collections;

public static class Enums{

    public enum CharacterRole { 
        Healer,
        DamageDealer
    }

    public enum CharacterFlags { 
        CHARACTER_FLAG_IS_MOVING = 1 << 0,
    }
}
