using System.Diagnostics;
using UnityEngine;

namespace _Project.Scripts.Runtime.Utility
{
    public class DrawLineBetweenTwoObjects : MonoBehaviour
    {
        public GameObject Target = null;
 
        public float LineThickness = 10f;
 
        public float OverrideLineThickness = -1f;
 
        public float EndLineThickness => OverrideLineThickness > -1f ? OverrideLineThickness : LineThickness;
 
        public Color LineColor = Color.white;
 
        public Material LineMaterial;
 
        public Transform OverridereflectionProbe = null;
 
        private GameObject line;
 
        private LineRenderer _renderer;
 
        long maxTimeout = 1000;
 
        private Stopwatch watch = Stopwatch.StartNew();
 
        private void CreateLineRenderer()
        {
            // try to find the target
            if (Target == null)
            {
                // if the target hasn't spawned yet return
                if (Target == null)
                {
                    return;
                }
            }
 
            line = new GameObject
            {
                name = $"Line({name})"
            };
            _renderer = line.AddComponent<LineRenderer>();
            _renderer.textureMode = LineTextureMode.Tile;
            _renderer.alignment = LineAlignment.View;
            _renderer.material = LineMaterial;
            _renderer.startWidth = LineThickness;
            _renderer.endWidth = EndLineThickness;
            _renderer.startColor = LineColor;
            _renderer.endColor = LineColor;
            _renderer.generateLightingData = true;
            _renderer.SetPositions(new Vector3[] { gameObject.transform.position, Target.transform.position });
            _renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;
            _renderer.probeAnchor = OverridereflectionProbe;
            float width = _renderer.endWidth;
            _renderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);
        }
 
        private void OnDestroy()
        {
            Destroy(line);
        }
 
        private void OnDisable()
        {
            if (line != null)
            {
                line?.SetActive(false);
            }
        }
 
        private void OnEnable()
        {
            if (line != null)
            {
                line?.SetActive(true);
            }
        }
 
        private void Start()
        {
            CreateLineRenderer();
        }
 
        private void Update()
        {
            // this script may run when the target hasn't spawned yet or has been destroyed we shouldnt attempt to keep spawning the renderer at all times.
            if (_renderer == null && watch.ElapsedMilliseconds > maxTimeout)
            {
                CreateLineRenderer();
                watch.Restart();
            }
            else
            {
                if (gameObject && Target)
                {
                    _renderer?.SetPositions(new Vector3[] { gameObject.transform.position, Target.transform.position });
                }
                else
                {
                    Destroy(line);
                    _renderer = null;
                }
            }
        }
    }
}