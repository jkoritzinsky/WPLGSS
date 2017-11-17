using Prism.Events;
using WPLGSS.Models;

namespace WPLGSS.ViewModels
{
    public class AddToGraphEvent : PubSubEvent<(InputChannel channel, int graphId)>
    {
    }
}