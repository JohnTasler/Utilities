using System;
using System.Windows.Forms;
using Tasler.Windows.Input;


namespace RawInputExplorer
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			TreeNode devicesNode = this.treeView.Nodes["rawInputDevicesNode"];

			TreeNode mouseDevicesNode = devicesNode.Nodes["mouseDevicesNode"];
			foreach (RawInputMouseDeviceInfo mouseInfo in RawInput.MouseDevices)
			{
				TreeNode deviceNode = new TreeNode(mouseInfo.Name);
				deviceNode.Tag = mouseInfo;
				mouseDevicesNode.Nodes.Add(deviceNode);
			}

			TreeNode keyboardDevicesNode = devicesNode.Nodes["keyboardDevicesNode"];
			foreach (RawInputKeyboardDeviceInfo keyboardInfo in RawInput.KeyboardDevices)
			{
				TreeNode deviceNode = new TreeNode(keyboardInfo.Name);
				deviceNode.Tag = keyboardInfo;
				keyboardDevicesNode.Nodes.Add(deviceNode);
			}

			TreeNode humanInterfaceDevicesNode = devicesNode.Nodes["humanInterfaceDevicesNode"];
			foreach (RawInputHumanInterfaceDeviceInfo hidInfo in RawInput.HumanInterfaceDevices)
			{
				TreeNode deviceNode = new TreeNode(hidInfo.Name);
				deviceNode.Tag = hidInfo;
				humanInterfaceDevicesNode.Nodes.Add(deviceNode);
			}

			// Perform default processing
			base.OnLoad(e);
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			this.propertyGrid.SelectedObject = this.treeView.SelectedNode.Tag;
		}
	}
}