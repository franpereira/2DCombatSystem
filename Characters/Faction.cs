using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Konrad.Characters
{
    /// <summary>
    /// Represents a group that a character can belong to.
    /// </summary>
    [CreateAssetMenu(fileName = "Faction_", menuName = "Characters/Faction Data")]
    public class Faction : ScriptableObject
    {
        [SerializeField] string factionName;
        public string Name => factionName;

        [SerializeField] List<Faction> enemies;
        public ReadOnlyCollection<Faction> Enemies => enemies.AsReadOnly();

        public bool AreEnemies(Faction other) => enemies.Contains(other);
    }
}