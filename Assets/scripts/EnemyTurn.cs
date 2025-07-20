using UnityEngine;
using System.Collections;
using c1a_proy.rpg.rpg.Assets.scripts;

namespace c1a_proy.rpg.rpg.Assets.scripts
{
    public class EnemyTurn : MonoBehaviour
    {
        [Header("Current Enemy")]
        public Enemy enemy;
        [Header("Battle Flow Manager")]
        public BattleFlowManager battleFlowManager;

        public void ExecuteTurn()
        {
            if (enemy != null && enemy.IsAlive())
            {
                // Skill selection and target (simplified)
                Skill skill = enemy.skills.Count > 0 ? enemy.skills[0] : null;
                ICharacter target = battleFlowManager.GetRandomPlayer();
                if (skill != null && target != null)
                {
                    target.TakeDamage(skill.damage);
                }
            }
            battleFlowManager.EndEnemyTurn();
        }
    }
}
