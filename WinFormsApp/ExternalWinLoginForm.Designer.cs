namespace WinFormsApp;

partial class ExternalWinLoginForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.Windows.Forms.Button button1;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
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
        this.button1 = new System.Windows.Forms.Button();
        this.components = new System.ComponentModel.Container();
        this.button1.Location = new System.Drawing.Point(100, 100);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(100, 30);
        this.button1.TabIndex = 0;
        this.button1.Text = "Show Login";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(this.button1_Click);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "Form1";
        this.Controls.Add(this.button1);
    }

    #endregion
}