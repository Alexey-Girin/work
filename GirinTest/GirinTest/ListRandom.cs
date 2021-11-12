using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GirinTest.Exceptions;

namespace GirinTest
{
    public class ListRandom
    {
        public ListNode Head { get; private set; }
        public ListNode Tail { get; private set; }
        public int Count { get; private set; }

        public void Serialize(Stream s)
        {
            var dictionaryNodeId = new Dictionary<ListNode, int>();
            var currentNode = Head;

            for (var i = 0; i < Count; i++)
            {
                dictionaryNodeId.Add(currentNode, i);
                currentNode = currentNode.Next;
            }

            currentNode = Head;

            using var writer = new BinaryWriter(s);

            for (var i = 0; i < Count; i++)
            {
                writer.Write(currentNode.Data == null);
                writer.Write(currentNode.Data ?? String.Empty);
                writer.Write(currentNode.Random != null
                    ? dictionaryNodeId[currentNode.Random]
                    : -1);

                currentNode = currentNode.Next;
            }
        }

        public void Deserialize(Stream s)
        {
            var listRandomId = new List<int>();
            var dictionaryIdNode = new Dictionary<int, ListNode>();

            var newHead = new ListNode();
            var currentNode = newHead;

            var index = 0;

            using (var reader = new BinaryReader(s))
            {
                if (reader.BaseStream.Length == 0)
                {
                    UpdateState(newHead: null, newTail: null, newCount: 0);
                    return;
                }

                while (true)
                {
                    bool isNullData;
                    string nodeData;
                    int nodeRandomId;

                    try
                    {
                        isNullData = reader.ReadBoolean();
                        nodeData = reader.ReadString();
                        nodeRandomId = reader.ReadInt32();
                    }
                    catch (EndOfStreamException)
                    {
                        throw new FailDeserializationException();
                    }

                    if (isNullData && nodeData != String.Empty)
                    {
                        throw new FailDeserializationException();
                    }

                    currentNode.Data = isNullData ? null : nodeData;
                    listRandomId.Add(nodeRandomId);
                    dictionaryIdNode.Add(index, currentNode);

                    if (reader.BaseStream.Position == reader.BaseStream.Length)
                    {
                        break;
                    }

                    currentNode.Next = new ListNode
                    {
                        Previous = currentNode
                    };
                    currentNode = currentNode.Next;

                    index++;
                }
            }

            if (listRandomId.Max() >= listRandomId.Count || listRandomId.Min() < -1)
            {
                throw new FailDeserializationException();
            }

            currentNode = newHead;

            for (var i = 0; i < listRandomId.Count; i++)
            {
                currentNode.Random = listRandomId[i] != -1
                    ? dictionaryIdNode[listRandomId[i]]
                    : null;

                if (i < listRandomId.Count - 1)
                {
                    currentNode = currentNode.Next;
                }
            }

            UpdateState(newHead: newHead, newTail: currentNode, newCount: listRandomId.Count);
        }

        public bool IsEqualByState(ListRandom otherList)
        {
            if (Count != otherList.Count)
            {
                return false;
            }

            var currentNode = Head;
            var currentNodeOther = otherList.Head;

            var dictionaryNodeId = new Dictionary<ListNode, int>();
            var dictionaryNodeIdOther = new Dictionary<ListNode, int>();

            for (var i = 0; i < Count; i++)
            {
                if (currentNode.Data != currentNodeOther.Data)
                {
                    return false;
                }

                dictionaryNodeId.Add(currentNode, i);
                dictionaryNodeIdOther.Add(currentNodeOther, i);

                currentNode = currentNode.Next;
                currentNodeOther = currentNodeOther.Next;
            }

            currentNode = Head;
            currentNodeOther = otherList.Head;

            for (var i = 0; i < Count; i++)
            {
                if (dictionaryNodeId[currentNode.Random] !=
                    dictionaryNodeIdOther[currentNodeOther.Random])
                {
                    return false;
                }
            }

            return true;
        }

        public void AddToEnd(ListNode newNode)
        {
            Count++;

            if (Head == null)
            {
                Head = newNode;
                Tail = newNode;
                newNode.Random = newNode;

                return;
            }

            Tail.Next = newNode;
            newNode.Previous = Tail;
            newNode.Random = Tail.Previous ?? Tail;
            Tail = newNode;
        }

        private void UpdateState(ListNode newHead, ListNode newTail, int newCount)
        {
            Head = newHead;
            Tail = newTail;
            Count = newCount;
        }
    }
}
