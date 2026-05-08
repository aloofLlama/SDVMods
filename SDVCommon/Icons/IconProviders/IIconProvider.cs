using SDVCommon;
using SDVCommon.Icons;

namespace SDVCommon.Icons
{
    public interface IIconProvider
    {
        bool CanHandle(string id);
        Icon? LoadIcon(string id);
    }
}
