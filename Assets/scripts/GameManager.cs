using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Gestores principales")]
    public BattleFlowManager battleFlowManager;
    public RoomCameraManager roomCameraManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartBattle()
    {
        if (battleFlowManager != null)
            battleFlowManager.BeginBattle();
    }
}
