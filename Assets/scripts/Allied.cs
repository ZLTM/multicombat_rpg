using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using c1a_proy.rpg.rpg.Assets.scripts;

namespace c1a_proy.rpg.rpg.Assets.scripts
{
	public class Allied : MonoBehaviour, ICharacter
	{
		[SerializeField] private string _characterName;
		[SerializeField] private int _maxHP;
		[SerializeField] private int _currentHP;
		[SerializeField] private int _speed;
		[SerializeField] private int strength;
		[SerializeField] private int defense;
		[SerializeField] private int power;
		[SerializeField] public List<Skill> skills;
		[SerializeField] private float fillTime;
		[SerializeField] private Slider timerBar;
		[SerializeField] private string fightMessage;
		[SerializeField] private string runMessage;
		private float elapsedTime;

		public string characterName { get { return _characterName; } }
		public int maxHP { get { return _maxHP; } }
		public int currentHP { get { return _currentHP; } }
		public int speed { get { return _speed; } }

		public void TakeDamage(int amount)
		{
			_currentHP = Mathf.Max(_currentHP - amount, 0);
		}

		public void Heal(int amount)
		{
			_currentHP = Mathf.Min(_currentHP + amount, _maxHP);
		}

		public bool IsAlive()
		{
			return _currentHP > 0;
		}

		// Additional properties for compatibility
		public float ElapsedTime { get => elapsedTime; set => elapsedTime = value; }
		public float FillTime { get => fillTime; set => fillTime = value; }
		public Slider TimerBar { get => timerBar; set => timerBar = value; }
		public bool IsEnemy => false;
		public string FightMessage { get => fightMessage; set => fightMessage = value; }
		public string RunMessage { get => runMessage; set => runMessage = value; }
		public int Strength => strength;
		public int Defense => defense;
		public int Power => power;
		public List<Skill> Skills => skills;

		public void TakeTurn()
		{
			// Implement player turn logic here
		}

		void Update()
		{
			ElapsedTime += Time.deltaTime;
			if (TimerBar != null && FillTime > 0)
				TimerBar.value = Mathf.Clamp01(ElapsedTime / FillTime);
		}
	}
}
