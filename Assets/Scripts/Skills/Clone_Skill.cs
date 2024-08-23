using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;    //��¡ԭ��
    [SerializeField] private float cloneDuration;   //��¡����ʱ��
    [Space]
    [SerializeField] private bool canAttack;    ///�ж��Ƿ���Թ���

    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField] private bool createCloneOnCounterAttack;

    [Header("Clone can Dupicate")]
    [SerializeField] private bool canDupicateClone;
    [SerializeField] private float chanceToDupicate;

    [Header("Clone can Dupicate")]
    public bool crystalInsteadOfClone;

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
            FindClosestEnemy(newClone.transform), canDupicateClone, chanceToDupicate);
        //����clone��λ��,ͬʱ���Կ�¡����ʱ��     Controller���ڿ�¡ԭ���ϵģ�������GetComponents
    }

    //�ó���������Ŀ�¡�ڿ�ʼ�ͽ�������һ��
    public void CreateCloneOnDashStart()    
    {
        if (createCloneOnDashStart)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnDashOver()
    {
        if (createCloneOnDashOver)
            CreateClone(player.transform, Vector3.zero);
    }

    //���������һ����¡
    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (createCloneOnCounterAttack)
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    //�ӳ�����
    public IEnumerator CreateCloneWithDelay(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
            CreateClone(_transform, _offset);
    }
}
