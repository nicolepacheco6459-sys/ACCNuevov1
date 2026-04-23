using KissMyAssets.VisualNovelCore.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    public class ChoiceReplicaNodeEditorView : BaseNodeEditorView
    {
        private readonly DialogueChoiceReplicaNodeData _data;

        /// <summary>
        /// The list of visual representations for each choice option.
        /// </summary>
        private readonly List<ChoiceOptionEditorView> _options = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceNodeEditorView"/> class.
        /// </summary>
        /// <param name="data">The dialogue node data, expected to be <see cref="DialogueChoiceNodeData"/>.</param>
        /// <param name="connectionController">The controller for handling node connections.</param>
        public ChoiceReplicaNodeEditorView(BaseDialogueNodeData data, INodeConnectionController connectionController) : base(data, connectionController)
        {
            _data = data as DialogueChoiceReplicaNodeData;

            SynchronizeOptionsFromData();
        }

        /// <summary>
        /// Synchronizes the internal list of <see cref="ChoiceOptionEditorView"/> with the underlying <see cref="DialogueChoiceNodeData.Options"/>.
        /// </summary>
        private void SynchronizeOptionsFromData()
        {
            // Initial synchronization with data: create a view for each data option.
            foreach (var opt in _data.Options)
            {
                var view = new ChoiceOptionEditorView(opt, ConnectionController);
                view.OnRemoveRequested += () => RemoveOption(_options.IndexOf(view));
                _options.Add(view);
            }
        }

        // === CHROME ===

        /// <summary>
        /// Draws the node title, the connection input button (left edge), and the delete button (top-right corner).
        /// </summary>
        /// <param name="r">The rectangle representing the node window.</param>
        protected override void DrawChrome(Rect r)
        {
            ChromeTitle("REPLICA AND CHOICE", r);
            ChromeEdgeButtonLeft("@", r, () => ConnectionController.OnLink?.Invoke(this));
            ChromeCornerButtonTopRight("X", r, () => ConnectionController.RequestDelete(NodeId));
        }

        // === BODY ===

        //// <summary>
        /// Draws the main body content of the choice node, including the background dropdown and all choice options.
        /// </summary>
        protected override void DrawBody()
        {
            if (_data == null)
                return;

            PartBackgroundDropdown(_data.BackgroundHolder);
            PartSpace(DialogueNodeConstants.ControlGapY);

            PartActorsList(_data.ActorsHolder);
            PartSpace(DialogueNodeConstants.ControlGapY);

            DrawTextAreaControl();
            PartSpace(DialogueNodeConstants.ControlGapY);

            DrawOptions();

            PartSpace(4f);
            PartButton("+ Add option", AddOption);
        }

        private void DrawTextAreaControl()
        {
            PartTextArea(_data.Text, newText =>
            {
                _data.Text = newText;
            }, minLines: 1);
        }

        /// <summary>
        /// Iterates through and draws all choice options, accumulating their measured size.
        /// </summary>
        private void DrawOptions()
        {
            for (int i = 0; i < _options.Count; i++)
                DrawOptionBlock(i);
        }

        /// <summary>
        /// Draws or measures a single choice option block at the specified index.
        /// </summary>
        /// <param name="index">The index of the option to draw.</param>
        private void DrawOptionBlock(int index)
        {
            var optionView = _options[index];
            // Calculate the available width for the option body, considering padding
            float bodyWidth = Mathf.Max(280f, CachedSize.x - 2f * DialogueNodeConstants.HorizontalPadding);

            // Pass dependencies for the current frame
            // Note: BodyOffsetWorld calculation here seems incorrect for GUILayout elements and might be intended for absolute positioning.
            Vector2 bodyOffsetWorld = new Vector2(Position.x, Position.y);

            optionView.BodyWidth = bodyWidth;
            optionView.BodyOffsetWorld = bodyOffsetWorld;

            // Actual drawing/measuring of the option
            optionView.DrawOptionBlock(IsMeasuring);

            // Accumulate size: during measurement, update window size based on the option block's measured height
            if (IsMeasuring)
            {
                AccumulateSize(new Vector2(bodyWidth, optionView.LastMeasuredHeight));
                PartSpace(6f);
            }
            else
            {
                PartSpace(6f);
            }
        }

        // === CONNECTIONS ===

        /// <summary>
        /// Overrides <see cref="BaseNodeEditorView.DrawConnection"/> to draw connections for all choice options.
        /// </summary>
        public override void DrawConnection()
        {
            for (int i = 0; i < _options.Count; i++)
            {
                _options[i].DrawConnection();
            }

        }

        // === SERVICE/UTILITIES ===

        /// <summary>
        /// Adds a new choice option to the node data and creates the corresponding editor view.
        /// </summary>
        private void AddOption()
        {
            var optData = new ChoiceOptionData();
            _data.Options.Add(optData);

            var view = new ChoiceOptionEditorView(optData, ConnectionController);
            view.OnRemoveRequested += () => RemoveOption(_options.IndexOf(view));

            _options.Add(view);
        }

        /// <summary>
        /// Removes a choice option from the node data and its editor view by index.
        /// </summary>
        /// <param name="index">The index of the option to remove.</param>
        private void RemoveOption(int index)
        {
            if (index < 0 || index >= _options.Count)
                return;

            // Remove from data first
            _data.Options.RemoveAt(index);
            // Remove from view list
            _options.RemoveAt(index);
        }

        /// <summary>
        /// Collects diagnostics for the choice node, checking for missing options, empty text, or broken connections.
        /// </summary>
        protected override void CollectDiagnostics()
        {
            if (_options.Count == 0)
                Diagnostics.ErrorOnce("choice.nooption", "Choice node has no options.");

            for (int i = 0; i < _data.Options.Count; i++)
            {
                var option = _data.Options[i];
                if (string.IsNullOrWhiteSpace(option.Text))
                    Diagnostics.WarnOnce($"option.{i}.emptytext", $"Option {i + 1} has empty text.");
                if (option.RelatedNodeId == Guid.Empty)
                    Diagnostics.WarnOnce($"option.{i}.noconnection", $"Option {i + 1} is not connected to any node.");
            }
        }

        /// <summary>
        /// Updates the position of the underlying data when the node is dragged in the editor.
        /// </summary>
        /// <param name="newPosition">The new position of the node.</param>
        protected override void OnNodePositionChanged(Vector2 newPosition)
        {
            if (_data != null) _data.Position = newPosition;
        }

        /// <summary>
        /// Propagates the broken connection event to all individual choice options.
        /// </summary>
        /// <param name="deletedNodeId">The ID of the node that was deleted.</param>
        public override void BrokeConnection(Guid deletedNodeId)
        {
            foreach (var option in _options)
            {
                option.BrokeConnection(deletedNodeId);
            }
        }
    }
}