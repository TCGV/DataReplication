using System.Collections.Generic;
using Tcgv.DataReplication.DataModel;
using Tcgv.DataReplication.Exceptions;

namespace Tcgv.DataReplication.Builders
{
    public abstract class GraphBuilder
    {
        public Graph Build(int n, int r)
        {
            ValidateParameters(n, r);
            var list = new List<Vertex>();
            for (int i = 0; i < n; i++)
                list.Add(new Vertex());
            return Build(list, r);
        }

        public abstract Graph Build(List<Vertex> vertices, int r);

        protected void ValidateParameters(int n, int r)
        {
            if (n <= 0)
                throw new PositiveParameterException(nameof(n));
            if (r <= 0)
                throw new PositiveParameterException(nameof(r));
            if (r > n - 1)
                throw new GraphOrderException();
        }
    }
}
