using System.Collections.Generic;

namespace CommerceFundamentalsWeb.SupportingClasses
{
    public class NodeEntryCombo
    {
        public IEnumerable<NameAndUrls> nodes { get; set; }
        public IEnumerable<NameAndUrls> entries { get; set; }
    }
}