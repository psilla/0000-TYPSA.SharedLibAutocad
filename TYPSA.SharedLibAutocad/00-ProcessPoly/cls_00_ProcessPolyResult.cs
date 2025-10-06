using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System.Text;

namespace TYPSA.SharedLib.Autocad.ProcessPoly
{
    public class cls_00_ProcessPolyResult
    {
        public class ProcessPolyResult
        {
            public List<Polyline> ValidPolylines { get; set; }
            public HashSet<ObjectId> PolylinesToIsolate { get; set; }
            public StringBuilder InfoSummary { get; set; }
            public int Total { get; set; }
            public int NullCount { get; set; }
            public int ValidCount { get; set; }
        }



    }
}
