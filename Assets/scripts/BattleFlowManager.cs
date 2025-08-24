using UnityEngine;
using UnityEngine.UI;
using c1a_proy.rpg.rpg.Assets.scripts;

public class BattleFlowManager : MonoBehaviour
{
    public enum PlayerAction { Fight, Run }
    [System.Serializable]
    public struct RoomButtons { public Button fight; public Button run; }
    [Header("Optional: Buttons por sala (index = room)")]
    public RoomButtons[] roomButtons;
    public int activeRoomIndex = 0;
    public float resetDelay = 0f;
    public bool timersActive = true;

    [System.Serializable]
    public struct CombatantSlot
    {
        public int roomIndex;
        public bool isEnemy;
        public MonoBehaviour combatant;
        public ICharacter Character => combatant as ICharacter;
    }

    [Header("Combatant Slots")]
    public CombatantSlot[] combatantSlots;

    void Start()
    {
    activeRoomIndex = 0;
    ResetActiveCombatantTimer();
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
                if (!slot.isEnemy && slot.roomIndex == activeRoomIndex && character.ElapsedTime >= character.FillTime)
                {
                    anyPlayerReady = true;
                }
                if (slot.isEnemy && character.ElapsedTime >= character.FillTime)
                {
                    // Imprimir mensaje de ataque del enemigo
                    Debug.Log($"[ENEMY] {character.FightMessage} (room {slot.roomIndex}, slot {i})");
                    character.ElapsedTime = 0f;
                    // Actualizar slider del enemigo si tiene CharacterUIBinder
                    var binder = (character as MonoBehaviour)?.GetComponent<CharacterUIBinder>();
                    if (binder != null)
                    {
                        binder.RefreshAll();
                    }
                }
            }
        }
        if (roomButtons != null && roomButtons.Length > 0)
        {
            for (int r = 0; r < roomButtons.Length; r++)
            {
                var rb = roomButtons[r];
                bool enable = (r == activeRoomIndex) && anyPlayerReady;
                if (rb.fight) rb.fight.interactable = enable;
                if (rb.run) rb.run.interactable = enable;
            }
        }
    }

    public void OnFightButtonPressed()
    {
    TryExecutePlayerActionInRoom(activeRoomIndex, PlayerAction.Fight);
    }

    public void OnRunButtonPressed()
    {
    TryExecutePlayerActionInRoom(activeRoomIndex, PlayerAction.Run);
    }

    private void AfterCombatantAction()
    {
        timersActive = false;
        if (roomButtons != null && roomButtons.Length > 0)
        {
            for (int r = 0; r < roomButtons.Length; r++)
            {
                var rb = roomButtons[r];
                if (rb.fight) rb.fight.interactable = false;
                if (rb.run) rb.run.interactable = false;
            }
        }
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
                // Forzar actualización del slider si hay CharacterUIBinder
                var binder = (character as MonoBehaviour)?.GetComponent<CharacterUIBinder>();
                if (binder != null)
                {
                    binder.RefreshAll();
                }
            }
        }
        timersActive = true;
        if (roomButtons != null && roomButtons.Length > 0)
        {
            for (int r = 0; r < roomButtons.Length; r++)
            {
                var rb = roomButtons[r];
                if (rb.fight) rb.fight.interactable = false;
                if (rb.run) rb.run.interactable = false;
            }
        }
    }

    public void SetActiveRoomIndex(int roomIndex)
    {
        activeRoomIndex = roomIndex;
    }

    public void BeginBattle()
    {
    }

    public ICharacter GetRandomPlayer()
    {
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

    public bool IsAnyPlayerReadyInRoom(int roomIndex)
    {
        for (int i = 0; i < combatantSlots.Length; i++)
        {
            var slot = combatantSlots[i];
            var character = slot.Character;
            if (character != null && !slot.isEnemy && slot.roomIndex == roomIndex && character.ElapsedTime >= character.FillTime)
                return true;
        }
        return false;
    }

    public bool TryExecutePlayerActionInRoom(int roomIndex, PlayerAction action)
    {
        for (int i = 0; i < combatantSlots.Length; i++)
        {
            var slot = combatantSlots[i];
            var character = slot.Character;
            if (character == null || slot.isEnemy || slot.roomIndex != roomIndex) continue;
            if (character.ElapsedTime < character.FillTime) continue;

            string actorName = string.IsNullOrEmpty(character.characterName) ? $"Player{i}" : character.characterName;
            string verb = action == PlayerAction.Fight
                ? (string.IsNullOrEmpty(character.FightMessage) ? "FIGHT" : character.FightMessage)
                : (string.IsNullOrEmpty(character.RunMessage) ? "RUN" : character.RunMessage);
            Debug.Log($"[{actorName}] {verb} (room {roomIndex}, slot {i})");
            character.ElapsedTime = 0f;
            AfterCombatantAction();
            return true;
        }
        return false;
    }
}
