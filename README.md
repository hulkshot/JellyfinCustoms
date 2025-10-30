# JellyfinCustoms

A Jellyfin plugin that pulls channels from a JSON API and exposes them as Live TV channels. Fully configurable — replace the API URL with your own authorized source.

---

## Features

- Fetch channels from any JSON API you control
- Expose streams in Jellyfin Live TV
- Supports channel logos, titles, and categories
- Easily extendable to support program guide (EPG) in the future

---

## Requirements

- Jellyfin 10.9 or later
- .NET 8 SDK to build the plugin
- Authorized API endpoint returning JSON channel data

---

## JSON API Format

The plugin expects an array of channel objects:

```json
[
  {
    "Id": "match1",
    "Title": "Example Match",
    "StreamUrl": "https://yourapi.example.com/hls/match1.m3u8",
    "Logo": "https://yourapi.example.com/images/match1.png",
    "Category": "Sports"
  }
]
