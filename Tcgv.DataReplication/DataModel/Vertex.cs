using System;
using System.Collections.Generic;
using System.Threading;

namespace Tcgv.DataReplication.DataModel
{
    public class Vertex
    {
        public Vertex()
        {
            Id = Interlocked.Increment(ref idCounter);
            Neighbors = new HashSet<Vertex>();
            Items = new List<int>();
            IsEnabled = true;
            unpropagetedItems = new Queue<int>();
            uncommitedItems = new Queue<int>();
        }

        public long Id { get; }
        public HashSet<Vertex> Neighbors { get; }
        public List<int> Items { get; private set; }
        public bool IsEnabled { get; private set; }

        public void AddNeighbors(params Vertex[] vertices)
        {
            foreach (var v in vertices)
            {
                Neighbors.Add(v);
                v.Neighbors.Add(this);
            }
        }

        public void AddItem(int item)
        {
            unpropagetedItems.Enqueue(item);
            Items.Add(item);
        }

        public int GetMaxShortestPathLength(int expectedVerticesCount)
        {
            var d = 0;
            var queue = new Queue<VertexDFSData>();
            var visited = new HashSet<Vertex>();

            queue.Enqueue(new VertexDFSData { Vertex = this, Length = 0 });
            visited.Add(this);

            while (queue.Count > 0)
            {
                var x = queue.Dequeue();
                d = Math.Max(d, x.Length);
                foreach (var c in x.Vertex.Neighbors)
                {
                    if (!visited.Contains(c))
                    {
                        queue.Enqueue(new VertexDFSData { Vertex = c, Length = x.Length + 1 });
                        visited.Add(c);
                    }
                }
            }

            return visited.Count == expectedVerticesCount ? d : -1;
        }

        public void Disable()
        {
            IsEnabled = false;
        }

        public override string ToString()
        {
            return $"#{Id}";
        }

        internal void Propagate()
        {
            while (unpropagetedItems.Count > 0)
            {
                var x = unpropagetedItems.Dequeue();
                if (IsEnabled)
                {
                    foreach (var v in Neighbors)
                        if (!v.Items.Contains(x))
                            v.uncommitedItems.Enqueue(x);
                }
            }
        }

        internal void Commit()
        {
            while (uncommitedItems.Count > 0)
                AddItem(uncommitedItems.Dequeue());
        }

        private Queue<int> unpropagetedItems;
        private Queue<int> uncommitedItems;

        private static int idCounter;
    }
}
