using System.Collections.Generic;
using System.Threading;

namespace Tcgv.DataReplication.DataModel
{
    public class Vertex
    {
        public Vertex()
        {
            Id = Interlocked.Increment(ref idCounter);
            Edges = new List<Vertex>();
            Items = new List<int>();
            IsEnabled = true;
            unpropagetedItems = new Queue<int>();
            uncommitedItems = new Queue<int>();
        }

        public long Id { get; }
        public List<Vertex> Edges { get; }
        public List<int> Items { get; private set; }
        public bool IsEnabled { get; private set; }

        public void Connect(Vertex vertex)
        {
            Edges.Add(vertex);
        }

        public void AddItem(int item)
        {
            unpropagetedItems.Enqueue(item);
            Items.Add(item);
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
                    foreach (var v in Edges)
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
