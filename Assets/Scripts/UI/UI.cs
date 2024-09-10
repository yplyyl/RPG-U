using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager
{
    [Header("end screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI; 
    [SerializeField] private GameObject ingameUI;

    public UI_SkillToolTip skillToolTip;
    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;

    [SerializeField] public UI_VolumeSlider[] volumeSettings;
    private void Awake()
    {
        skillTreeUI.SetActive(true);
        //SwitchTo(skillTreeUI);  //Ϊ���ڼ��ܽű��Ϸ����¼�֮ǰ���ڼ���������Ϸ����¼���
        fadeScreen.gameObject.SetActive(true);
    }

    void Start()
    {
        //SwitchTo(null);
        SwitchTo(ingameUI);

        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
        //itemToolTip = GetComponentInChildren<UI_ItemToolTip>();    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(characterUI);

        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);

        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionsUI);
    }

    public void SwitchTo(GameObject _menu)      //�л����ں���
    {
        /*        if (_menu == null)
                {
                    Debug.LogError("Menu GameObject is null");
                    return;
                }*/

        for (int i = 0; i < transform.childCount; i++)      //��֤���ڵ��뵭��Ч���ĺ���ʱ�Ż�Ϊ�棬�Ż�ʹdarkScreen���ִ���
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;  //��Ҫ��������ֵ�����Ļ��Ϸ����Ļ�ԡ�
            if(fadeScreen == false)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
/*            if (AudioManager.instance == null)
            {
                Debug.Log("AudioManager.instance is not initialized.");
                return;
            }*/

            AudioManager.instance.PlaySfx(7, null);
            _menu.SetActive(true);
        }

        if (GameManager.instance != null)
        {
            if (_menu == ingameUI)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)   //�����л����ں���
    {
        if (_menu != null && _menu.activeSelf)  //ͨ���ж��Ƿ���mune��mune�Ƿ񼤻�������ʹ����Ϊ���ӻ򲻿�ʹ
        {
            _menu.SetActive(false);
            CheckForIngameUI();
            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForIngameUI() //������UI����ʱ�Զ��л�ֵInGameUI����
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
                return;
        }
        SwitchTo(ingameUI);
    }

    public void SwitchOnEndScreen() //�����ۺ�Ч������
    {
        //SwitchTo(null)
        //fadeScreen.gameObject.SetActive(true);
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorutione());
    }

    IEnumerator EndScreenCorutione()    //������ʾ�ı�����
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);
    }

    public void RestartGameButton() => GameManager.instance.RestartScene();     //�����ؿ�����

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string,float> pair in _data.volumeSettings)
        {
            foreach (UI_VolumeSlider item in volumeSettings)
            {
                if (item.parametr == pair.Key)
                    item.LoadSlider(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach (UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);
        }
    }
}
