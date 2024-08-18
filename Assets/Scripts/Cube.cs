using UnityEngine;

namespace PunctualSolutions.UnityPackageDevel
{
    public class Cube : MonoBehaviour
    {
        [SerializeField] MeshRenderer _renderer;
        static readonly  int          MainColor = Shader.PropertyToID("_BaseColor");

        public void ClearColor()
        {
            _renderer.material.SetColor(MainColor, Color.clear);
        }

        public void Set()
        {
            _renderer.material.SetColor(MainColor, Random.ColorHSV());
            transform.position = new(Random.Range(0, 10), Random.Range(0, 10));
        }
    }
}