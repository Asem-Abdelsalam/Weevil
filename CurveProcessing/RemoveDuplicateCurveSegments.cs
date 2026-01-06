using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weevil.Core;

namespace Weevil.CurveProcessing
{
    public class RemoveDuplicateCurveSegments : GH_Component
    {
        public RemoveDuplicateCurveSegments()
            : base("Remove Duplicate Segments", "RemDupSegs",
                  "Removes duplicated portions of curves when they partially overlap",
                  WeevilConstants.Category, WeevilConstants.Curves)
        {}
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Input curves to process", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "T", "Tolerance for overlap detection", GH_ParamAccess.item, 0.01);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Unique Curves", "U", "Curves with duplicated segments removed", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Has Duplicates", "D", "Boolean List indicating if each curve had duplicated segments", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var curves = new List<Curve>();
            double tol = 0.01;

            if (!DA.GetDataList(0, curves)) return;
            DA.GetData(1, ref tol);

            var resultCurves = new List<Curve>();
            var info = new List<string>();

            bool[] processed = new bool[curves.Count];

            for (int i = 0; i < curves.Count; i++)
            {
                if (processed[i]) continue;

                Curve curveA = curves[i];
                var segments = new List<Curve> { curveA.DuplicateCurve() };
                bool foundDuplicate = false;



            }






        }
        public override Guid ComponentGuid => new Guid("A1B2C3D4-E5F6-4A5B-9C8D-7E6F5A4B3C2D");

    }
}
