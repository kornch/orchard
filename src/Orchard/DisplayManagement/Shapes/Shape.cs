﻿using System.Collections;
using System.Collections.Generic;

namespace Orchard.DisplayManagement.Shapes {
    public class Shape : IShape, IEnumerable {
        private const string DefaultPosition = "5";

        private readonly IList<object> _items = new List<object>();
        private readonly IList<string> _classes = new List<string>();
        private readonly IDictionary<string, string> _attributes = new Dictionary<string, string>();

        public virtual ShapeMetadata Metadata { get; set; }

        public virtual string Id { get; set; }
        public virtual IList<string> Classes { get { return _classes; } }
        public virtual IDictionary<string, string> Attributes { get { return _attributes; } }
        public virtual IEnumerable<dynamic> Items { get { return _items; } }

        public virtual Shape Add(object item, string position = DefaultPosition) {
            try {
                ((dynamic)item).Metadata.Position = position;
            }
            catch {
                // need to implemented positioned wrapper for non-shape objects
            }
            _items.Add(item); // not messing with position at the moment
            return this;
        }

        public virtual Shape AddRange(IEnumerable<object> items, string position = DefaultPosition) {
            foreach (var item in items)
                Add(item, position);
            return this;
        }

        public virtual IEnumerator GetEnumerator() {
            return _items.GetEnumerator();
        }


    }
}