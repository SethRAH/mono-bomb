using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoBomb
{
    class DJ
    {
        public string CurrentPlaylist { get; private set; }
        public string CurrentSong { get; private set; }

        private SortedDictionary<string, IEnumerable<SoundEffect>> playlists;

        public IEnumerable<string> PlayListNames
        {
            get
            {
                if(playlists != null)
                {
                    return new List<string>(playlists.Keys);
                }
                return new List<string>();
            }
        }

        public DJ()
        {
            playlists = new SortedDictionary<string, IEnumerable<SoundEffect>>();
        }

        public void Push(SoundEffect song, string playlistName)
        {
            if (playlists.ContainsKey(playlistName))
            {
                playlists[playlistName] = playlists[playlistName].Union(new List<SoundEffect>() { song });
            }
            else
            {
                playlists.Add(playlistName, new List<SoundEffect>() { song });
            }
        }

        public void Push(IEnumerable<SoundEffect> songs, string playlistName)
        {
            if (playlists.ContainsKey(playlistName))
            {
                playlists[playlistName] = playlists[playlistName].Union(songs);
            }
            else
            {
                playlists.Add(playlistName, songs);
            }
        }

        public void Clear()
        {
            playlists = new SortedDictionary<string, IEnumerable<SoundEffect>>();
        }

        public void Clear(string playlistName)
        {
            if (playlists.ContainsKey(playlistName))
            {
                playlists.Remove(playlistName);
            }
        }
    }
}
