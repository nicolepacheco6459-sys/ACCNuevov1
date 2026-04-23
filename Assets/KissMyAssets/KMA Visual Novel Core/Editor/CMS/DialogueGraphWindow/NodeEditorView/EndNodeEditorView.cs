using KissMyAssets.VisualNovelCore.Runtime;
using System;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    public class EndNodeEditorView : BaseNodeEditorView
    {
        private DialogueEndNodeData _data;

        public EndNodeEditorView(BaseDialogueNodeData data, INodeConnectionController connectionController) : base(data, connectionController)
        {
            _data = data as DialogueEndNodeData;
        }

        // StartNodeEditorView
        protected override void DrawChrome(Rect r)
        {
            ChromeTitle("END", r);
            ChromeEdgeButtonLeft("@", r, () => ConnectionController.OnLink.Invoke(this));
            ChromeCornerButtonTopRight("X", r, () => ConnectionController.RequestDelete(NodeId));
        }

        protected override void DrawBody()
        {

        }

        protected override void OnNodePositionChanged(Vector2 p) { _data.Position = p; }

        public override void BrokeConnection(Guid deletedNodeId) { }
    }
}
