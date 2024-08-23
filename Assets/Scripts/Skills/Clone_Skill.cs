using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;    //克隆原型
    [SerializeField] private float cloneDuration;   //克隆持续时间
    [Space]
    [SerializeField] private bool canAttack;    ///判断是否可以攻击

    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField] private bool createCloneOnCounterAttack;

    [Header("Clone can Dupicate")]
    [SerializeField] private bool canDupicateClone;
    [SerializeField] private float chanceToDupicate;

    [Header("Clone can Dupicate")]
    public bool crystalInsteadOfClone;

    public void CreateClone(Transform _clonePosition, Vector3 _offset)  //传入克隆位置
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            //SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget(); 
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);     //创建新的克隆, 克隆 original 对象并返回克隆对象。

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, 
            FindClosestEnemy(newClone.transform), canDupicateClone, chanceToDupicate);
        //调试clone的位置,同时调试克隆持续时间     Controller绑在克隆原型上的，所以用GetComponents
    }

    //让冲刺留下来的克隆在开始和结束各有一个
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

    //反击后产生一个克隆
    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (createCloneOnCounterAttack)
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    //延迟生成
    public IEnumerator CreateCloneWithDelay(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
            CreateClone(_transform, _offset);
    }
}
