using PunctualSolutionsTool.Tool;
using UnityEngine;

namespace PunctualSolutions.UnityPackageDevel
{
    public class ColliderTest : MonoBehaviour
    {
        [SerializeField] BoxCollider box1;
        [SerializeField] BoxCollider box2;
        void Update()
        {
            print($"完全包含:{box1.FullyInclusive(box2)}");
        }
    }
}