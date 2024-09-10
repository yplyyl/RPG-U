using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity; 
    [SerializeField] private float bounceSpeed;

    [Header("Pierce info")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCooldown = .35f;
    [SerializeField] private float maxTraveDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;

    [Header("Skill info")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefab; 
    [SerializeField] private Vector2 launchForce; 
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Passive skills")]
    [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlocked { get; private set; }
    [SerializeField] private UI_SkillTreeSlot vulnurableUnlockButton;
    public bool vulnerableUnlocked { get; private set; }

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots; 
    [SerializeField] private float spaceBeetwenDots; 
    [SerializeField] private GameObject dotPrefab; 
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        GenereateDots();
        SetupGravity();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        vulnurableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockVulnerable);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounce);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierce);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpin);
    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        //base.Update();

        if (Input.GetKeyDown(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);


        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBeetwenDots);
            }
        }

    }

    public void CreatSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
        {
            newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
        }
        else if(swordType == SwordType.Pierce)
        {
            newSwordScript.SetupPierce(pierceAmount);
        }
        else if (swordType == SwordType.Spin)
        {
            newSwordScript.SetupSpin(true, maxTraveDistance, spinDuration, hitCooldown);
        }

        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Unlock skill
    private void UnlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked)
            timeStopUnlocked = true;
    }
    private void UnlockVulnerable()
    {
        if (vulnurableUnlockButton.unlocked)
            vulnerableUnlocked = true;
    }
    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked)
        {
            swordUnlocked = true;
            swordType = SwordType.Regular;
        }
    }
    private void UnlockBounce()
    {
        if (bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }
    private void UnlockPierce()
    {
        if (pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }
    private void UnlockSpin()
    {
        if (spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }
    #endregion

    #region Aim region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenereateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y) * t
            + .5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }
    #endregion

    public override void CheckUnlock()
    {
        UnlockSword();
        UnlockBounce();
        UnlockPierce();
        UnlockSpin();
        UnlockVulnerable();
        UnlockTimeStop();
    }
}
