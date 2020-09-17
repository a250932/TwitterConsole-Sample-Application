using System;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
namespace TwitterConsole
{
    class Program
    {
        static void Main(string[] args)
        {


            //Configure Twitter OAuth
            var resource_url = ConfigurationManager.AppSettings["resource_url"];
            var oauthToken = ConfigurationManager.AppSettings["oauth_token"];
            var oauthTokenSecret = ConfigurationManager.AppSettings["oauth_token_secret"];
            var oauthCustomerKey = ConfigurationManager.AppSettings["oauth_consumer_key"];
            var oauthConsumerSecret = ConfigurationManager.AppSettings["oauth_consumer_secret"];
            var searchGroups = ConfigurationManager.AppSettings["twitter_keywords"];

            var sendExtendedInformation = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["send_extended_information"]) ?
            Convert.ToBoolean(ConfigurationManager.AppSettings["send_extended_information"])
            : false;

            var myTweetObserver = new TweetEventObserver();

            var keywords = searchGroups.Contains('|') ? string.Join(",", searchGroups.Split('|')) : searchGroups;
            var tweet = new Tweet();
            var datum = tweet.StreamStatuses(new TwitterConfig(oauthToken, oauthTokenSecret, oauthCustomerKey, oauthConsumerSecret,
            keywords, searchGroups, resource_url)).Where(e => !string.IsNullOrWhiteSpace(e.Text)).Select(t => new Payload { Text = t.Text, RawJson = t.RawJson, HashTags = t.Entities.hashtags, Urls = t.Entities.Urls});

            datum.ToObservable().Subscribe(myTweetObserver);
        }
    }
}
