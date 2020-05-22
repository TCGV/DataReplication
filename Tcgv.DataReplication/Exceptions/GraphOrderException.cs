using System;

namespace Tcgv.DataReplication.Exceptions
{
    public class GraphOrderException : Exception
    {
        public GraphOrderException()
            : base("Graph order must be less than number of vertices") { }
    }
}
