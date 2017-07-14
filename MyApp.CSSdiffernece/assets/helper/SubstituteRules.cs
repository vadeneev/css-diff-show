using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.CSSdiffernece.assets.helper
{
    class Replacer
    {
        public string main;
        public string rule;
        public string replace;
        public Replacer(string main, string replace, string rule = "")
        {
            this.rule = rule;
            this.main = main;
            this.replace = replace;
        }
    }
    static class SubstituteRules
    {
        private static List<Replacer> substList;
        static SubstituteRules()
        {
            substList = new List<Replacer> (){
                 new Replacer("white","#fff"),
                 new Replacer("black", "#000"),
                 new Replacer("1 0 auto", "1", "flex")
                };

        }
        static public string proceed(string incomeRuleValue, string incomeRule = "")
        {
            string result = incomeRuleValue;
            for (int i = 0, length = substList.Count(); i < length; i++)
            {
                if (substList[i].rule == "" || incomeRule.IndexOf(substList[i].rule) > -1)
                    result = result.Replace(substList[i].main, substList[i].replace);
            }
            return result;
        }
    }
}
