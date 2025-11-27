using UnityEditor;
using GraphProcessor;

public class GraphProcessorMenuItems : NodeGraphProcessorMenuItems
{
	[MenuItem("Game/Graph Processor/Create Node Script", false, MenuItemPosition.afterCreateScript)]
	private static void CreateNodeCSharpScritpt() => CreateDefaultNodeCSharpScritpt();
	
	[MenuItem("Game/Graph Processor/Create Node View Script", false, MenuItemPosition.afterCreateScript + 1)]
	private static void CreateNodeViewCSharpScritpt() => CreateDefaultNodeViewCSharpScritpt();

	// To add your C# script creation with you own templates, use ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, defaultFileName)
}