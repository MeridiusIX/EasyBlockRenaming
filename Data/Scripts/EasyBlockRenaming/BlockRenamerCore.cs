using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System.Collections.Generic;
using System.Text;
using VRage.Game.Components;
using VRage.Utils;

namespace EasyBlockRenaming {

	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]

	public class BlockRenamerCore : MySessionComponentBase {

		Dictionary<IMyTerminalBlock, string> TempStringRenames = new Dictionary<IMyTerminalBlock, string>();

		List<IMyTerminalControl> ControlsListMaster = null;

		bool SetupDone = false;

		public override void UpdateBeforeSimulation() {

			if (SetupDone == false) {

				SetupDone = true;
				ControlsListMaster = CreateControlList();
				MyAPIGateway.TerminalControls.CustomControlGetter += AddControlsToBlocks;
				MyAPIGateway.Utilities.InvokeOnGameThread(() => { this.SetUpdateOrder(MyUpdateOrder.NoUpdate); });

			}

		}

		public void AddControlsToBlocks(IMyTerminalBlock block, List<IMyTerminalControl> controls) {

			if (block == null)
				return;

			foreach (var control in ControlsListMaster)
				controls.Add(control);
		
		}

		public List<IMyTerminalControl> CreateControlList() {

			var controlList = new List<IMyTerminalControl>();

			//Separator
			var separator = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSeparator, IMyTerminalBlock>("Renamer_Separator");
			separator.Enabled = (Block) => { return true; };
			separator.SupportsMultipleBlocks = true;
			controlList.Add(separator);

			//Label
			var label = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlLabel, IMyTerminalBlock>("Renamer_Label");
			label.Enabled = (Block) => { return true; };
			label.SupportsMultipleBlocks = true;
			label.Label = MyStringId.GetOrCompute("Block Renaming Controls");
			controlList.Add(label);

			//Textbox
			var textbox = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlTextbox, IMyTerminalBlock>("Renamer_Textbox");
			textbox.Enabled = (Block) => { return true; };
			textbox.SupportsMultipleBlocks = true;
			textbox.Title = MyStringId.GetOrCompute("New Naming");
			textbox.Getter = (Block) => {

				string storedString = "";

				if (TempStringRenames.TryGetValue(Block, out storedString) == false) {

					storedString = "";

				}

				return new StringBuilder(storedString);

			};
			textbox.Setter = (Block, Builder) => {

				string storedString = "";

				if (TempStringRenames.TryGetValue(Block, out storedString) == false) {

					TempStringRenames.Add(Block, Builder.ToString());

				} else {

					TempStringRenames[Block] = Builder.ToString();

				}

			};
			controlList.Add(textbox);

			//Replace Name
			var replaceName = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyTerminalBlock>("Renamer_RenameButton");
			replaceName.Enabled = (Block) => { return true; };
			replaceName.SupportsMultipleBlocks = true;
			replaceName.Title = MyStringId.GetOrCompute("Replace Name");
			replaceName.Action = (Block) => {

				string storedString = "";

				if (TempStringRenames.TryGetValue(Block, out storedString) == false) {

					storedString = "";

				}

				Block.CustomName = storedString;

			};
			controlList.Add(replaceName);

			//Prefix Name
			var prefixName = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyTerminalBlock>("Renamer_PrefixButton");
			prefixName.Enabled = (Block) => { return true; };
			prefixName.SupportsMultipleBlocks = true;
			prefixName.Title = MyStringId.GetOrCompute("Prefix Name");
			prefixName.Action = (Block) => {

				string storedString = "";

				if (TempStringRenames.TryGetValue(Block, out storedString) == false) {

					storedString = "";

				}

				storedString += Block.CustomName;
				Block.CustomName = storedString;

			};
			controlList.Add(prefixName);

			//Suffix Name
			var suffixName = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyTerminalBlock>("Renamer_SuffixButton");
			suffixName.Enabled = (Block) => { return true; };
			suffixName.SupportsMultipleBlocks = true;
			suffixName.Title = MyStringId.GetOrCompute("Suffix Name");
			suffixName.Action = (Block) => {

				string storedString = "";

				if (TempStringRenames.TryGetValue(Block, out storedString) == false) {

					storedString = "";

				}

				Block.CustomName += storedString;

			};
			controlList.Add(suffixName);

			//Reset Name
			var resetName = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlButton, IMyTerminalBlock>("Renamer_ResetButton");
			resetName.Enabled = (Block) => { return true; };
			resetName.SupportsMultipleBlocks = true;
			resetName.Title = MyStringId.GetOrCompute("Reset Name");
			resetName.Action = (Block) => {

				Block.CustomName = Block.DefinitionDisplayNameText;

			};
			controlList.Add(resetName);

			return controlList;

		}

		protected override void UnloadData() {

			MyAPIGateway.TerminalControls.CustomControlGetter -= AddControlsToBlocks;

		}

	}

}
