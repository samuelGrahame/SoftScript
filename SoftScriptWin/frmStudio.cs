using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using SoftScript.Stuidio;
using DevExpress.XtraRichEdit.Services;
using System.Diagnostics;

namespace SoftScript
{
    public partial class frmStudio : DevExpress.XtraEditors.XtraForm
    {
        public frmStudio()
        {
            InitializeComponent();
        }

        private void frmStudio_Load(object sender, EventArgs e)
        {
            richEditControl1.ReplaceService<ISyntaxHighlightService>(new CustomSyntaxHighlightService(richEditControl1.Document));
            //richEditControl1.Document.Sections[0].Page.Width = Units.InchesToDocumentsF(80f);
            //richEditControl1.Document.DefaultCharacterProperties.FontName = "Courier New";

            richEditControl1.Text = @"a equals 1
b equals 100000

while a is smaller then b loop
	
	increment a
next
";
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var value = CodeParser.Parse(richEditControl1.Text);
            var sw = Stopwatch.StartNew();
            Executor.ExecuteNodes(value.nodes, value.variableCount);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " ms");

        }
    }
}