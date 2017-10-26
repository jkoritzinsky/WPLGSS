using Prism.Events;
using WPLGSS.Models;

namespace WPLGSS.ViewModels
{
    internal class AddToGraphEvent : PubSubEvent<(Channel channel, int graphId)>
    {
    }
}