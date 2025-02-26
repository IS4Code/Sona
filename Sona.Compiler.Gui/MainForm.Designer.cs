using System.Drawing;
using System.Windows.Forms;

namespace IS4.Sona.Compiler.Gui
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
            ToolStrip toolStrip;
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            progressBar = new ToolStripProgressBar();
            orientationButton = new ToolStripButton();
            zoomButton = new ToolStripButton();
            codeSplit = new SplitContainer();
            sonaRichText = new DoubleBufferedRichTextBox();
            splitContainer1 = new SplitContainer();
            resultRichText = new DoubleBufferedRichTextBox();
            messageBox = new TextBox();
            toolStrip = new ToolStrip();
            toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)codeSplit).BeginInit();
            codeSplit.Panel1.SuspendLayout();
            codeSplit.Panel2.SuspendLayout();
            codeSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.AddRange(new ToolStripItem[] { orientationButton, zoomButton, progressBar });
            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(800, 25);
            toolStrip.TabIndex = 2;
            toolStrip.Text = "toolStrip";
            // 
            // progressBar
            // 
            progressBar.Alignment = ToolStripItemAlignment.Right;
            progressBar.MarqueeAnimationSpeed = 50;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(100, 22);
            // 
            // orientationButton
            // 
            orientationButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            orientationButton.Image = (Image)resources.GetObject("orientationButton.Image");
            orientationButton.ImageTransparentColor = Color.Magenta;
            orientationButton.Name = "orientationButton";
            orientationButton.Size = new Size(105, 22);
            orientationButton.Text = "orientationButton";
            orientationButton.Click += orientationButton_Click;
            // 
            // zoomButton
            // 
            zoomButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            zoomButton.Image = (Image)resources.GetObject("zoomButton.Image");
            zoomButton.ImageTransparentColor = Color.Magenta;
            zoomButton.Name = "zoomButton";
            zoomButton.Size = new Size(77, 22);
            zoomButton.Text = "zoomButton";
            zoomButton.Click += zoomButton_Click;
            // 
            // codeSplit
            // 
            codeSplit.Dock = DockStyle.Fill;
            codeSplit.Location = new Point(0, 25);
            codeSplit.Name = "codeSplit";
            // 
            // codeSplit.Panel1
            // 
            codeSplit.Panel1.Controls.Add(sonaRichText);
            // 
            // codeSplit.Panel2
            // 
            codeSplit.Panel2.Controls.Add(splitContainer1);
            codeSplit.Size = new Size(800, 425);
            codeSplit.SplitterDistance = 396;
            codeSplit.TabIndex = 0;
            // 
            // sonaRichText
            // 
            sonaRichText.AcceptsTab = true;
            sonaRichText.DetectUrls = false;
            sonaRichText.Dock = DockStyle.Fill;
            sonaRichText.Location = new Point(0, 0);
            sonaRichText.Name = "sonaRichText";
            sonaRichText.Size = new Size(396, 425);
            sonaRichText.TabIndex = 3;
            sonaRichText.Text = "";
            sonaRichText.WordWrap = false;
            sonaRichText.ContentsResized += sonaRichText_ContentsResized;
            sonaRichText.SelectionChanged += sonaText_SelectionChanged;
            sonaRichText.TextChanged += sonaText_TextChanged;
            sonaRichText.KeyPress += sonaRichText_KeyPress;
            sonaRichText.PreviewKeyDown += sonaRichText_PreviewKeyDown;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel2;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(resultRichText);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(messageBox);
            splitContainer1.Size = new Size(400, 425);
            splitContainer1.SplitterDistance = 325;
            splitContainer1.TabIndex = 2;
            // 
            // resultRichText
            // 
            resultRichText.DetectUrls = false;
            resultRichText.Dock = DockStyle.Fill;
            resultRichText.Location = new Point(0, 0);
            resultRichText.Name = "resultRichText";
            resultRichText.ReadOnly = true;
            resultRichText.Size = new Size(400, 325);
            resultRichText.TabIndex = 2;
            resultRichText.Text = "";
            resultRichText.WordWrap = false;
            resultRichText.ContentsResized += resultRichText_ContentsResized;
            // 
            // messageBox
            // 
            messageBox.Dock = DockStyle.Fill;
            messageBox.Location = new Point(0, 0);
            messageBox.Multiline = true;
            messageBox.Name = "messageBox";
            messageBox.ReadOnly = true;
            messageBox.ScrollBars = ScrollBars.Both;
            messageBox.Size = new Size(400, 96);
            messageBox.TabIndex = 2;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(codeSplit);
            Controls.Add(toolStrip);
            Name = "MainForm";
            Text = "Sona";
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            codeSplit.Panel1.ResumeLayout(false);
            codeSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)codeSplit).EndInit();
            codeSplit.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer codeSplit;
        private SplitContainer splitContainer1;
        private TextBox messageBox;
        private DoubleBufferedRichTextBox resultRichText;
        private DoubleBufferedRichTextBox sonaRichText;
        private ToolStrip toolStrip;
        private ToolStripButton orientationButton;
        private ToolStripButton zoomButton;
        private ToolStripProgressBar progressBar;
    }
}
