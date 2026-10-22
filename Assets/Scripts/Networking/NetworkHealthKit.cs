using NetworkTankArena.Combat;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace NetworkTankArena.Networking
{
    /// <summary>Persistent server result for the combat lab. No health or destruction rules.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkObject))]
    public sealed class NetworkHealthKit : NetworkBehaviour
    {
        public int healthamount = 25;
        public bool isUsed = false;

       


        private void OnTriggerEnter(Collider other)
        {
            
            if (!IsSpawned || !IsServer) return;

            var target = other.gameObject.GetComponent<NetworkDamageTestTarget>();
            if(target == null) return;
            

            //isUsed = true;
            target.Heal(healthamount);
            
            

            Debug.Log($"[Server] Kit {NetworkObjectId} picked up by client {target.OwnerClientId} " +
                  $"on frame {Time.frameCount}, fixed time {Time.fixedTime:F3}", this);
            NetworkObject.Despawn(true);
        }  
    }
}
