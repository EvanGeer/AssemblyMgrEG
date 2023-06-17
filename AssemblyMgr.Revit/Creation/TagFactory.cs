using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Extensions;
using AssemblyMgr.Revit.DataExtraction;
using AssemblyMgr.Revit.Extensions;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyMgr.Revit.Creation
{
    internal class TagFactory
    {
        public Document Doc { get; set; }
        public ElementDistiller Distiller { get; set; }
        public TagTypeExtractor TagTypeExtractor { get; }

        public TagFactory(Document doc, ElementDistiller distiller, TagTypeExtractor tagTypeExtractor)
        {
            Doc = doc;
            Distiller = distiller;
            TagTypeExtractor = tagTypeExtractor;
        }

        /// <summary>
        /// Tags all elements in a 3D view.
        /// </summary>
        /// <param name="view">View to tag</param>
        /// <param name="tagOffset">Tag off set if using leaders. Will be 0 if leaders are not used.</param>
        public void CreateTags(BuiltViewPort assemblyMgrView)
        {
            if (!(assemblyMgrView.Definition is ViewPortDefinition_Model viewDef)) return;

            // ToDo: custom tag offset per tag type
            var tagOffset = 6 / 12.0;

            //build out tags
            if (assemblyMgrView.View is View3D view3d && !view3d.IsLocked)
                view3d.SaveOrientationAndLock();

            foreach (var tagSet in viewDef.TagSettings.TagItems.Where(x => x.type != ItemType.None && !string.IsNullOrEmpty(x.tag)))
            {
                var elementSet = Distiller[tagSet.type];

                var tagType = TagTypeExtractor.GetTagType(tagSet.tag);

                elementSet.ForEach(x => placeTag(x, tagType, tagOffset, assemblyMgrView.View));
            }
        }

        private void placeTag(Element elem, FamilySymbol tagType, double tagOffset, View view)
        {
            var elemRef = new Reference(elem);
            var elementCenter = elem.get_BoundingBox(view).GetCenter();

            var elementDirection = elem.GetDirection();

            // this should be normal to the element
            var offsetDiretion = elem.IsPipe()
                ? elementDirection.CrossProduct(view.ViewDirection)
                : elementDirection.Negate();

            if (offsetDiretion.IsAlmostEqualTo(view.RightDirection, 0.1)
                || offsetDiretion.IsAlmostEqualTo(view.RightDirection.Negate(), 0.1))
                offsetDiretion = view.RightDirection.Negate();

            if (offsetDiretion.IsAlmostEqualTo(view.UpDirection, 0.1)
                || offsetDiretion.IsAlmostEqualTo(view.UpDirection.Negate(), 0.1))
                offsetDiretion = view.UpDirection.Negate();


            var tagXYZ = elementCenter + offsetDiretion * tagOffset;

            var tag = IndependentTag.Create(Doc, tagType.Id, view.Id, elemRef, true/* viewDef.HasTagLeaders*/, TagOrientation.Horizontal, tagXYZ);
            tag.LeaderEndCondition = LeaderEndCondition.Free;
            tag.SetLeaderEnd(elemRef, elementCenter);
            tag.TagHeadPosition = tagXYZ;
        }
    }
}
