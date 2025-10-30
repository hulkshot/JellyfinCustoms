using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JellyfinCustoms.Models;
using MediaBrowser.Controller.LiveTv;

namespace JellyfinCustoms
{
    public class CustomsLiveTvService : ILiveTvService
    {
        private readonly ChannelProvider _channelProvider = new ChannelProvider();

        public async Task<IEnumerable<ChannelInfo>> GetChannels(CancellationToken cancellationToken)
        {
            var channelItems = await _channelProvider.GetChannelsAsync();
            var channels = new List<ChannelInfo>();

            foreach (var item in channelItems)
            {
                channels.Add(new ChannelInfo
                {
                    Id = item.Id,
                    Name = item.Title,
                    Number = item.Id,
                    LogoUrl = item.Logo,
                    Tags = string.IsNullOrEmpty(item.Category) ? new List<string>() : new List<string> { item.Category },
                    IsFavorite = false
                });
            }

            return channels;
        }

        public Task<IEnumerable<ProgramInfo>> GetPrograms(string channelId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<ProgramInfo>>(new List<ProgramInfo>());
        }
    }
}
