namespace Tcgv.DataReplication.DataModel
{
    public class VertexDFSData
    {
        public Vertex Vertex { get; set; }
        public int Length { get; set; }
        public VertexDFSData Previous { get; set; }
    }
}