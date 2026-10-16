using NetworkTankArena.Combat;
using Unity.Netcode;
using UnityEngine;

namespace NetworkTankArena.Networking
{
    /// <summary>Persistent server result for the combat lab. No health or destruction rules.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkObject))]
    public sealed class NetworkDamageTestTarget : NetworkBehaviour, IDamageable
    {
        private readonly NetworkVariable<int> hitCount = new NetworkVariable<int>(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> accumulatedDamage = new NetworkVariable<int>(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> health = new NetworkVariable<int>(0,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> maxHealth = new NetworkVariable<int>(100,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public int HitCount => hitCount.Value;
        public int AccumulatedDamage => accumulatedDamage.Value;

        public int Health => health.Value;

        
        public int MaxHealth => maxHealth.Value;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                health.Value = maxHealth.Value;
            }
        }
        public void ApplyDamage(int amount, GameObject source)
        {
            if (!IsSpawned || !IsServer || !isActiveAndEnabled || amount <= 0)
                return;
            if (health.Value <= 0)
            {
                return;
            }
                

            // Keep the teaching counters nonnegative even in a very long-running session.
            hitCount.Value = (int)System.Math.Min(int.MaxValue, (long)hitCount.Value + 1);
            accumulatedDamage.Value = (int)System.Math.Min(int.MaxValue, (long)accumulatedDamage.Value + amount);
            health.Value = (int)System.Math.Max(0, (long)health.Value - amount);

            if(health.Value == 0)
            {
                Debug.Log("died");
                NetworkObject.Despawn(true); // Server owns the target; clients will see it disappear.
            }
        }

        private void OnGUI()
        {
            if (!IsSpawned || !IsClient)
                return;

            // Read current state, including the initial late-join value; no hit-event replay is needed.
            GUI.Box(new Rect(12f, 170f, 300f, 50f),
                $"Damage test target\nHits: {HitCount}   Damage: {AccumulatedDamage}\nHealth: {Health}/{MaxHealth}");
        }
    }
}
