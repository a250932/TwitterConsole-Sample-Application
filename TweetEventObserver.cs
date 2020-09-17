
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Collections.Generic;
namespace TwitterConsole
{
    public class TweetEventObserver : IObserver<Payload>
    {
        private static Timer aTimer;
        private int count = 0;
        private double emojiPercent = 0;
        private double picPercent = 0;
        private int emojiCount = 0;
        private int picCount = 0;
        private int countContainsUrl = 0;
        private double urlPercent = 0;
        private DateTime dtStart = System.DateTime.Now;

        public TweetEventObserver()
        {
            Console.WriteLine("Tweet Stats will display in ~ 30 seconds ");
            dtStart = System.DateTime.Now;
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 30000;
            aTimer.Elapsed += Display;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        public void Display(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Displaying Tweet Stats ");
            DisplayTimeExpired();
            DisplayEmojiCount();
            Console.WriteLine("Total Tweets: " + count.ToString());
            Console.WriteLine("Top 5 Emojis: " + GlobalCounter.Current.TopEmoji());
            Console.WriteLine("Top 5 Hashtags: " + GlobalCounter.Current.TopHashTags());
            Console.WriteLine("Top 5 Domains: " + GlobalCounter.Current.TopDomains());
            picCount = GlobalCounter.Current.PicinUrlCount();
            Console.WriteLine("Pic Count ", picCount);
            emojiPercent = ((double)emojiCount / (double)count) * 100;
            Console.WriteLine("Percentage of tweets with emojis: " + Math.Round(emojiPercent,2) + "%");
            urlPercent = ((double)countContainsUrl / (double)count) * 100;
            Console.WriteLine("Percentage of tweets with urls: " + Math.Round(urlPercent, 2) + "%");
            Console.WriteLine("--------------------------------------------------------------------------");

        }

        public void OnNext(Payload TwitterPayloadData)
        {
            var strMessage = TwitterPayloadData.Text;
            SetCounter();
            CountEmojis(TwitterPayloadData.RawJson);
            CountHashTags(TwitterPayloadData.HashTags);
            CountUrls(TwitterPayloadData.Urls);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {

        }
        private void SetCounter()
        {
            count = GlobalCounter.Current.GetNext();
        }

        private void DisplayEmojiCount()
        {
            var count = GlobalCounter.Current.GetEmojiCount();
        }


        private void DisplayTimeExpired()
        {
            var te =  (System.DateTime.Now - dtStart); ;
            Console.WriteLine("timeExpired: {0}", te.TotalSeconds.ToString());
        }

        private void CountEmojis(string sMessage)
        {
            var emojiList = GlobalCounter.Current.GetEmojiList();
            var result = emojiList.Where(x => sMessage.Contains(x.Unified)).ToList();
            if (result.Count > 0)
            {
                emojiCount += 1;
                GlobalCounter.Current.AddEmoji(result);
            }          
        }
        private void CountHashTags(List<Hashtags> hashtags)
        {
            if (hashtags.Count > 0)
            {
                GlobalCounter.Current.AddHashTags(hashtags);
            }           
        }

        private void CountUrls(List<Urls> urls)
        {
            if (urls.Count > 0)
            {
                GlobalCounter.Current.AddURLs(urls);
                countContainsUrl +=1;
            }
        }
    }
}
