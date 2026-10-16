using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGeneration // Created by Soul from A Door To Gaming
{
    [CreateAssetMenu(fileName = "Dungeon Generation Type", menuName = "Dungeon Generation/Dungeon Generation Type")]
    public class DungeonGenerationType : ScriptableObject
    {
        [Serializable]
        public struct Room
        {
            public DungeonGenerationRoom dungeonRoom;

            [Min(1)] 
            public int weight;

            [Min(1)] 
            public int level;
        }

        public int minRoomCount;
        public int maxRoomCount;
        [Space]
        public string firstGroup;
        [Space]
        [SerializeField] private Room[] rooms;
        [Space]
        public DungeonGenerationRoom deafaultDeadEnd;

        public DungeonGenerationRoom GetRandomRoom(DungeonGenerationNode node, bool isFirstRoom, int generationLevel)
        {
            var allowedRooms = rooms.ToList();
            allowedRooms.RemoveAll(x => x.level > generationLevel);

            string group = isFirstRoom ? firstGroup : node.ownerRoom.group;
            Debug.Log(group == firstGroup && isFirstRoom);

            bool canGetFromDifferentGroup = false;

            if (node != null) // If its not first room
                canGetFromDifferentGroup = node.chanceToGenerateFromDifferentGroups >= UnityEngine.Random.Range(0.0f, 100.0f);

            var weightedRoomList = new List<DungeonGenerationRoom>();
            foreach (var room in allowedRooms)
            {
                for (int w = 0; w < room.weight; w++)
                {
                    weightedRoomList.Add(room.dungeonRoom);
                }
            }

            weightedRoomList = weightedRoomList.OrderBy(x => UnityEngine.Random.value).ToList();

            if (isFirstRoom)
            {
                weightedRoomList.RemoveAll(x => x.group != firstGroup);

                if (weightedRoomList == null)
                {
                    Debug.LogError("Generation Type Needs A Room From First Group That Also Supports The Minimum Level!!!");
                    return null;
                }

                return weightedRoomList[UnityEngine.Random.Range(0, weightedRoomList.Count)];
            }

            foreach (var room in weightedRoomList)
            {
                bool isSameGroup = room.group == group;
                if (canGetFromDifferentGroup != isSameGroup)
                {
                    return room;
                }
            }

            return null;
        }
    }
}