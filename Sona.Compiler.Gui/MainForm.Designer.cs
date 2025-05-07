using System.Drawing;
using System.Windows.Forms;

namespace Sona.Compiler.Gui
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SplitContainer diagnosticsSplit;
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            ToolStripMenuItem saveMenuButton;
            ToolStripMenuItem saveAsMenuButton;
            ToolStripMenuItem loadMenuButton;
            MenuStrip menuStrip;
            codeSplit = new SplitContainer();
            sonaRichText = new DoubleBufferedRichTextBox();
            resultRichText = new DoubleBufferedRichTextBox();
            sourceToolStrip = new ToolStrip();
            blockDelimitersButton = new ToolStripButton();
            adjustLineNumbersButton = new ToolStripButton();
            progressBar = new ToolStripProgressBar();
            lineWrapButton = new ToolStripButton();
            messageBox = new TextBox();
            runMenuButton = new ToolStripMenuItem();
            statusStrip = new StatusStrip();
            lineLabel = new ToolStripStatusLabel();
            editorToolStrip = new ToolStrip();
            zoomButton = new ToolStripButton();
            orientationButton = new ToolStripButton();
            saveFileDialog = new SaveFileDialog();
            openFileDialog = new OpenFileDialog();
            diagnosticsSplit = new SplitContainer();
            saveMenuButton = new ToolStripMenuItem();
            saveAsMenuButton = new ToolStripMenuItem();
            loadMenuButton = new ToolStripMenuItem();
            menuStrip = new MenuStrip();
            ((System.ComponentModel.ISupportInitialize)diagnosticsSplit).BeginInit();
            diagnosticsSplit.Panel1.SuspendLayout();
            diagnosticsSplit.Panel2.SuspendLayout();
            diagnosticsSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)codeSplit).BeginInit();
            codeSplit.Panel1.SuspendLayout();
            codeSplit.Panel2.SuspendLayout();
            codeSplit.SuspendLayout();
            sourceToolStrip.SuspendLayout();
            menuStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            editorToolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // diagnosticsSplit
            // 
            diagnosticsSplit.Dock = DockStyle.Fill;
            diagnosticsSplit.FixedPanel = FixedPanel.Panel2;
            diagnosticsSplit.Location = new Point(0, 49);
            diagnosticsSplit.Name = "diagnosticsSplit";
            diagnosticsSplit.Orientation = Orientation.Horizontal;
            // 
            // diagnosticsSplit.Panel1
            // 
            diagnosticsSplit.Panel1.Controls.Add(codeSplit);
            // 
            // diagnosticsSplit.Panel2
            // 
            diagnosticsSplit.Panel2.Controls.Add(messageBox);
            diagnosticsSplit.Size = new Size(817, 451);
            diagnosticsSplit.SplitterDistance = 358;
            diagnosticsSplit.TabIndex = 0;
            // 
            // codeSplit
            // 
            codeSplit.Dock = DockStyle.Fill;
            codeSplit.Location = new Point(0, 0);
            codeSplit.Name = "codeSplit";
            codeSplit.Orientation = Orientation.Horizontal;
            // 
            // codeSplit.Panel1
            // 
            codeSplit.Panel1.Controls.Add(sonaRichText);
            // 
            // codeSplit.Panel2
            // 
            codeSplit.Panel2.Controls.Add(resultRichText);
            codeSplit.Panel2.Controls.Add(sourceToolStrip);
            codeSplit.Size = new Size(817, 358);
            codeSplit.SplitterDistance = 174;
            codeSplit.TabIndex = 2;
            // 
            // sonaRichText
            // 
            sonaRichText.AcceptsTab = true;
            sonaRichText.DetectUrls = false;
            sonaRichText.Dock = DockStyle.Fill;
            sonaRichText.Location = new Point(0, 0);
            sonaRichText.Name = "sonaRichText";
            sonaRichText.Size = new Size(817, 174);
            sonaRichText.TabIndex = 3;
            sonaRichText.Text = "";
            sonaRichText.WordWrap = false;
            sonaRichText.ContentsResized += sonaRichText_ContentsResized;
            sonaRichText.SelectionChanged += sonaText_SelectionChanged;
            sonaRichText.TextChanged += sonaText_TextChanged;
            sonaRichText.KeyDown += sonaRichText_KeyDown;
            sonaRichText.KeyPress += sonaRichText_KeyPress;
            sonaRichText.PreviewKeyDown += sonaRichText_PreviewKeyDown;
            // 
            // resultRichText
            // 
            resultRichText.DetectUrls = false;
            resultRichText.Dock = DockStyle.Fill;
            resultRichText.Location = new Point(0, 25);
            resultRichText.Name = "resultRichText";
            resultRichText.ReadOnly = true;
            resultRichText.Size = new Size(817, 155);
            resultRichText.TabIndex = 2;
            resultRichText.Text = "";
            resultRichText.WordWrap = false;
            resultRichText.ContentsResized += resultRichText_ContentsResized;
            // 
            // sourceToolStrip
            // 
            sourceToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            sourceToolStrip.Items.AddRange(new ToolStripItem[] { blockDelimitersButton, adjustLineNumbersButton, progressBar, lineWrapButton });
            sourceToolStrip.Location = new Point(0, 0);
            sourceToolStrip.Name = "sourceToolStrip";
            sourceToolStrip.Size = new Size(817, 25);
            sourceToolStrip.TabIndex = 4;
            sourceToolStrip.Text = "Source";
            // 
            // blockDelimitersButton
            // 
            blockDelimitersButton.CheckOnClick = true;
            blockDelimitersButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            blockDelimitersButton.Image = (Image)resources.GetObject("blockDelimitersButton.Image");
            blockDelimitersButton.ImageTransparentColor = Color.Magenta;
            blockDelimitersButton.Name = "blockDelimitersButton";
            blockDelimitersButton.Size = new Size(127, 22);
            blockDelimitersButton.Text = "Show block delimiters";
            blockDelimitersButton.CheckedChanged += blockDelimitersButton_CheckedChanged;
            // 
            // adjustLineNumbersButton
            // 
            adjustLineNumbersButton.CheckOnClick = true;
            adjustLineNumbersButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            adjustLineNumbersButton.Image = (Image)resources.GetObject("adjustLineNumbersButton.Image");
            adjustLineNumbersButton.ImageTransparentColor = Color.Magenta;
            adjustLineNumbersButton.Name = "adjustLineNumbersButton";
            adjustLineNumbersButton.Size = new Size(165, 22);
            adjustLineNumbersButton.Text = "Preserve source line numbers";
            adjustLineNumbersButton.CheckedChanged += adjustLineNumbersButton_CheckedChanged;
            // 
            // progressBar
            // 
            progressBar.Alignment = ToolStripItemAlignment.Right;
            progressBar.MarqueeAnimationSpeed = 50;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(100, 22);
            // 
            // lineWrapButton
            // 
            lineWrapButton.CheckOnClick = true;
            lineWrapButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            lineWrapButton.Image = (Image)resources.GetObject("lineWrapButton.Image");
            lineWrapButton.ImageTransparentColor = Color.Magenta;
            lineWrapButton.Name = "lineWrapButton";
            lineWrapButton.Size = new Size(69, 22);
            lineWrapButton.Text = "Word wrap";
            lineWrapButton.CheckedChanged += lineWrapButton_CheckedChanged;
            // 
            // messageBox
            // 
            messageBox.Dock = DockStyle.Fill;
            messageBox.Location = new Point(0, 0);
            messageBox.Multiline = true;
            messageBox.Name = "messageBox";
            messageBox.ReadOnly = true;
            messageBox.ScrollBars = ScrollBars.Both;
            messageBox.Size = new Size(817, 89);
            messageBox.TabIndex = 2;
            // 
            // saveMenuButton
            // 
            saveMenuButton.Name = "saveMenuButton";
            saveMenuButton.Size = new Size(43, 20);
            saveMenuButton.Text = "Save";
            saveMenuButton.Click += saveMenuButton_Click;
            // 
            // saveAsMenuButton
            // 
            saveAsMenuButton.Name = "saveAsMenuButton";
            saveAsMenuButton.Size = new Size(66, 20);
            saveAsMenuButton.Text = "Save as...";
            saveAsMenuButton.Click += saveAsMenuButton_Click;
            // 
            // loadMenuButton
            // 
            loadMenuButton.Name = "loadMenuButton";
            loadMenuButton.Size = new Size(45, 20);
            loadMenuButton.Text = "Load";
            loadMenuButton.Click += loadMenuButton_Click;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { saveMenuButton, saveAsMenuButton, loadMenuButton, runMenuButton });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(817, 24);
            menuStrip.TabIndex = 3;
            menuStrip.Text = "Menu";
            // 
            // runMenuButton
            // 
            runMenuButton.Enabled = false;
            runMenuButton.Name = "runMenuButton";
            runMenuButton.Size = new Size(40, 20);
            runMenuButton.Text = "Run";
            runMenuButton.Click += runMenuButton_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { lineLabel });
            statusStrip.Location = new Point(0, 500);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(817, 22);
            statusStrip.TabIndex = 4;
            statusStrip.Text = "Status";
            // 
            // lineLabel
            // 
            lineLabel.Name = "lineLabel";
            lineLabel.Size = new Size(28, 17);
            lineLabel.Text = "1 : 1";
            // 
            // editorToolStrip
            // 
            editorToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            editorToolStrip.Items.AddRange(new ToolStripItem[] { zoomButton, orientationButton });
            editorToolStrip.Location = new Point(0, 24);
            editorToolStrip.Name = "editorToolStrip";
            editorToolStrip.Size = new Size(817, 25);
            editorToolStrip.TabIndex = 2;
            editorToolStrip.Text = "Editor";
            // 
            // zoomButton
            // 
            zoomButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            zoomButton.Image = (Image)resources.GetObject("zoomButton.Image");
            zoomButton.ImageTransparentColor = Color.Magenta;
            zoomButton.Name = "zoomButton";
            zoomButton.Size = new Size(77, 22);
            zoomButton.Text = "zoomButton";
            zoomButton.ToolTipText = "Reset zoom";
            zoomButton.Click += zoomButton_Click;
            // 
            // orientationButton
            // 
            orientationButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            orientationButton.Image = (Image)resources.GetObject("orientationButton.Image");
            orientationButton.ImageTransparentColor = Color.Magenta;
            orientationButton.Name = "orientationButton";
            orientationButton.Size = new Size(105, 22);
            orientationButton.Text = "orientationButton";
            orientationButton.ToolTipText = "Switch view";
            orientationButton.Click += orientationButton_Click;
            // 
            // saveFileDialog
            // 
            saveFileDialog.DefaultExt = "sona";
            saveFileDialog.Filter = "Sona source (*.sona)|*.sona";
            // 
            // openFileDialog
            // 
            openFileDialog.DefaultExt = "sona";
            openFileDialog.Filter = "Sona source (*.sona)|*.sona";
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(817, 522);
            Controls.Add(diagnosticsSplit);
            Controls.Add(editorToolStrip);
            Controls.Add(menuStrip);
            Controls.Add(statusStrip);
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            Text = "Sona";
            diagnosticsSplit.Panel1.ResumeLayout(false);
            diagnosticsSplit.Panel2.ResumeLayout(false);
            diagnosticsSplit.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)diagnosticsSplit).EndInit();
            diagnosticsSplit.ResumeLayout(false);
            codeSplit.Panel1.ResumeLayout(false);
            codeSplit.Panel2.ResumeLayout(false);
            codeSplit.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)codeSplit).EndInit();
            codeSplit.ResumeLayout(false);
            sourceToolStrip.ResumeLayout(false);
            sourceToolStrip.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            editorToolStrip.ResumeLayout(false);
            editorToolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer diagnosticsSplit;
        private SplitContainer codeSplit;
        private TextBox messageBox;
        private DoubleBufferedRichTextBox resultRichText;
        private DoubleBufferedRichTextBox sonaRichText;
        private ToolStrip editorToolStrip;
        private ToolStripButton orientationButton;
        private ToolStripButton zoomButton;
        private ToolStripButton blockDelimitersButton;
        private ToolStripButton adjustLineNumbersButton;
        private MenuStrip menuStrip;
        private ToolStripMenuItem saveMenuButton;
        private ToolStripMenuItem saveAsMenuButton;
        private ToolStripMenuItem loadMenuButton;
        private ToolStripMenuItem runMenuButton;
        private StatusStrip statusStrip;
        private ToolStripProgressBar progressBar;
        private ToolStrip sourceToolStrip;
        private ToolStripStatusLabel lineLabel;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;
        private ToolStripButton lineWrapButton;
    }
}
