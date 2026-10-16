using UnityEngine;
using UnityEngine.Events;

using System.Collections;
using System.Linq;

namespace DungeonGeneration // Created by Soul from A Door To Gaming
{
    public class DungeonGeneration : MonoBehaviour
    {
        private enum LogMode
        {
            None,
            Normal,
            Debug
        }

        [Header("Generator Settings")]
        [SerializeField] private bool generateOnStart = false;
        public DungeonGenerationType genType;

        [Header("Generation Settings")]
        [Min(1)] public int generationLevel;
        [Space]
        public bool useRandomSeed = true;
        public int seed;
        
        [Header("Events")]
        public UnityEvent onGenerationStart;
        public UnityEvent onGenerationComplete;

        [Header("Debug Settings")]
        [SerializeField] private LogMode logMode = LogMode.Normal;

        private int roomCount;
        private int maxRoomCount;

        private void Start()
        {
            // Generate On Start
            if (logMode == LogMode.Debug) Debug.Log($"Is Generating On Start? {generateOnStart}");

            if (generateOnStart)
                GenerateDungeon();
        }

        [ContextMenu("Generate Dungeon")]
        public void GenerateDungeon()
        {
            // Stop Generating Previous Dungeon
            StopAllCoroutines();

            // Set Seed
            if (useRandomSeed) seed = Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(seed);

            if (logMode == LogMode.Normal || logMode == LogMode.Debug) Debug.Log($"Started Generating Dungeon!!! Seed: {seed}");

            // Destroy Old Dungeon
            roomCount = 0;
            maxRoomCount = Random.Range(genType.minRoomCount, genType.maxRoomCount + 1);

            foreach (var room in GetComponentsInChildren<DungeonGenerationRoom>())
            {
                Destroy(room.gameObject);
            }

            // Generate Dungeon
            onGenerationStart.Invoke();
            StartCoroutine(CreateRoom(transform));
        }

        private IEnumerator CreateRoom(Transform startingNode)
        {
            var node = startingNode;

            for (int i = roomCount; i < maxRoomCount; i = roomCount)
            {
                yield return null;
                // Spawn Room
                DungeonGenerationRoom roomToSpawn = genType.GetRandomRoom(
                    node.GetComponent<DungeonGenerationNode>(), // Node to spawn from
                    i == 0, // If is first room
                    generationLevel); // The Generation Level

                var newRoom = Instantiate(roomToSpawn, transform);
                newRoom.gameObject.name = "Room_" + roomCount;


                // Get A Random Node From This Room
                var chosenNode = newRoom.ownedNodes[Random.Range(0, newRoom.ownedNodes.Count)].transform;
                newRoom.ownedNodes.Remove(chosenNode.GetComponent<DungeonGenerationNode>());

                // Change Transform Of Room
                if (node != transform)
                {
                    // Rotate Room
                    Quaternion rotationOffset = Quaternion.Inverse(chosenNode.rotation)
                        * Quaternion.LookRotation(-node.forward, node.up);
                    newRoom.transform.rotation = rotationOffset * newRoom.transform.rotation;

                    // Position Room
                    Vector3 positionOffset = node.position - chosenNode.position;
                    newRoom.transform.position += positionOffset;
                }

                // Make Sure Game Registered New Transform Before Overlap Check
                yield return null;

                // Check If Overlaping
                if (newRoom.CheckOverlap() && node != transform)
                {
                    Destroy(newRoom.gameObject);
                    if (logMode == LogMode.Debug) Debug.Log($"Failed To Spawned Room");
                }
                else if (node != transform)
                {
                    Destroy(node.gameObject);
                    Destroy(chosenNode.gameObject);

                    if (logMode == LogMode.Debug) Debug.Log($"Spawned Room {roomCount}");
                }
                else
                {
                    if (logMode == LogMode.Debug) Debug.Log("Spawned First Room");
                }

                yield return null;

                // Add To Room Count And Spawn New Room If Under Max
                if (newRoom != null)
                {
                    roomCount++;
                }

                var roomNodes = GetComponentsInChildren<DungeonGenerationNode>();
                node = roomNodes[Random.Range(0, roomNodes.Length)].transform;
            }
            PlaceDeadEnds();
        }

        private void PlaceDeadEnds()
        {
            var roomNodes = GetComponentsInChildren<DungeonGenerationNode>().ToList();

            for(int i = 0; i < roomNodes.Count; i++)
            {
                if (roomNodes[i] == null) break;

                if (logMode == LogMode.Debug) Debug.Log($"Spawning Dead End {i}");

                DungeonGenerationRoom chosenDeadEnd = genType.deafaultDeadEnd;

                if (roomNodes[i].overrideDeadEnd != null) chosenDeadEnd = roomNodes[i].overrideDeadEnd;

                var deadEnd = Instantiate(chosenDeadEnd, transform);
                deadEnd.gameObject.name = "DeadEnd_" + i;
                var chosenNode = deadEnd.GetComponentInChildren<DungeonGenerationNode>().transform;
                // Rotate Room
                Quaternion rotationOffset = Quaternion.Inverse(chosenNode.rotation)
                    * Quaternion.LookRotation(-roomNodes[i].transform.forward, roomNodes[i].transform.up);
                deadEnd.transform.rotation = rotationOffset * deadEnd.transform.rotation;

                // Position Room
                Vector3 positionOffset = roomNodes[i].transform.position - chosenNode.position;
                deadEnd.transform.position += positionOffset;
            }

            FinishDungeonGenration();
        }

        private void FinishDungeonGenration()
        {
            foreach(var node in GetComponentsInChildren<DungeonGenerationNode>())
            {
                Destroy(node.gameObject);
            }

            if (logMode == LogMode.Normal || logMode == LogMode.Debug) Debug.Log($"Dungeon Generated With {roomCount} Room's!!!");

            onGenerationComplete.Invoke();
        }
    }
}