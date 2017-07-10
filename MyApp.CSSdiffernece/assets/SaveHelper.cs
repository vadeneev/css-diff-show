using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.CSSdiffernece.assets
{
    static class SaveHelper
    {
        private static StringBuilder formAnswerSelectors(CSSobj obj, bool isShowSigns = true, bool isMinus = false)
        {
            StringBuilder sb = new StringBuilder();
            var length = obj.CSSrules.Count();
            for (int i = 0; i < length; i++)
            {
                if (obj.CSSrules[i].selectorEqTo == -1)
                {
                    sb.Append(appendSelector(obj.CSSrules[i], isMinus, isShowSigns));
                    continue;
                }
            }
            return sb;
        }
        private static StringBuilder formAnswerRules(CSSobj obj, CSSobj obj_2)
        {
            StringBuilder sb = new StringBuilder();
            var length = obj.CSSrules.Count();
            for (int i = 0; i < length; i++)
            {
                if (obj.CSSrules[i].selectorEqTo == -1)
                {
                    continue;
                }
                var eqNum = obj.CSSrules[i].selectorEqTo;
                if (!(obj.CSSrules[i].hasRules.Any(x => x == false) || obj_2.CSSrules[eqNum].hasRules.Any(x => x == false))) continue;
                if (obj.CSSrules[i].Media != null)
                    sb.Append("@media " + obj.CSSrules[i].Media + "{\r\n");
                sb.Append(string.Join(",\r\n", obj.CSSrules[i].Selector))
                    .Append(" {\r\n\t");
                for (int j = 0; j < obj.CSSrules[i].Rules.Count(); j++)
                {
                    if (obj.CSSrules[i].hasRules[j]) continue;
                    sb.Append("\r\n-\t" + obj.CSSrules[i].Rules[j]);
                }
                for (int j = 0; j < obj_2.CSSrules[eqNum].Rules.Count(); j++)
                {
                    if (obj_2.CSSrules[eqNum].hasRules[j]) continue;
                    sb.Append("\r\n+\t" + obj_2.CSSrules[eqNum].Rules[j]);
                }
                sb.Append("\r\n\t}\r\n");
                if (obj.CSSrules[i].Media != null)
                    sb.Append("}");
            }
            return sb;
        }
        private static StringBuilder formFull(CSSobj obj_1, CSSobj obj_2)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(addLegenda(obj_1, obj_2));
            sb.Append(formAnswerSelectors(obj_1));
            sb.Append(formAnswerSelectors(obj_2, true, true));
            sb.Append(formAnswerRules(obj_1, obj_2));
            return sb;
        }
        private static StringBuilder formRules(CSSobj obj_1, CSSobj obj_2)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(addLegenda(obj_1, obj_2));
            sb.Append(formAnswerRules(obj_1, obj_2));
            return sb;
        }
        private static StringBuilder formSelector(CSSobj obj, bool showSign, bool isMinus)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(addSelectorLegenda(isMinus));
            sb.Append(formAnswerSelectors(obj, showSign, isMinus));
            return sb;
        }


        public static void saveFile(string filePath, CSSobj objA, CSSobj objB, string option)
        {
            switch (option)
            {
                case "Export errors at same selectors":
                    System.IO.File.WriteAllText(filePath, formRules(objA, objB).ToString());
                    break;
                case "Export not exist at #1":
                    System.IO.File.WriteAllText(filePath, formSelector(objA, false, true).ToString());
                    break;
                case "Export not exist at #2":
                    System.IO.File.WriteAllText(filePath, formSelector(objB, false, false).ToString());
                    break;
                default:
                    System.IO.File.WriteAllText(filePath, formFull(objA, objB).ToString());
                    break;
            }
        }
        private static StringBuilder addSelectorLegenda(bool needAdd = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DESCRIPTION:\r\n");
            String text = needAdd ? "added to" : "remover from";
            sb.Append("This content must be " + text + "File\r\n");
            sb.AppendLine("###################################################################\r\n");
            return sb;
        }
        private static StringBuilder addLegenda(CSSobj objA, CSSobj objB)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DESCRIPTION:\r\n");
            sb.AppendLine("\tSign:\t'-' means - you must delete this rules or selectors from file: " + objA.FilePath + "\r\n");
            sb.AppendLine("\tSign:\t'+' means - you must add this rules or selectors to file: " + objA.FilePath + "\r\n");
            sb.AppendLine("###################################################################\r\n");
            sb.AppendLine("First file:\t" + objA.FilePath);
            sb.AppendLine("Second file:\t" + objB.FilePath);
            sb.AppendLine("###################################################################\r\n");
            return sb;
        }
        private static StringBuilder appendSelector(SelectorRule objRule, bool isMinus, bool needSigns = true)
        {
            StringBuilder sb = new StringBuilder();
            string header = isMinus ?
                "---------------exists in first file, but not exists in second---------------" :
                "+++++++++++++++++   not exists in first file, but exists in second++++++++++++";
            string footer = isMinus ? "--------------------------" : "+++++++++++++++++++++++";
            if (needSigns)
                sb.Append("\r\n/*" + header + "*/");
            sb.Append("\r\n");
            if (objRule.Media != null)
                sb.Append("@media " + objRule.Media + "{\r\n");
            sb.Append(string.Join(",\r\n", objRule.Selector))
                .Append(" {\r\n\t")
                .Append(string.Join("\r\n\t", objRule.Rules))
                .Append("\r\n\t}\r\n");
            if (objRule.Media != null)
                sb.Append("}");
            if (needSigns)
                sb.Append("\r\n/*" + footer + "*/" + "\r\n");
            sb.Append("\r\n");
            return sb;
        }
    }
}
