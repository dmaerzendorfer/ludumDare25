using UnityEngine;

namespace _Project.Scripts.Runtime.Utility
{
    public class RandomPointInCollider
    {
        Collider2D collider;
        Collider2D noZone;
        Vector3 minBound;
        Vector3 maxBound;
        int maxInNoZone;

        private int _currInNoZone = 0;

        public RandomPointInCollider(Collider2D collider, Collider2D noZone, int maxInNoZone)
        {
            this.collider = collider;
            this.noZone = noZone;
            this.maxInNoZone = maxInNoZone;
            this.minBound = collider.bounds.min;
            this.maxBound = collider.bounds.max;
        }
        public void ResetNoZoneCount()
        {
            _currInNoZone=0;
        }

        public Vector3 RandomPoint()
        {
            Vector3 randomPoint;

            do
            {
                randomPoint =
                    new Vector3(
                        Random.Range(minBound.x, maxBound.x),
                        Random.Range(minBound.y, maxBound.y),
                        Random.Range(minBound.z, maxBound.z)
                    );
                if (noZone.OverlapPoint((randomPoint)))
                {
                    if (_currInNoZone == maxInNoZone)
                        continue;
                    _currInNoZone++;
                }
            } while (!collider.OverlapPoint(randomPoint));

            return randomPoint;
        }
    }
}