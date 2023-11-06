using Fusion;
using UnityEngine;

namespace MMSGP.Collectables
{
    public class CollectableBase : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(NetworkIsCollectedChanged))]
        public NetworkBool isCollected { get; set; } = false;

        protected static void NetworkIsCollectedChanged(Changed<CollectableBase> changed)
        {
            if (changed.Behaviour.isCollected)
            {
                changed.Behaviour.Collected();
                changed.Behaviour.gameObject.SetActive(false);
            }
        }
        
        public void SetIsCollected(bool isCollected)
        {
            this.isCollected = isCollected;
        }

        protected virtual void Collected()
        {
            
        }
    }
}