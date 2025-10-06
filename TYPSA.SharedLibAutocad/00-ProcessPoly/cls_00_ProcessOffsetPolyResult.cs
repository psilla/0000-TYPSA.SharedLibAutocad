using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Text;

namespace TYPSA.SharedLib.Autocad.ProcessPoly
{
    public class cls_00_ProcessOffsetPolyResult
    {
        public class ProcessOffsetPolyResult
        {
            public List<Polyline> ValidOffsetPolylines { get; set; }
            public List<Polyline> ValidOffsetAndOriginalPolys { get; set; }
            public HashSet<ObjectId> OffsetPolylinesToIsolate { get; set; }
            public Dictionary<Handle, Handle> DictPolyToOffset { get; set; }
            public StringBuilder InfoSummary { get; set; }
            public double? OffsetDistance { get; set; }
            public int Total { get; set; }
            public int NullCount { get; set; }
            public int ValidCount { get; set; }
        }



    }
}
