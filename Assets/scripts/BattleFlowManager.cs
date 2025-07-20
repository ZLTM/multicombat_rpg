using UnityEngine;
using UnityEngine.UI;
using c1a_proy.rpg.rpg.Assets.scripts;

public class BattleFlowManager : MonoBehaviour
{
    // Inspector-driven: asigna los combatientes y sus barras en el inspector
    public Button fightButton;
    public Button runButton;
    public int activeCombatantIndex = 0;
    public int activeRoomIndex = 0;
    public float resetDelay = 0f;
    public bool timersActive = true;

    [System.Serializable]
    public struct CombatantSlot
    {
        public int roomIndex;
        public bool isEnemy;
        public MonoBehaviour combatant;
        public Slider timerBar;
        public ICharacter Character => combatant as ICharacter;
    }

    [Header("Combatant Slots")]
    public CombatantSlot[] combatantSlots;

    void Start()
    {
    activeRoomIndex = 0; // Ensure room 1 is always the starting room
    fightButton.interactable = false;
    runButton.interactable = false;
    ResetActiveCombatantTimer();
    UpdateCombatantSliderVisibility();
    }

    void Update()
    {
        if (!timersActive) return;
        bool anyPlayerReady = false;
        for (int i = 0; i < combatantSlots.Length; i++)
        {
            var slot = combatantSlots[i];
            var character = slot.Character;
            if (character != null)
            {
                if (slot.timerBar != null && character.FillTime > 0)
                {
                    slot.timerBar.value = Mathf.Clamp01(character.ElapsedTime / character.FillTime);
                }
                // Only check player readiness for slots in the current room
                if (!slot.isEnemy && slot.roomIndex == activeRoomIndex && character.ElapsedTime >= character.FillTime)
                {
                    anyPlayerReady = true;
                }
                // Enemy auto-attack and slider reset for each enemy independently
                if (slot.isEnemy && character.ElapsedTime >= character.FillTime)
                {
                    character.ElapsedTime = 0f;
                    if (slot.timerBar != null)
                        slot.timerBar.value = 0f;
                    Debug.Log($"Enemy in room {slot.roomIndex} attacks!");
                }
            }
        }
        // Only enable buttons for current room
        fightButton.interactable = anyPlayerReady;
        runButton.interactable = anyPlayerReady;
    }

    public void OnFightButtonPressed()
    {
        // Find the player combatant for the current room
        for (int i = 0; i < combatantSlots.Length; i++)
        {
            var slot = combatantSlots[i];
            var character = slot.Character;
            if (character != null && !slot.isEnemy && slot.roomIndex == activeRoomIndex && character.ElapsedTime >= character.FillTime)
            {
                string msg = character.FightMessage ?? $"FIGHT{i + 1}";
                Debug.Log(msg);
                // Reset only this timer
                character.ElapsedTime = 0f;
                if (slot.timerBar != null)
                    slot.timerBar.value = 0f;
                AfterCombatantAction();
                break;
            }
        }
    }

    public void OnRunButtonPressed()
    {
        // Find the player combatant for the current room
        for (int i = 0; i < combatantSlots.Length; i++)
        {
            var slot = combatantSlots[i];
            var character = slot.Character;
            if (character != null && !slot.isEnemy && slot.roomIndex == activeRoomIndex && character.ElapsedTime >= character.FillTime)
            {
                string msg = character.RunMessage ?? $"RUN{i + 1}";
                Debug.Log(msg);
                // Reset only this timer
                character.ElapsedTime = 0f;
                if (slot.timerBar != null)
                    slot.timerBar.value = 0f;
                AfterCombatantAction();
                break;
            }
        }
    }

    private void AfterCombatantAction()
    {
    timersActive = false;
    fightButton.interactable = false;
    runButton.interactable = false;
    Invoke(nameof(ResetActiveCombatantTimer), resetDelay);
    }

    private void ResetActiveCombatantTimer()
    {
        // Only reset timer for player in current room
        for (int i = 0; i < combatantSlots.Length; i++)
        {
            var slot = combatantSlots[i];
            var character = slot.Character;
            if (character != null && !slot.isEnemy && slot.roomIndex == activeRoomIndex)
            {
                character.ElapsedTime = 0f;
                if (slot.timerBar != null)
                    slot.timerBar.value = 0f;
            }
        }
        timersActive = true;
        // Only update visibility for current room
        UpdateCombatantSliderVisibility();
        var actSlot = combatantSlots[activeCombatantIndex];
        var actCharacter = actSlot.Character;
        if (actCharacter != null && !actSlot.isEnemy && actSlot.roomIndex == activeRoomIndex && actCharacter.ElapsedTime >= actCharacter.FillTime)
        {
            fightButton.interactable = true;
            runButton.interactable = true;
        }
        else
        {
            fightButton.interactable = false;
            runButton.interactable = false;
        }
    }

    public void SetActiveCombatantIndex(int index)
    {
        if (combatantSlots != null && combatantSlots.Length > 0)
        {
            activeCombatantIndex = Mathf.Clamp(index, 0, combatantSlots.Length - 1);
            UpdateCombatantSliderVisibility();
        }
        else
        {
            Debug.LogError("combatantSlots is null or empty. Cannot set active combatant index.");
            activeCombatantIndex = 0;
        }
    }

    public void SetActiveRoomIndex(int roomIndex)
    {
        activeRoomIndex = roomIndex;
        UpdateCombatantSliderVisibility();
    }

    private void UpdateCombatantSliderVisibility()
    {
        for (int i = 0; i < combatantSlots.Length; i++)
        {
            var slot = combatantSlots[i];
            bool show = slot.roomIndex == activeRoomIndex;
            if (slot.timerBar != null)
                slot.timerBar.gameObject.SetActive(show);
        }
    }

    public void BeginBattle()
    {
        // Inicializa el flujo de batalla aquí
    }

    public ICharacter GetRandomPlayer()
    {
        // Example: return a random non-enemy character from combatantSlots
        var players = new System.Collections.Generic.List<ICharacter>();
        for (int i = 0; i < combatantSlots.Length; i++)
        {
            var c = combatantSlots[i].Character;
            if (c != null && !combatantSlots[i].isEnemy)
                players.Add(c);
        }
        if (players.Count == 0) return null;
        return players[Random.Range(0, players.Count)];
    }

    public void EndEnemyTurn()
    {
        // Implement logic to handle end of enemy turn here
        // For example, advance to next combatant or reset timers
    }
}
