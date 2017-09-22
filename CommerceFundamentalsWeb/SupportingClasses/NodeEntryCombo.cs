using System.Collections.Generic;

namespace CommerceFundamentalsWeb.SupportingClasses
{
    public class NodeEntryCombo
    {
        public IEnumerable<NameAndUrls> Nodes { get; set; }
        public IEnumerable<NameAndUrls> Entries { get; set; }
    }
}