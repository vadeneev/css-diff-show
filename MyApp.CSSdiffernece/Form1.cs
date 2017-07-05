using MyApp.CSSdiffernece.assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyApp.CSSdiffernece
{
    public partial class Form1 : Form
    {
        CSSobj objA;
        CSSobj objB;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            objA = openAndInit();
            showBtn();
        }

        private CSSobj openCSS()
        {
            return new CSSobj(openFileDialog1.InitialDirectory + openFileDialog1.FileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            objB = openAndInit();
            showBtn();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            proceedCssFiles(objA, objB);
            button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string filePath;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = saveFileDialog1.InitialDirectory + saveFileDialog1.FileName;
                saveFile(filePath);
            }
        }
        private void showBtn()
        {
            if (objA != null && objB != null)
            {
                button3.Enabled = true;
            }
        }
        private CSSobj openAndInit()
        {
            CSSobj obj = null;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                obj = openCSS();
                obj.init();                
            }
            return obj;
        }
        private void proceedCssFiles(CSSobj objA, CSSobj objB)
        {
            int lengthA = objA.CSSrules.Count();
            for (int i = 0; i < lengthA; i++)
            {
                int sameRule = objB.CSSrules.FindIndex(x => x.isChecked == false && x.selectorHASH == objA.CSSrules[i].selectorHASH);
                if (sameRule == -1) continue;
                SelectorRule objBcur = objB.CSSrules[sameRule];
                objBcur.selectorEqTo = i;
                objBcur.isChecked = true;
                SelectorRule objAcur = objA.CSSrules[i];
                objAcur.selectorEqTo = sameRule;
                objAcur.isChecked = true;
                int lengthRules = objAcur.Rules.Count();
                for (var j = 0; j < lengthRules; j++)
                {
                    string key = objAcur.Rules[j].name;
                    var foundIndex = objBcur.Rules.FindAll(x => x.name == key);
                    if (foundIndex == null) continue;
                    for (int k = 0; k < foundIndex.Count(); k++)
                    {
                        if (foundIndex[k].value != objAcur.Rules[j].value) continue;
                        int index = objBcur.Rules.FindIndex(x => x == foundIndex[k]);
                        objBcur.hasRules[index] = true;
                        objAcur.hasRules[j] = true;
                    }
                }
            }
        }

        private void saveFile(string filePath)
        {
            StringBuilder sb = new StringBuilder();
            var lengthA = objA.CSSrules.Count();
            sb.Append(addLegenda());
            for (int i = 0; i < lengthA; i++)
            {
                if (objA.CSSrules[i].selectorEqTo == -1)
                {
                    sb.Append(appendSelector(objA.CSSrules[i], true));
                    continue;
                }
                var eqNum = objA.CSSrules[i].selectorEqTo;
                if (!(objA.CSSrules[i].hasRules.Any(x => x == false) || objB.CSSrules[eqNum].hasRules.Any(x => x == false))) continue;
                sb.Append(string.Join(",\r\n", objA.CSSrules[i].Selector))
                    .Append(" {\r\n\t");
                for (int j = 0; j < objA.CSSrules[i].Rules.Count(); j++)
                {
                    if (objA.CSSrules[i].hasRules[j]) continue;
                    sb.Append("\r\n-\t" + objA.CSSrules[i].Rules[j]);
                }
                for (int j = 0; j < objB.CSSrules[eqNum].Rules.Count(); j++)
                {
                    if (objB.CSSrules[eqNum].hasRules[j]) continue;
                    sb.Append("\r\n+\t" + objB.CSSrules[eqNum].Rules[j]);
                }
                sb.Append("\r\n\t}\r\n");
            }
            for (int i = 0, lengthB = objB.CSSrules.Count(); i < lengthB; i++)
            {
                if (objB.CSSrules[i].selectorEqTo == -1)
                {
                    sb.Append(appendSelector(objB.CSSrules[i], false));
                    continue;
                }
            }
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }
        private StringBuilder addLegenda()
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
        private StringBuilder appendSelector(SelectorRule objRule, bool isMinus)
        {
            StringBuilder sb = new StringBuilder();
            string header = isMinus ?
                "---------------exists in first file, but not exists in second---------------" :
                "+++++++++++++++++   not exists in first file, but exists in second++++++++++++";
            string footer = isMinus ? "--------------------------" : "+++++++++++++++++++++++";
            sb.Append("\r\n/*" + header + "*/" + "\r\n");
            if (objRule.Media != null)
                sb.Append("@media "+objRule.Media + "{\r\n");
            sb.Append(string.Join(",\r\n", objRule.Selector))
                .Append(" {\r\n\t")
                .Append(string.Join("\r\n\t", objRule.Rules))
                .Append("\r\n\t}\r\n");
            if (objRule.Media != null)
                sb.Append("}");
            sb.Append("\r\n/*" + footer + "*/" + "\r\n");
            return sb;
        }
    }
}
