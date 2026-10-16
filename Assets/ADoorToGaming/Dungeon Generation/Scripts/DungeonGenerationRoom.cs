using UnityEngine;

using System.Collections.Generic;
using System.Linq;

namespace DungeonGeneration // Created by Soul from A Door To Gaming
{
    public class DungeonGenerationRoom : MonoBehaviour
    {
        public Vector3 roomOffset = Vector3.zero;
        public Vector3 roomSize = Vector3.one;
        public string group;
        [HideInInspector] public List<DungeonGenerationNode> ownedNodes { get; private set; } = new List<DungeonGenerationNode>();

        private void Awake()
        {
            ownedNodes = GetComponentsInChildren<DungeonGenerationNode>().ToList();
        }

        public bool CheckOverlap()
        {
            Collider[] hits = Physics.OverlapBox(
                    transform.position + roomOffset,
                    roomSize * 0.49f,
                    transform.rotation,
                    ~0,
                    QueryTriggerInteraction.Ignore);

            foreach (var hit in hits)
            {
                if (hit.GetComponentInParent<DungeonGenerationRoom>() != null && !hit.transform.IsChildOf(transform))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.matrix = rotationMatrix;

            Gizmos.DrawWireCube(roomOffset, roomSize);
        }
    }

}