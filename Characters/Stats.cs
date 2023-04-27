using UnityEngine;

namespace Konrad.Characters
{
    public class Stats : MonoBehaviour
    {
        public int Strength = 5;
        public int Dexterity = 5;
        public int Constitution = 5;
        public int Intelligence = 5;

        int _attack;
        public int Attack { get => _attack; set => _attack = Strength + value; }
        int _defense;
        public int Defense { get => _defense; set => _defense = Constitution + value; }
    }
}