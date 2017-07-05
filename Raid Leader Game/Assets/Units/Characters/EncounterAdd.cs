using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterAdd : BaseCharacter {
    
    Enums.EncounterAdds addType;

    public EncounterAdd(string _name, Enums.EncounterAdds _type)
        : base(_name)
    {
        addType = _type;
    }
}
