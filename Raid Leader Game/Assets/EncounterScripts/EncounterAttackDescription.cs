using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterAttackDescription {

    string m_attackName;
    string m_attackDescription;
    List<Enums.CharacterRole> m_targetTypes = new List<Enums.CharacterRole>();

    public string AttackName {get { return m_attackName; } }
    public string AttackDescription { get { return m_attackDescription; } }
    public List<Enums.CharacterRole> TargetTypes { get { return m_targetTypes; } }

    public EncounterAttackDescription(List<Enums.CharacterRole> targets, string name, string description)
    {
        m_attackName = name;
        m_attackDescription = description;
        m_targetTypes = targets;
    }
}
