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
            
            if (!IsSpawned || !IsServer || isUsed)
                return;

            var target = other.gameObject.GetComponent<NetworkDamageTestTarget>();
            if(target == null)
            {
                return;
            }

            target.Heal(healthamount);
            isUsed = true;
            

            Debug.Log("Health kit used up, " + healthamount + " health restored.");
            NetworkObject.Despawn(true);
        }  
    }
}
