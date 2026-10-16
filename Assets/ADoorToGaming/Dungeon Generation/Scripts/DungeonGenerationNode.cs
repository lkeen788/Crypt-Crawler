using UnityEngine;

namespace DungeonGeneration // Created by Soul from A Door To Gaming
{
    public class DungeonGenerationNode : MonoBehaviour
    {
        public DungeonGenerationRoom ownerRoom { get; private set; }

        [Range(0.0f, 100.0f)]
        public float chanceToGenerateFromDifferentGroups = 0f;
        public DungeonGenerationRoom overrideDeadEnd;

        private void Awake()
        {
            ownerRoom = transform.parent.GetComponent<DungeonGenerationRoom>();

            if (transform.rotation.x != 0f || transform.rotation.z != 0f)
            {
                Debug.LogWarning("Generation Nodes That Are Have Their X Or Z Rotation Changed Are Not Supported." +
                    "Put The X And Z Rotations To 0 For A Better Experience!!!");
            }
        }
    }
}