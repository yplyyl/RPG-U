using System.Collections;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magiceResist,
    fireDamage,
    iceDamage,
    lightingDamage
}
public class Characterstats : MonoBehaviour
{
    //public int damage;
    //public int maxHealth;
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;       // 力量 增伤1点 临界值power 1% 物抗
    public Stat agility;        // 敏捷 闪避 1% 临界chance 1%
    public Stat intelligence;     // 1 点 魔法伤害 1点魔抗
    public Stat vitality;       //加血的

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;     // 暴击率
    public Stat critPower;      //150% 爆伤

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;        //闪避值
    public Stat magiceResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited;  // 持续烧伤
    public bool isChilled;  // 削弱护甲 20%
    public bool isShocked;  // 降低敌人命中率

    [SerializeField] private float ailmentsDuration = 4;
    private float igniteTimer;
    private float iceTimer;
    private float lightingTimer;

    private int igniteDamage;
    private float igniteDamageCoolDown = .3f;
    private float igniteDamageTimer;

    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;

    public int currentHealth;
    public bool isDead { get; private set; }
    public bool isInvincible { get; private set; }
    private bool isVulnerable;

    public System.Action onHealthChanged;   //使角色在Stat里调用UI层的函数

    protected virtual void Start()
    {
        critPower.SetDefaultValue(150);     //设置默认爆伤
        currentHealth = GetMaxHealthValue();
        fx = GetComponent<EntityFX>();

        //Debug.Log("character stats called");
        //example
        //damage.AddModifier(4);
    }

    protected virtual void Update()
    {
        //所有的状态都设置上默认持续时间，持续过了就结束状态
        igniteTimer -= Time.deltaTime;
        iceTimer -= Time.deltaTime;
        lightingTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (igniteTimer < 0)
            isIgnited = false;

        if (iceTimer < 0)
            isChilled = false;

        if (lightingTimer < 0)
            isShocked = false;

        if(isIgnited)
            ApplyIgniteDamage();
    }
    public void MakeVulnerableFor(float _duration)      //脆弱效果函数
    {
        StartCoroutine(VulnerableCorutine(_duration));
    }

    private IEnumerator VulnerableCorutine(float _duration)
    {
        isVulnerable = true;
        yield return new WaitForSeconds(_duration);
        isVulnerable = false;
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statModifier)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statModifier));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statModifier)
    {
        _statModifier.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statModifier.RemoveModifier(_modifier);
    }

    public virtual void DoDamage(Characterstats _targetStats)   //计算后造成伤害函数
    {
        bool criticalStrike = false;

        if (TargetCanAvoidAttack(_targetStats))  //设置闪避
            return;

        _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())      //爆伤设置
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            criticalStrike = true;
            //Debug.Log("toatl Crit damage " + totalDamage);
        }

        fx.CreateHitFX(_targetStats.transform, criticalStrike);

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);      //设置防御
        _targetStats.TakeDamage(totalDamage);

        DoMagicDamage(_targetStats); //remove if 
    }

    #region Magical damage and ailemnts
    public virtual void DoMagicDamage(Characterstats _targetStats)  //法伤计算
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightingDamage;
        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);

        _targetStats.TakeDamage(totalMagicDamage);

        //防止循环在所有元素伤害为0时出现死循环
        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
        {
            return;
        }

        AttemptyToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightingDamage);
    }

    private void AttemptyToApplyAilements(Characterstats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        //让元素效果取决最高的伤害
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        //防止出现元素伤害一致而导致无法触发元素效果
        //循环判断触发某个元素效果
        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .33f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("apply fire");
                return;
            }
            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("apply ice");
                return;
            }
            if (Random.value < .99f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("apply lighting");
                return;
            }
        }

        //给点燃伤害赋值
        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        //给伤害赋值
        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)   //判断异常状态
    {
        //if (isIgnited || isChilled || isShocked)
        //return;
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            igniteTimer = ailmentsDuration;

            fx.IgniteFxFor(ailmentsDuration);
        }
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            iceTimer = ailmentsDuration;

            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);

            fx.ChillFxFor(ailmentsDuration);
        }
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                HitNearestTargetWithShockStrike();
            }
        }
    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        isShocked = _shock;
        lightingTimer = ailmentsDuration;

        fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<Characterstats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        //被点燃后，出现多段伤害后点燃停止
        if (igniteDamageTimer < 0)
        {
            //Debug.Log("burn damage" + igniteDamage);
            //currentHealth -= igniteDamage;

            DecreaseHealthBy(igniteDamage);
            if (currentHealth < 0 && !isDead)
                Die();

            igniteDamageTimer = igniteDamageCoolDown;
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;   //灼烧伤害
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;   //shandian伤害
    #endregion

    public virtual void TakeDamage(int _damage)
    {
        if (isInvincible)
            return;

        //currentHealth -= _damage;
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");
        //Debug.Log(_damage);

        if (currentHealth < 0 && !isDead)
            Die();
    }

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void DecreaseHealthBy(int _damage)    //改变当前声明值，不调用特效
    {
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        currentHealth -= _damage;

        if (onHealthChanged != null)
            onHealthChanged();
    }

    public virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    public void MakeInvincible(bool _Invincible) => isInvincible = _Invincible;

    #region Stat calculations
    protected int CheckTargetArmor(Characterstats _targetStats, int totalDamage)  //设置防御
    {
        if (isChilled)  //被冰冻后，角色护甲减少
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();           
        }
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private int CheckTargetResistance(Characterstats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magiceResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }   //法抗计算

    public virtual void OnEvasion()
    {

    }

    protected bool TargetCanAvoidAttack(Characterstats _targetStats)   //设置闪避
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)       //被麻痹后敌人的闪避率提升
        {
            totalEvasion += 20;
        }

        if (Random.Range(0,100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }   

    protected bool CanCrit()
    {
        int totalCritChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCritChance)
        {
            return true;
        }
        return false;
    }   //判断是否暴击

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        //Debug.Log("total Crit power %"+ totalCritPower);

        float critDamage = _damage * totalCritPower;
        //Debug.Log("crit damage before round up"+ critDamage);

        return Mathf.RoundToInt(critDamage);    //返回舍入为最近整数的
    }   //计算暴击后伤害

    public int GetMaxHealthValue()  //统计生命值函数
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    #endregion

    public Stat GetStat(StatType _statType)
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelligence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magiceResist) return magiceResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightingDamage) return lightingDamage;

        return null;
    }
}
