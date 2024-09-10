using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{

    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot cloneInsteadUnlockButton;
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Crystal simple")]
    [SerializeField] private UI_SkillTreeSlot crystalUnlockButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot explosiveUnlockButton;
    [SerializeField] private bool canExplode;

    [Header("Move crystal")]
    [SerializeField] private UI_SkillTreeSlot movingUnlockButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi Stack crystal")]
    [SerializeField] private UI_SkillTreeSlot multiStackUnlockButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>(); //ˮ���б�

    protected override void Start()
    {
        base.Start();

        crystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        cloneInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalmirage);
        explosiveUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
        movingUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMoveCrystal);
        multiStackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiStackCrystal);
    }

    #region Unlock skill 
    private void UnlockCrystal()
    {
        if (crystalUnlockButton.unlocked)
            crystalUnlocked = true;
    }
    private void UnlockCrystalmirage()
    {
        if (cloneInsteadUnlockButton.unlocked)
            cloneInsteadOfCrystal = true;
    }
    private void UnlockExplosiveCrystal()
    {
        if (explosiveUnlockButton.unlocked)
            canExplode = true;
    }
    private void UnlockMoveCrystal()
    {
        if (movingUnlockButton.unlocked)
            canMoveToEnemy = true;
    }
    private void UnlockMultiStackCrystal()
    {
        if (multiStackUnlockButton.unlocked)
            canUseMultiStacks = true;
    }
    #endregion

    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else
        {
            //���������ˮ�������ƶ�ʱ˲��
            if (canMoveToEnemy)
                return;

            //��ըǰ���ɫ����λ��
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }

        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);

        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform), player);
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal()   //��List��Ķ���ʵ��������
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWindow);  // �����Զ�����ˮ������

                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform), player);

                //��ˮ��������������ȴʱ���ʹ�ò���ˮ��
                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }

                return true;
            }

        }
        return false;
    }

    private void RefilCrystal() //��List���Prefab����
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void ResetAbility() //�Զ�����ˮ������
    {
        if (cooldown > 0)
            return;

        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }

    public override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalmirage();
        UnlockExplosiveCrystal();
        UnlockMoveCrystal();
        UnlockMultiStackCrystal();
    }
}
