using System;

namespace Tcgv.DataReplication.Exceptions
{
    public class PositiveParameterException : ArgumentException
    {
        public PositiveParameterException(string paramName)
            : base("Parameter value must be greater than zero", paramName) { }
    }
}
