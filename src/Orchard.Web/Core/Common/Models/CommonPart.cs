﻿using System;
using Orchard.Core.Common.Records;
using Orchard.Models;
using Orchard.Security;

namespace Orchard.Core.Common.Models {
    public class CommonPart : ContentPart<CommonRecord> {
        private readonly Lazy<IUser> _owner = new Lazy<IUser>();
        private readonly Lazy<IContent> _container = new Lazy<IContent>();

        public IUser Owner {
            get { return _owner.Value; }
            set { _owner.Value = value; }
        }

        public IContent Container {
            get { return _container.Value; }
            set { _container.Value = value; }
        }

        public void LoadOwner(Func<IUser> loader) {
            _owner.Loader(loader);
        }
        public void LoadContainer(Func<IContent> loader) {
            _container.Loader(loader);
        }
    }

    public class Lazy<T> {
        private Func<T> _loader;
        private T _value;

        public void Loader(Func<T> loader) {
            _loader = loader;
        }

        public T Value {
            get {
                if (_loader != null) {
                    _value = _loader();
                    _loader = null;
                }
                return _value;
            }
            set {
                _value = value;
                _loader = null;
            }
        }
    }
}