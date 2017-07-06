using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyApp.CSSdiffernece.assets
{
    public class CSSobj
    {        

        private List<SelectorRule> cssrules;

        public List<SelectorRule> CSSrules
        {
            get { return cssrules; }
            set { cssrules = value; }
        }

        private string cssContent;
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public CSSobj(string filePath)
        {
            this.FilePath = filePath;
            cssrules = new List<SelectorRule>();
        }
        public void init()
        {
            if (filePath == null || filePath.Length == 0) return;
            StreamReader sr = new StreamReader(FilePath);
            cssContent = sr.ReadToEnd();
            sr.Close();
            normalizeCSS();
            createCSSOM();
        }
        private void normalizeCSS()
        {   
            cssContent = Regex.Replace(cssContent, @"\/\*[\s\w\W]*?\*\/", "", RegexOptions.Multiline);            
        }

        private void createCSSOM()
        {
            string mediaPtrn = @"@media\s{1}([\w\W\s]*?)\{([\w\W]*?)\}[\r\n\t\s]{0,}\}";
            foreach (Match m in Regex.Matches(cssContent, mediaPtrn))
            {                
                addRules(m.Groups[2].Value, m.Groups[1].Value);                
            }                        
            addRules(Regex.Replace(cssContent, mediaPtrn, ""), null);                
        }
        private void addRules(string rulesStr, string media)
        {
            var allRules = Regex.Split(rulesStr, @"}\r?\n{0,}\t?").Where(
                s => !String.IsNullOrEmpty(s.Trim())).ToList<string>();
            var length = allRules.Count;

            for (int i = 0; i < length; i++)
            {
                var tempRule = new SelectorRule();
                var pair = Regex.Split(allRules[i], "{\r?\n?\t?");
                tempRule.addSelectors(pair[0].Split(','), media);
                tempRule.addRules(Regex.Split(pair[1].Trim(), ";|(;?\r\n\t?)"));
                cssrules.Add(tempRule);
            }
        }
    }
}
