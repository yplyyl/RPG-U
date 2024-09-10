using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //���ڳ����Ĳ���

public class GameManager : MonoBehaviour,ISaveManager
{
    private Transform player;

    public static GameManager instance;
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckPointId;

    [Header("Lost currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyY;
    [SerializeField] private float lostCurrencyX;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>();

        player = PlayerManager.instance.player.transform;
    }

    public void RestartScene()      //�����ؿ�����
    {
        SaveManager.instance.SaveData();
        Scene scene = SceneManager.GetActiveScene();    //��ó�ʼ����
        SceneManager.LoadScene(scene.name);     //��ȡ�ĳ�������ͨ���ַ�������
    }

    public void LoadData(GameData _data)
    {
        StartCoroutine(LoseWithDelay(_data));
    }

    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (pair.Key == checkpoint.checkpointId && pair.Value == true)
                    checkpoint.ActivateCheckpoint();
            }
        }
    }

    private void LoadLostCurrency(GameData _data)       //�������Լ񵽵�Ǯʬ�庯��
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (lostCurrencyAmount > 0)
        {
            GameObject newLossCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLossCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    private IEnumerator LoseWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        LoadClosestCheckpoint(_data);
        LoadLostCurrency(_data);
    }

    private void LoadClosestCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointId == null)
            return;

        closestCheckPointId = _data.closestCheckpointId;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint.checkpointId == closestCheckPointId)
                player.position = checkpoint.transform.position;
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;

        if(FindClosestCheckPoint() != null)
            _data.closestCheckpointId = FindClosestCheckPoint().checkpointId;

        _data.checkpoints.Clear();

        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.checkpointId, checkpoint.activationStatus);
        }
    }

    private Checkpoint FindClosestCheckPoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);
            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }
        return closestCheckpoint;
    }

    public void PauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
