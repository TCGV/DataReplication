using System.Collections.Generic;
using Tcgv.DataReplication.DataModel;
using Tcgv.DataReplication.Exceptions;

namespace Tcgv.DataReplication.Builders
{
    public abstract class GraphBuilder
    {
        public Graph Build(int vertices, int k)
        {
            ValidateParameters(vertices, k);
            var list = new List<Vertex>();
            for (int i = 0; i < vertices; i++)
                list.Add(new Vertex());
            return Build(list, k);
        }

        public abstract Graph Build(List<Vertex> vertices, int k);

        protected void ValidateParameters(int vertices, int k)
        {
            if (vertices <= 0)
                throw new PositiveParameterException(nameof(vertices));
            if (k <= 0)
                throw new PositiveParameterException(nameof(k));
            if (k > vertices - 1)
                throw new GraphOrderException();
        }
    }
}
