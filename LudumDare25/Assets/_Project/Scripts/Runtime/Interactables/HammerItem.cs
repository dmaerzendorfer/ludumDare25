
using UnityEngine;

namespace _Project.Scripts.Runtime.Interactables
{
    public class HammerItem : Item
    {
        protected override void HandleUsage(Vector3 worldPosTarget)
        {
            base.HandleUsage(worldPosTarget);
            //tween it to swipe and check for anything hit
        }
    }
}