using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterConsole
{
    public class Emoji
    {
        private string unified;
        public string Unified
        {
            get { return unified; }   // get method
            set { unified = GetUnified(value); }
        }
        public string GetUnified(string uValue)
        {
            StringBuilder sb = new StringBuilder("");
            var pref = @"\u";
            var sArry = uValue.Split('-');
            for (int i = 0; i < sArry.Length; i++)
            {

                sb.Append(pref);
                sb.Append(sArry[i]);
            }
            return sb.ToString();
        }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Non_Qualified { get; set; }


    }
}
