namespace MyTestProject
{
    partial class Report1
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group2 = new Telerik.Reporting.Group();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            this.groupFooterSection1 = new Telerik.Reporting.GroupFooterSection();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.groupHeaderSection1 = new Telerik.Reporting.GroupHeaderSection();
            this.salesCreditCaptionTextBox = new Telerik.Reporting.TextBox();
            this.salesDebitCaptionTextBox = new Telerik.Reporting.TextBox();
            this.yearCaptionTextBox = new Telerik.Reporting.TextBox();
            this.manufacturerCaptionTextBox = new Telerik.Reporting.TextBox();
            this.modelCaptionTextBox = new Telerik.Reporting.TextBox();
            this.labelsGroupFooter = new Telerik.Reporting.GroupFooterSection();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.labelsGroupHeader = new Telerik.Reporting.GroupHeaderSection();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.modelDataTextBox = new Telerik.Reporting.TextBox();
            this.manufacturerDataTextBox = new Telerik.Reporting.TextBox();
            this.yearDataTextBox = new Telerik.Reporting.TextBox();
            this.salesDebitDataTextBox = new Telerik.Reporting.TextBox();
            this.salesCreditDataTextBox = new Telerik.Reporting.TextBox();
            this.objectDataSource1 = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // groupFooterSection1
            // 
            this.groupFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.groupFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox3});
            this.groupFooterSection1.Name = "groupFooterSection1";
            this.groupFooterSection1.PrintOnEveryPage = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0416666679084301D), Telerik.Reporting.Drawing.Unit.Inch(0.10007842630147934D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.258333683013916D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox3.Value = "=Fields.Manufacturer";
            // 
            // groupHeaderSection1
            // 
            this.groupHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D);
            this.groupHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.salesCreditCaptionTextBox,
            this.salesDebitCaptionTextBox,
            this.yearCaptionTextBox,
            this.manufacturerCaptionTextBox,
            this.modelCaptionTextBox});
            this.groupHeaderSection1.Name = "groupHeaderSection1";
            this.groupHeaderSection1.PrintOnEveryPage = true;
            // 
            // salesCreditCaptionTextBox
            // 
            this.salesCreditCaptionTextBox.CanGrow = true;
            this.salesCreditCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.1875D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.salesCreditCaptionTextBox.Name = "salesCreditCaptionTextBox";
            this.salesCreditCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2666666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.salesCreditCaptionTextBox.StyleName = "Caption";
            this.salesCreditCaptionTextBox.Value = "Sales Credit";
            // 
            // salesDebitCaptionTextBox
            // 
            this.salesDebitCaptionTextBox.CanGrow = true;
            this.salesDebitCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8958332538604736D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.salesDebitCaptionTextBox.Name = "salesDebitCaptionTextBox";
            this.salesDebitCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2666666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.salesDebitCaptionTextBox.StyleName = "Caption";
            this.salesDebitCaptionTextBox.Value = "Sales Debit";
            // 
            // yearCaptionTextBox
            // 
            this.yearCaptionTextBox.CanGrow = true;
            this.yearCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.6145832538604736D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.yearCaptionTextBox.Name = "yearCaptionTextBox";
            this.yearCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2666666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.yearCaptionTextBox.StyleName = "Caption";
            this.yearCaptionTextBox.Value = "Year";
            // 
            // manufacturerCaptionTextBox
            // 
            this.manufacturerCaptionTextBox.CanGrow = true;
            this.manufacturerCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3229166269302368D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.manufacturerCaptionTextBox.Name = "manufacturerCaptionTextBox";
            this.manufacturerCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2666666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.manufacturerCaptionTextBox.StyleName = "Caption";
            this.manufacturerCaptionTextBox.Value = "Manufacturer";
            // 
            // modelCaptionTextBox
            // 
            this.modelCaptionTextBox.CanGrow = true;
            this.modelCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0416666679084301D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.modelCaptionTextBox.Name = "modelCaptionTextBox";
            this.modelCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2666666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.modelCaptionTextBox.StyleName = "Caption";
            this.modelCaptionTextBox.Value = "Model";
            // 
            // labelsGroupFooter
            // 
            this.labelsGroupFooter.Height = Telerik.Reporting.Drawing.Unit.Pixel(32.403793334960938D);
            this.labelsGroupFooter.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2});
            this.labelsGroupFooter.Name = "labelsGroupFooter";
            this.labelsGroupFooter.Style.Visible = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(7.85986558184959E-05D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999597787857056D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox2.Value = "=Sum(Fields.SalesCredit)";
            // 
            // labelsGroupHeader
            // 
            this.labelsGroupHeader.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28117132186889648D);
            this.labelsGroupHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1});
            this.labelsGroupHeader.Name = "labelsGroupHeader";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D), Telerik.Reporting.Drawing.Unit.Inch(7.8678131103515625E-05D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.0999608039855957D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox1.Value = "=Fields.Manufacturer";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28121063113212585D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.modelDataTextBox,
            this.manufacturerDataTextBox,
            this.yearDataTextBox,
            this.salesDebitDataTextBox,
            this.salesCreditDataTextBox});
            this.detail.Name = "detail";
            // 
            // modelDataTextBox
            // 
            this.modelDataTextBox.CanGrow = true;
            this.modelDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.modelDataTextBox.Name = "modelDataTextBox";
            this.modelDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2666666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.28121063113212585D));
            this.modelDataTextBox.StyleName = "Data";
            this.modelDataTextBox.Value = "=Fields.Model";
            // 
            // manufacturerDataTextBox
            // 
            this.manufacturerDataTextBox.CanGrow = true;
            this.manufacturerDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3083332777023315D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.manufacturerDataTextBox.Name = "manufacturerDataTextBox";
            this.manufacturerDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2666666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.28121063113212585D));
            this.manufacturerDataTextBox.StyleName = "Data";
            this.manufacturerDataTextBox.Value = "=Fields.Manufacturer";
            // 
            // yearDataTextBox
            // 
            this.yearDataTextBox.CanGrow = true;
            this.yearDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.5958333015441895D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.yearDataTextBox.Name = "yearDataTextBox";
            this.yearDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2666666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.28121063113212585D));
            this.yearDataTextBox.StyleName = "Data";
            this.yearDataTextBox.Value = "=Fields.Year";
            // 
            // salesDebitDataTextBox
            // 
            this.salesDebitDataTextBox.CanGrow = true;
            this.salesDebitDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8833332061767578D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.salesDebitDataTextBox.Name = "salesDebitDataTextBox";
            this.salesDebitDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2666666507720947D), Telerik.Reporting.Drawing.Unit.Inch(0.28121063113212585D));
            this.salesDebitDataTextBox.StyleName = "Data";
            this.salesDebitDataTextBox.Value = "=Fields.SalesDebit";
            // 
            // salesCreditDataTextBox
            // 
            this.salesCreditDataTextBox.CanGrow = true;
            this.salesCreditDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.salesCreditDataTextBox.Name = "salesCreditDataTextBox";
            this.salesCreditDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999600172042847D), Telerik.Reporting.Drawing.Unit.Inch(0.28121063113212585D));
            this.salesCreditDataTextBox.StyleName = "Data";
            this.salesCreditDataTextBox.Value = "=Fields.SalesCredit";
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(RestServiceTelerik.Reports.Cars);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // Report1
            // 
            this.DataSource = this.objectDataSource1;
            group1.GroupFooter = this.groupFooterSection1;
            group1.GroupHeader = this.groupHeaderSection1;
            group1.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.Manufacturer"));
            group1.Name = "group1";
            group2.GroupFooter = this.labelsGroupFooter;
            group2.GroupHeader = this.labelsGroupHeader;
            group2.Name = "labelsGroup";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1,
            group2});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.groupHeaderSection1,
            this.groupFooterSection1,
            this.labelsGroupHeader,
            this.labelsGroupFooter,
            this.detail});
            this.Name = "Report1";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Title")});
            styleRule1.Style.Color = System.Drawing.Color.Black;
            styleRule1.Style.Font.Bold = true;
            styleRule1.Style.Font.Italic = false;
            styleRule1.Style.Font.Name = "Tahoma";
            styleRule1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(20D);
            styleRule1.Style.Font.Strikeout = false;
            styleRule1.Style.Font.Underline = false;
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Caption")});
            styleRule2.Style.Color = System.Drawing.Color.Black;
            styleRule2.Style.Font.Name = "Tahoma";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(11D);
            styleRule2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Data")});
            styleRule3.Style.Font.Name = "Tahoma";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(11D);
            styleRule3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("PageInfo")});
            styleRule4.Style.Font.Name = "Tahoma";
            styleRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(11D);
            styleRule4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.5D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.ObjectDataSource objectDataSource1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox modelDataTextBox;
        private Telerik.Reporting.TextBox manufacturerDataTextBox;
        private Telerik.Reporting.TextBox yearDataTextBox;
        private Telerik.Reporting.TextBox salesDebitDataTextBox;
        private Telerik.Reporting.TextBox salesCreditDataTextBox;
        private Telerik.Reporting.Group labelsGroup;
        private Telerik.Reporting.GroupFooterSection labelsGroupFooter;
        private Telerik.Reporting.GroupHeaderSection labelsGroupHeader;
        private Telerik.Reporting.Group group1;
        private Telerik.Reporting.GroupFooterSection groupFooterSection1;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection1;
        private Telerik.Reporting.TextBox salesCreditCaptionTextBox;
        private Telerik.Reporting.TextBox salesDebitCaptionTextBox;
        private Telerik.Reporting.TextBox yearCaptionTextBox;
        private Telerik.Reporting.TextBox manufacturerCaptionTextBox;
        private Telerik.Reporting.TextBox modelCaptionTextBox;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
    }
}