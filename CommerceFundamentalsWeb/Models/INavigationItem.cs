using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;

namespace CommerceFundamentalsWeb.Models
{
    public interface INavigationItem : IContent
    {
        bool VisibleInMenu { get; }
    }
}
