namespace RawInputExplorer
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Mouse Devices");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Keyboard Devices");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Human Interface Devices");
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Raw Input Devices", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.treeView = new System.Windows.Forms.TreeView();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.treeView);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.propertyGrid);
			this.splitContainer.Size = new System.Drawing.Size(568, 339);
			this.splitContainer.SplitterDistance = 300;
			this.splitContainer.TabIndex = 0;
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.Location = new System.Drawing.Point(0, 0);
			this.treeView.Name = "treeView";
			treeNode1.Name = "mouseDevicesNode";
			treeNode1.Text = "Mouse Devices";
			treeNode2.Name = "keyboardDevicesNode";
			treeNode2.Text = "Keyboard Devices";
			treeNode3.Name = "humanInterfaceDevicesNode";
			treeNode3.Text = "Human Interface Devices";
			treeNode4.Name = "rawInputDevicesNode";
			treeNode4.Text = "Raw Input Devices";
			this.treeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
			this.treeView.Size = new System.Drawing.Size(300, 339);
			this.treeView.TabIndex = 0;
			this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(264, 339);
			this.propertyGrid.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(568, 339);
			this.Controls.Add(this.splitContainer);
			this.Name = "MainForm";
			this.Text = "Raw Input Explorer";
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.PropertyGrid propertyGrid;
	}
}

