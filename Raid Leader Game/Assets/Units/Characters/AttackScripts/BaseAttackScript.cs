public class BaseAttackScript {

    protected float m_castTime;
    protected float m_baseMultiplier;
    protected string m_attackName;
    
    //used to Initialize the correct values
    public virtual void SetupAttack() {; }

    //Derived version will do the actual damage and such
    public virtual void StartFight(int index, Raider attacker, RaidSceneController rsc, RaiderScript rs) { ;    }

}
