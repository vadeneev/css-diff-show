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
            proceedCompareCssFiles(objA, objB);
            button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string filePath;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = saveFileDialog1.InitialDirectory + saveFileDialog1.FileName;
                SaveHelper.saveFile(filePath, objA, objB, comboBox1.SelectedItem.ToString());
            }
        }
        private void showBtn()
        {
            if (objA != null && objB != null)
            {
                button3.Enabled = true;
                comboBox1.Enabled = true;
                comboBox1.Text = "Full export";
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
        private void proceedCompareCssFiles(CSSobj objA, CSSobj objB)
        {
            int lengthA = objA.CSSrules.Count();
            for (int i = 0; i < lengthA; i++)
            {
                var result = Enumerable.Range(0, objB.CSSrules.Count)
                            .Where(t => objB.CSSrules[t].selectorHASH == objA.CSSrules[i].selectorHASH)
                            .ToList();
                if (!result.Any()) continue;
                foreach (int fRule in result)
                {
                    SelectorRule objBcur = objB.CSSrules[fRule];
                    objBcur.selectorEqTo = i;
                    objBcur.isChecked = true;
                    SelectorRule objAcur = objA.CSSrules[i];
                    objAcur.selectorEqTo = fRule;
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
        }       
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
