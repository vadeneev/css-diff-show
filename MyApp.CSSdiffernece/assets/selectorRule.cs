using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyApp.CSSdiffernece.assets
{
    public class cssRule
    {
        public string name;
        public string value;
        public cssRule(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
        public override string ToString()
        {
            return name + ": " + value+";";
        }
    }
    public class SelectorRule
    {
        public bool isChecked = false;

        public int selectorEqTo = -1;

        public bool[] hasRules;

        public string selectorHASH;        

        private string media;

        public string Media
        {
            get { return media; }
            set { media = value; }
        }

        private List<string> selector;

        public List<string> Selector
        {
            get { return selector; }
            set { selector = value; }
        }
        private List<cssRule> rules;

        public List<cssRule> Rules
        {
            get { return rules; }
            set { rules = value; }
        }
       
        public SelectorRule()
        {
            selector = new List<string>();
            rules = new List<cssRule>();
        }

        public void addRules(IEnumerable<string> strRules)
        {
            var clearedRules = strRules.Where(s => !string.IsNullOrEmpty(s.Trim())).ToList<string>();
            var length = clearedRules.Count();
            for(int i = 0; i < length; i++)
            {
                var tempRule = clearedRules[i].Split(':').Where(s => !string.IsNullOrEmpty(s.Trim())).Select(s => s.Trim()).ToArray<string>();
                if (tempRule == null || tempRule.Length < 2) continue;
                rules.Add( new cssRule(textNormalize(tempRule[0]), textNormalize(tempRule[1])));
                
            }
            hasRules = new bool[rules.Count];
        }
        public void addSelector(string selectorVal)
        {
            Selector.Add(selectorVal);
        }
        public void addSelectors(IEnumerable<string> selectorVal, string media)
        {
            Media = media == null? media : normalizeMedia(media);
            selectorVal = selectorVal.Select(x => normalizeSelector(x)).ToList<string>();
            Selector.AddRange(selectorVal);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] buf = Encoding.UTF8.GetBytes(string.Join(",", selectorVal) + Media);
            byte[] hash = sha1.ComputeHash(buf, 0, buf.Length); 
            selectorHASH = BitConverter.ToString(hash).Replace("-", "");
        }
        private string textNormalize(string str)
        {
            return Regex.Replace(str, @"\s{2,}", " ").Trim();
        }
        private string normalizeMedia(string str)
        {
            str = textNormalize(str);
            str = Regex.Replace(str, @"\s?,\s?", ", ");
            str = Regex.Replace(str, @"(and|,)\s?\(", "$1 (");
            str = Regex.Replace(str, @"\)\s?(and|,)", ") $1");            
            str = Regex.Replace(str, @"\((\s?)([\w\d-]+)(\s?):(\s?)([\.\w\d-]+)(\s?)\)", "($2:$5)");
            return str;
        }
        private string normalizeSelector(string str)
        {            
            str = textNormalize(str);
            str = Regex.Replace(str, @"\s{0,}([>+~]){1}\s{0,}", "$1");
            str = Regex.Replace(str, @"\[\s ? ([\w -] +)\s ? ([| *$^] ?=) ?\s ? ([\w\""'-]+)?\s?\]","[$1$2$3]");
            return str;
        }
    }
}
