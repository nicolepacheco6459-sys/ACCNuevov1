using System;
using System.Collections.Generic;
using System.Linq;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class DialogueSceneModel
    {
        private readonly StartNodeModel _starterNode;

        private BaseDialogueNodeModel _currentNode;

        private readonly Dictionary<Guid, BaseDialogueNodeModel> _dialogueMap = new();

        public DialogueSceneModel(DialogueData data)
        {
            _starterNode = new StartNodeModel(data.StartNode);
            _dialogueMap = data.Nodes.ToDictionary(id => id.NodeId.Value, data => data.CreateModel());
        }

        public BaseDialogueNodeModel GetNextNode()
        {
            if (_currentNode == null)
            {
                _currentNode = _starterNode;
            }
            else
            {
                Guid nextNodeId = _currentNode.GetNextNode();
                _dialogueMap.TryGetValue(nextNodeId, out _currentNode);
            }

            return _currentNode;
        }
    }
}
