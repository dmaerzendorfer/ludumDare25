using UnityEngine;

namespace _Project.Scripts.Runtime.Utility
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererUpdater : MonoBehaviour
    {
        public Transform from;

        public Transform to;

        private LineRenderer _lineRenderer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
        }

        // Update is called once per frame
        void Update()
        {
            _lineRenderer.SetPosition(0, from.position);
            _lineRenderer.SetPosition(1, to.position);
        }
    }
}