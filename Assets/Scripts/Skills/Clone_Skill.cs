using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;    //��¡ԭ��
    [SerializeField] private float cloneDuration;   //��¡����ʱ��
    [Space]

    [Header("Clone Attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;    ///�ж��Ƿ���Թ���

    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    //[SerializeField] private bool createCloneOnDashStart;
    //[SerializeField] private bool createCloneOnDashOver;
    //[SerializeField] private bool createCloneOnCounterAttack;

    [Header("Multiple  Clone")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDupicateClone;
    [SerializeField] private float chanceToDupicate;

    [Header("Crystal Instead Of Clone  ")]
    [SerializeField] private UI_SkillTreeSlot crystalInsteadUnlockButton;
    public bool crystalInsteadOfClone;

    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiClone);
        crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }

    #region unlock skill
    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }
    private void UnlockAggresiveClone()
    {
        if (aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
    }
    private void UnlockMultiClone()
    {
        if (multipleUnlockButton.unlocked)
        {
            canDupicateClone = true;
            attackMultiplier = multiCloneAttackMultiplier;
        }
    }
    private void UnlockCrystalInstead()
    {
        if (crystalInsteadUnlockButton.unlocked)
        {
            crystalInsteadOfClone = true;
        }
    }
    #endregion

    public void CreateClone(Transform _clonePosition, Vector3 _offset)  //�����¡λ��
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            //SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget(); 
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);     //�����µĿ�¡, ��¡ original ���󲢷��ؿ�¡����

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, 
            FindClosestEnemy(newClone.transform), canDupicateClone, chanceToDupicate, player, attackMultiplier);
        //����clone��λ��,ͬʱ���Կ�¡����ʱ��     Controller���ڿ�¡ԭ���ϵģ�������GetComponents
    }

    public void CreateCloneWithDelay(Transform _enemyTransform)     //���������һ����¡
    {
        //if (createCloneOnCounterAttack)
        StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    public IEnumerator CloneDelayCorotine(Transform _transform, Vector3 _offset)    //�ӳ�����
    {
        yield return new WaitForSeconds(.4f);
            CreateClone(_transform, _offset);
    }

    public override void CheckUnlock()
    {
        UnlockAggresiveClone();
        UnlockCloneAttack();
        UnlockCrystalInstead();
        UnlockMultiClone();
    }
}
