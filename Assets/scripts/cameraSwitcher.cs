using UnityEngine;
using UnityEngine.InputSystem;
using c1a_proy.rpg.rpg.Assets.scripts;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cámaras")]
    [SerializeField] public Camera[] cameras;
    [Header("Gestor de flujo de batalla")]
    public BattleFlowManager battleFlowManager;
    private InputAction[] camActions;
    private int currentIndex = 0;
    [Header("Acciones de entrada")]
    [SerializeField] public InputActionAsset inputActions;

    private void Start()
    {
        // Force only camera 0 enabled at start, others disabled
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].enabled = (i == 0);
        }
        if (battleFlowManager != null)
            battleFlowManager.SetActiveRoomIndex(0);
    }

    private void OnEnable()
    {
        var cameraMap = inputActions.FindActionMap("Camera");
        camActions = new InputAction[cameras.Length];
        for (int i = 0; i < cameras.Length; i++)
        {
            string actionName = $"ActivateCam{i + 1}";
            camActions[i] = cameraMap.FindAction(actionName);
            int idx = i;
            camActions[i].performed += ctx => ShowCamera(idx);
            camActions[i].Enable();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < camActions.Length; i++)
        {
            camActions[i].performed -= ctx => ShowCamera(i);
            camActions[i].Disable();
        }
    }

    public void ShowCamera(int index)
    {
        currentIndex = index;
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].enabled = (i == index);
        }
        if (battleFlowManager != null)
            battleFlowManager.SetActiveRoomIndex(index);
    }
}