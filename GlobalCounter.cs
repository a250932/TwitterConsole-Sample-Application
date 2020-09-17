using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace TwitterConsole
{
    using System;
    using System.Threading;

    /// <summary>
    /// A multithread safe singleton class to generate unique integer counter from 0 up to scope of uint
    /// </summary>
    public sealed class GlobalCounter
    {
        private List<Emoji> emojis = null;
        private static List<Emoji> internalEmojiList = new List<Emoji>();
        private static List<Hashtags> internalHashTagList = new List<Hashtags>();
        private static List<Urls> internalURLList = new List<Urls>();
        private static readonly object lockObject = new object();
        private static GlobalCounter counter = null;
        private DateTime dtStart = System.DateTime.Now;
        private int internalCounter = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalCounter"/> class.
        /// Private constructor for singleton
        /// </summary>
        private GlobalCounter()
        {
            string json = File.ReadAllText(@"emoji.json");
            emojis = JsonConvert.DeserializeObject<List<Emoji>>(json);

        }

        /// <summary>
        /// Gets the singleton instance with thread safety
        /// </summary>
        public static GlobalCounter Current
        {
            get
            {
                if (counter == null)
                {
                    lock (lockObject)
                    {
                        if (counter == null)
                        {
                            counter = new GlobalCounter();
                        }
                    }
                }

                return counter;
            }
        }


        /// <summary>
        /// Gets the next counter after specified increment. Default is 1.
        /// </summary>
        /// <param name="increment">incement</param>
        /// <returns>The new counter value AFTER the increment</returns>
        public int GetNext(uint increment = 1)
        {
            return Interlocked.Add(ref this.internalCounter, (int)increment);
        }

        public int GetEmojiCount()
        {
            return internalEmojiList.Count();
        }
        public void AddEmoji(List<Emoji> emj)
        {
            internalEmojiList.AddRange(emj);

        }

        public void AddHashTags(List<Hashtags> ht)
        {
            internalHashTagList.AddRange(ht);

        }

        public void AddURLs(List<Urls> urls)
        {
            internalURLList.AddRange(urls);

        }

        public int PicinUrlCount()
        {
            return internalURLList.Where(t => t.isPic == true).Count();
                           
        }


        public string TopEmoji()
        {
            StringBuilder sb = new StringBuilder();
            var emojiRank = internalEmojiList.GroupBy(x => x.Name)
                          .Select(group => new
                          {
                              Emoji = group.Key,
                              Count = group.Count()
                          })
                          .OrderByDescending(x => x.Count).ToList().Take(5);
            sb.Append("{");
            foreach (var e in emojiRank)
            {
                sb.Append("[");
                sb.Append(e.Emoji);
                sb.Append(",");
                sb.Append(e.Count);
                sb.Append("]");
                sb.Append(",");
                sb.Append(" ");
            }
            sb.Append("}");
            return sb.ToString();
        }


        public string TopHashTags()
        {
            StringBuilder sb = new StringBuilder();
            var htRank = internalHashTagList.GroupBy(x => x.text)
                          .Select(group => new
                          {
                              Emoji = group.Key,
                              Count = group.Count()
                          })
                          .OrderByDescending(x => x.Count).ToList().Take(5);
            sb.Append("{");
            foreach (var e in htRank)
            {
                sb.Append("[");
                sb.Append(e.Emoji);
                sb.Append(",");
                sb.Append(e.Count);
                sb.Append("]");
                sb.Append(",");
                sb.Append(" ");
            }
            sb.Append("}");
            return sb.ToString();
        }


        public string TopDomains()
        {
            StringBuilder sb = new StringBuilder();
            var htRank = internalURLList.GroupBy(x => x.Domain)
                          .Select(group => new
                          {
                              Emoji = group.Key,
                              Count = group.Count()
                          })
                          .OrderByDescending(x => x.Count).ToList().Take(5);
            sb.Append("{");
            foreach (var e in htRank)
            {
                sb.Append("[");
                sb.Append(e.Emoji);
                sb.Append(",");
                sb.Append(e.Count);
                sb.Append("]");
                sb.Append(",");
                sb.Append(" ");
            }
            sb.Append("}");
            return sb.ToString();
        }



        public TimeSpan GetTimeExpired()
        {
            return (System.DateTime.Now - dtStart);
        }

        public List<Emoji> GetEmojiList()
        {
            return emojis;
        }
    }

}
