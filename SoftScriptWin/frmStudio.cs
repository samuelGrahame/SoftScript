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

            richEditControl1.Text = @"a equals 10
b equals 15

decrement b
decrement b
decrement b
decrement b
decrement b

if a variable named a is not equal to a variable named b then 
    a equals b
end

write a to the console
";
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var value = CodeParser.Parse(richEditControl1.Text);

            Executor.ExecuteNodes(value.nodes, value.variableCount);
        }
    }
}