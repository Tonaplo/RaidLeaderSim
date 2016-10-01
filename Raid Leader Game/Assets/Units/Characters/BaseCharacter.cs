using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaseCharacter : MonoBehaviour {

    protected string name;
    protected int baseHealth = 100;
    protected int health = 0;
    protected int maxHealth = 100;
    protected int gearLevel = 0;
    protected int skillLevel = 0;
    protected Enums.CharacterRole role;
    protected int movementModifier;
    protected int flags;
    protected Text text;

    public int GetHealth() { return health; }
    public int GetGearLevel() { return gearLevel; }
    public int GetSkillLevel() { return skillLevel; }
    public Enums.CharacterRole GetRole() { return role; }
    int GetMovementModifier() { return movementModifier; }

    void SetText() {
      //  text.text = name + "\n" + GetHealth().ToString() + "/" + maxHealth.ToString();
    }

	// Use this for initialization
	void Start () {
        RecomputeMaxHealth();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RecomputeMaxHealth() {
        maxHealth = (int)(baseHealth * (1 + ((float)gearLevel * .01f)));
    }

    public void ResetHealthToMax() {
        RecomputeMaxHealth();
        health = maxHealth;
    }

    public void ModifyHealth(int amount) {
        health += amount;

        if (health > maxHealth)
            health = maxHealth;

        if (health < 0)
        {
            health = 0;
            OnDeath();
            this.gameObject.SetActive(false);
        }

        SetText();
    }

    public void ModifySkillLevel(int amount) {
        skillLevel += amount;
    }

    public void ModifyGearLevel(int amount) {
        gearLevel += amount;
    }

    public int ComputeThroughput(int amount) {
        float floatAmount = (float)amount;
        int increase = GetGearLevel() + GetSkillLevel();
        floatAmount *= (1.0f + ((float)increase/100.0f));

        if (IsFlagSet(Enums.CharacterFlags.CHARACTER_FLAG_IS_MOVING))
            floatAmount *= ((float)GetMovementModifier() / 100.0f);

        return (int)floatAmount;
    }

    public bool IsFlagSet(Enums.CharacterFlags flag) {
        return ((int)flag & flags) != 0; 
    }

    public void SetFlag(Enums.CharacterFlags flag, bool set) {
        if(set)
            flags |= (int)flag;
        else
            flags &= ~(int)flag;
    }

    //Must be overridden by each type;
    public virtual void OnDeath() {}
}
