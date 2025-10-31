using System;
using System.Collections.Generic;

namespace JellyfinCustoms.Models
{
    public class StreamedPkMatch
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        // Unix timestamp in milliseconds
        public long Date { get; set; }
        public string? Poster { get; set; }
        public bool Popular { get; set; }

        public TeamsInfo? Teams { get; set; }

        public List<SourceInfo>? Sources { get; set; }
    }

    public class TeamsInfo
    {
        public TeamInfo? Home { get; set; }
        public TeamInfo? Away { get; set; }
    }

    public class TeamInfo
    {
        public string? Name { get; set; }
        public string? Badge { get; set; }
    }

    public class SourceInfo
    {
        public string Source { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
    }
}
