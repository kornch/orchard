using Orchard.Models;

namespace Orchard.Core.Common.Models {
    public class ContentModel : ModelPart {
        public string Body { get; set; }
        public string Format { get; set; }
    }
}