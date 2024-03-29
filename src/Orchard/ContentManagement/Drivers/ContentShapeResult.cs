﻿using System;
using Orchard.ContentManagement.Handlers;
using Orchard.DisplayManagement.Shapes;

namespace Orchard.ContentManagement.Drivers {
    public class ContentShapeResult : DriverResult {
        private string _defaultLocation;
        private string _differentiator;
        private readonly string _shapeType;
        private readonly string _prefix;
        private readonly Func<BuildShapeContext, dynamic> _shapeBuilder;
        private string _groupId;

        public ContentShapeResult(string shapeType, string prefix, Func<BuildShapeContext, dynamic> shapeBuilder) {
            _shapeType = shapeType;
            _prefix = prefix;
            _shapeBuilder = shapeBuilder;
        }

        public override void Apply(BuildDisplayContext context) {
            ApplyImplementation(context, context.DisplayType);
        }

        public override void Apply(BuildEditorContext context) {
            ApplyImplementation(context, null);
        }

        private void ApplyImplementation(BuildShapeContext context, string displayType) {
            var placement = context.FindPlacement(_shapeType, _differentiator, _defaultLocation);
            if (string.IsNullOrEmpty(placement.Location) || placement.Location == "-")
                return;

            dynamic parentShape = context.Shape;
            context.ContentPart = ContentPart;

            var newShape = _shapeBuilder(context);

            // ignore it if the driver returned a null shape
            if(newShape == null) {
                return;
            }

            // add a ContentPart property to the final shape 
            if (ContentPart != null && newShape.ContentPart == null) {
                newShape.ContentPart = ContentPart;
            }

            // add a ContentField property to the final shape 
            if (ContentField != null && newShape.ContentField == null) {
                newShape.ContentField = ContentField;
            }

            ShapeMetadata newShapeMetadata = newShape.Metadata;
            newShapeMetadata.Prefix = _prefix;
            newShapeMetadata.DisplayType = displayType;
            newShapeMetadata.PlacementSource = placement.Source;
            
            // if a specific shape is provided, remove all previous alternates and wrappers
            if (!String.IsNullOrEmpty(placement.ShapeType)) {
                newShapeMetadata.Type = placement.ShapeType;
                newShapeMetadata.Alternates.Clear();
                newShapeMetadata.Wrappers.Clear();
            }

            // parse group placement
            var group = placement.GetGroup();
            if (!String.IsNullOrEmpty(group)) {
                _groupId = group;
            }

            if (!string.Equals(context.GroupId ?? "", _groupId ?? "", StringComparison.OrdinalIgnoreCase))
                return;

            foreach (var alternate in placement.Alternates) {
                newShapeMetadata.Alternates.Add(alternate);
            }

            foreach (var wrapper in placement.Wrappers) {
                newShapeMetadata.Wrappers.Add(wrapper);
            }

            // the zone name is in reference of Layout, e.g. /AsideSecond
            if (placement.IsLayoutZone()) {
                parentShape = context.Layout;
            }

            var position = placement.GetPosition();
            var zone = placement.GetZone();

            if (String.IsNullOrEmpty(position)) {
                parentShape.Zones[zone].Add(newShape);
            }
            else {
                parentShape.Zones[zone].Add(newShape, position);
            }
        }

        public ContentShapeResult Location(string zone) {
            _defaultLocation = zone;
            return this;
        }

        public ContentShapeResult Differentiator(string differentiator) {
            _differentiator = differentiator;
            return this;
        }

        public ContentShapeResult OnGroup(string groupId) {
            _groupId=groupId;
            return this;
        }

        public string GetDifferentiator() {
            return _differentiator;
        }

        public string GetGroup() {
            return _groupId;
        }

        public string GetLocation() {
            return _defaultLocation;
        }

        public string GetShapeType() {
            return _shapeType;
        }
    }
}