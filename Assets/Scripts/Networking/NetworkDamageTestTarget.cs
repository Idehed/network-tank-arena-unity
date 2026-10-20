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
        public NetworkVariable<int> health = new NetworkVariable<int>(0,
            NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);
        public NetworkVariable<int> maxHealth = new NetworkVariable<int>(100,
            NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);

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
            health.Value = (int)System.Math.Max(0, (long)health.Value - amount);

            if (health.Value == 0)
            {
                Debug.Log("died");
                NetworkObject.Despawn(true); // Server owns the target; clients will see it disappear.
            }
        }

        public void Heal(int amount)
        {
            if (!IsSpawned || !IsServer || amount <= 0 || health.Value <= 0)
                return;

            health.Value = Mathf.Min(maxHealth.Value, health.Value + amount);
        }

        private void OnGUI()
        {
            if (!IsSpawned || !IsClient || !IsOwner)
                return;

            // Read current state, including the initial late-join value; no hit-event replay is needed.
            GUI.color = Color.red;
            Rect rect = new Rect(12f, 120f, 300f, 50f);
            GUI.Box(rect, $"Health: {Health}/{MaxHealth}");
        }
    }
}
