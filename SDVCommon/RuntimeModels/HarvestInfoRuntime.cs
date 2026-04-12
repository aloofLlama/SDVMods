using System;
using System.Collections.Generic;
using System.Text;
using SDVCommon.Icons;

namespace SDVCommon.RuntimeModels
{
    public class HarvestInfoRuntime
    {
        public Icon? HarvestIcon { get; set; }

        // Future: artisan product icons, cooking icons, etc.
        public Dictionary<string, Icon?> ExtraIcons { get; set; } = new();
    }
}
