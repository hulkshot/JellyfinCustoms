using System.Collections.Generic;
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
                    Name = item.Title,
                    Id = item.Id,
                    LogoUrl = item.Logo,
                    Number = item.Id,
                    Tags = string.IsNullOrEmpty(item.Category) ? new List<string>() : new List<string> { item.Category }
                });
            }

            return channels;
        }

        public Task<IEnumerable<ProgramInfo>> GetPrograms(string channelId, CancellationToken cancellationToken)
        {
            // Optional: implement EPG if your API provides program data
            return Task.FromResult<IEnumerable<ProgramInfo>>(new List<ProgramInfo>());
        }
    }
}
