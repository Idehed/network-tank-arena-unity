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


        private void OnCollisionEnter(Collision collision)
        {
            
            if (!IsSpawned || !IsServer)
                return;
            if (collision.gameObject.CompareTag("Player"))
            {
                var target = collision.gameObject.GetComponent<NetworkDamageTestTarget>();
                if(target == null)
                {
                    return;
                }

                target.Heal(healthamount);
                isUsed = true;

                Debug.Log("Health kit used up, " + healthamount + " health restored.");
                NetworkObject.Despawn(true);
                
                StartCoroutine(RespawnKit());


            }


        }

        IEnumerator RespawnKit()
        {
            var local = gameObject.GetComponent<LocalNetworkSession>();
            // Server owns the target; clients will see it disappear.
            yield return new WaitForSeconds(3.0f);
            Instantiate(local.healthTargetPrefab, local.healthTargetSpawn.position, Quaternion.Euler(0f, local.healthTargetSpawn.eulerAngles.y, 0f));
            NetworkObject.Spawn(true);
        }
    }
}
