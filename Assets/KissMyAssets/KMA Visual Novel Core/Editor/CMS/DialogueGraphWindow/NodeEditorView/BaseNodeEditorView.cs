using KissMyAssets.VisualNovelCore.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{

    /// <summary>
    /// Base class for the visual representation of a node in the dialogue editor.
    /// It implements the core logic for drawing the node window, measuring its size, 
    /// and handling diagnostics panels and common UI controls.
    /// </summary>
    public abstract class BaseNodeEditorView
    {
        private readonly BaseDialogueNodeData _nodeData;

        /// <summary>
        /// The current position of the node in the editor view.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The cached size (width and height) of the node, calculated during measurement.
        /// </summary>
        public Vector2 CachedSize { get; private set; } = new Vector2(DialogueNodeConstants.MinWindowWidth, DialogueNodeConstants.MinWindowHeight);

        /// <summary>
        /// Controller for managing node connections.
        /// </summary>
        protected INodeConnectionController ConnectionController;

        private float contentHeight;
        private float contentMaxWidth;

        /// <summary>
        /// Flag indicating if the node is currently in the size measurement phase.
        /// </summary>
        protected bool IsMeasuring;

        /// <summary>
        /// Controller for collecting and displaying diagnostic messages (warnings/errors) for the node.
        /// </summary>
        protected readonly NodeDiagnosticsController Diagnostics = new NodeDiagnosticsController();

        /// <summary>
        /// The unique identifier of the node.
        /// </summary>
        public Guid NodeId => _nodeData.NodeId;

        /// <summary>
        /// Handles the event when a connection is broken with another node.
        /// </summary>
        /// <param name="deletedNodeId">The ID of the node with which the connection was broken.</param>
        public abstract void BrokeConnection(Guid deletedNodeId);

        /// <summary>
        /// Constructor for the base node editor view.
        /// </summary>
        /// <param name="nodeData">The underlying dialogue node data.</param>
        /// <param name="connectionController">The connection controller.</param>
        protected BaseNodeEditorView(BaseDialogueNodeData nodeData, INodeConnectionController connectionController)
        {
            this._nodeData = nodeData;
            ConnectionController = connectionController;

            if (_nodeData != null)
                Position = _nodeData.Position;
        }

        /// <summary>
        /// Virtual method to draw the "chrome" (title, edge buttons) of the node window.
        /// </summary>
        /// <param name="windowRect">The rect representing the node window.</param>
        protected virtual void DrawChrome(Rect windowRect) { }

        /// <summary>
        /// Abstract method to draw the main content body of the node.
        /// Must be implemented in derived classes.
        /// </summary>
        protected abstract void DrawBody();

        /// <summary>
        /// The main method to draw the entire node in the editor.
        /// It triggers size measurement, draws the GUI window, and handles position changes.
        /// </summary>
        public virtual void DrawNode()
        {
            Diagnostics.BeginFrame();

            MesureNodeViewSize();

            Rect rect = new Rect(Position, CachedSize);

            rect = GUI.Window(GetHashCode(), rect, id =>
            {
                DrawChrome(rect);

                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                GUILayout.Space(DialogueNodeConstants.VerticalPadding);
                GUILayout.BeginHorizontal();
                GUILayout.Space(DialogueNodeConstants.HorizontalPadding);
                GUILayout.BeginVertical();

                DrawBody();
                CollectDiagnostics();
                PartDiagnosticsPanel();

                GUILayout.EndVertical();
                GUILayout.Space(DialogueNodeConstants.HorizontalPadding);
                GUILayout.EndHorizontal();
                GUILayout.Space(DialogueNodeConstants.VerticalPadding - DialogueNodeConstants.VerticalSpacing);
                GUILayout.EndVertical();

                GUI.DragWindow();
            }, GUIContent.none);

            if (rect.position != Position)
            {
                Position = rect.position;
                OnNodePositionChanged(Position);
            }
        }

        /// <summary>
        /// Measures the required size to display the node's content by calling <see cref="DrawBody"/> 
        /// and UI parts while <see cref="IsMeasuring"/> is true.
        /// </summary>
        private void MesureNodeViewSize()
        {
            IsMeasuring = true;
            ResetMeasurement();
            DrawBody();
            DrawConnection();
            CollectDiagnostics();
            PartDiagnosticsPanel();
            FinalizeMeasurement();
            IsMeasuring = false;
        }

        /// <summary>
        /// Called when the node's position has been changed by the user dragging the window.
        /// </summary>
        /// <param name="newPosition">The new position of the node.</param>
        protected virtual void OnNodePositionChanged(Vector2 newPosition) { }

        /// <summary>
        /// Virtual method to draw the node's output connections.
        /// </summary>
        public virtual void DrawConnection() { }

        /// <summary>
        /// Resets the current content size measurement values.
        /// </summary>
        protected void ResetMeasurement()
        {
            contentHeight = 0f;
            contentMaxWidth = 0f;
            CachedSize = new Vector2(DialogueNodeConstants.MinWindowWidth, 0f);
        }

        /// <summary>
        /// Finalizes the measurement phase and sets the final cached size of the node (<see cref="CachedSize"/>).
        /// </summary>
        protected void FinalizeMeasurement()
        {
            float width = Mathf.Max(DialogueNodeConstants.MinWindowWidth, contentMaxWidth + DialogueNodeConstants.HorizontalPadding * 2f);
            float height = Mathf.Max(DialogueNodeConstants.MinWindowHeight, contentHeight + DialogueNodeConstants.VerticalPadding * 2f);
            CachedSize = new Vector2(width, height);
        }

        /// <summary>
        /// Accumulates the size of a UI element to the total content height and updates the maximum content width.
        /// Used during the measurement phase.
        /// </summary>
        /// <param name="elementSize">The size (width, height) of the element.</param>
        /// <param name="addVerticalGap">Whether to add the standard vertical spacing after the element.</param>
        protected void AccumulateSize(Vector2 elementSize, bool addVerticalGap = true)
        {
            contentMaxWidth = Mathf.Max(contentMaxWidth, elementSize.x);
            contentHeight += DialogueNodeConstants.RowHeight(elementSize.y);
            if (addVerticalGap) contentHeight += DialogueNodeConstants.VerticalSpacing;
        }

        /// <summary>
        /// Draws (or measures) a bold title label.
        /// </summary>
        /// <param name="text">The title text.</param>
        protected void PartTitle(string text)
        {
            var content = new GUIContent(text);
            Vector2 measured = EditorStyles.boldLabel.CalcSize(content);
            if (!IsMeasuring) GUILayout.Label(content, EditorStyles.boldLabel);
            AccumulateSize(measured);
        }

        /// <summary>
        /// Draws (or measures) a standard text label.
        /// </summary>
        /// <param name="text">The label text.</param>
        protected void PartLabel(string text)
        {
            var content = new GUIContent(text);
            Vector2 measured = EditorStyles.label.CalcSize(content);
            if (!IsMeasuring) GUILayout.Label(content, EditorStyles.label);
            AccumulateSize(measured);
        }

        /// <summary>
        /// Draws (or measures) a mini text label.
        /// </summary>
        /// <param name="text">The mini label text.</param>
        protected void PartMiniLabel(string text)
        {
            var content = new GUIContent(text);
            Vector2 measured = EditorStyles.miniLabel.CalcSize(content);
            if (!IsMeasuring) GUILayout.Label(content, EditorStyles.miniLabel);
            AccumulateSize(measured);
        }

        /// <summary>
        /// Draws (or measures) a standard button.
        /// </summary>
        /// <param name="label">The text on the button.</param>
        /// <param name="onClick">The action to execute when the button is clicked (only outside measuring mode).</param>
        /// <returns>Returns true if the button was clicked.</returns>
        protected bool PartButton(string label, Action onClick = null)
        {
            var content = new GUIContent(label);
            Vector2 measured = GUI.skin.button.CalcSize(content);
            float height = DialogueNodeConstants.RowHeight(measured.y);

            bool clicked = false;
            if (!IsMeasuring)
                clicked = GUILayout.Button(content, GUILayout.Height(height));

            if (clicked && onClick != null)
                onClick();

            AccumulateSize(new Vector2(measured.x, height));
            return clicked;
        }

        /// <summary>
        /// Draws (or measures) a row containing two buttons side-by-side.
        /// </summary>
        /// <param name="leftLabel">Text for the left button.</param>
        /// <param name="onLeftClick">Action for the left button.</param>
        /// <param name="rightLabel">Text for the right button.</param>
        /// <param name="onRightClick">Action for the right button.</param>
        protected void PartButtonsRow(string leftLabel, Action onLeftClick, string rightLabel, Action onRightClick)
        {
            var leftContent = new GUIContent(leftLabel);
            var rightContent = new GUIContent(rightLabel);
            Vector2 leftSize = GUI.skin.button.CalcSize(leftContent);
            Vector2 rightSize = GUI.skin.button.CalcSize(rightContent);

            float rowHeight = DialogueNodeConstants.RowHeight(Mathf.Max(leftSize.y, rightSize.y));
            float rowWidth = leftSize.x + DialogueNodeConstants.ControlGapX + rightSize.x;

            if (!IsMeasuring)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(leftContent, GUILayout.Height(rowHeight))) onLeftClick?.Invoke();
                if (GUILayout.Button(rightContent, GUILayout.Height(rowHeight))) onRightClick?.Invoke();
                GUILayout.EndHorizontal();
            }

            AccumulateSize(new Vector2(rowWidth, rowHeight));
        }

        /// <summary>
        /// Draws (or measures) a vertical space.
        /// </summary>
        /// <param name="pixels">The height of the space in pixels.</param>
        protected void PartSpace(float pixels)
        {
            if (!IsMeasuring) GUILayout.Space(pixels);
            AccumulateSize(new Vector2(0f, pixels), addVerticalGap: false);
        }

        /// <summary>
        /// Draws (or measures) a multi-line text area (TextArea).
        /// </summary>
        /// <param name="value">The current text value.</param>
        /// <param name="onChanged">Action called when the text is changed.</param>
        /// <param name="minLines">Minimum number of lines to display.</param>
        /// <param name="maxLines">Maximum number of lines to display (0 for no limit).</param>
        /// <returns>The updated text value.</returns>
        protected string PartTextArea(string value, Action<string> onChanged = null, int minLines = 4, int maxLines = 0)
        {
            string currentValue = value ?? string.Empty;
            var content = new GUIContent(currentValue);

            float contentWidth = Mathf.Max(DialogueNodeConstants.TextAreaMinWidth, contentMaxWidth);
            float calculatedHeight = EditorStyles.textArea.CalcHeight(content, contentWidth);

            float minHeight = Mathf.Max(DialogueNodeConstants.RowHeight(DialogueNodeConstants.LineHeight) * minLines, DialogueNodeConstants.ButtonMinHeight * minLines);
            if (maxLines > 0)
            {
                float maxHeight = DialogueNodeConstants.RowHeight(DialogueNodeConstants.LineHeight) * maxLines;
                calculatedHeight = Mathf.Min(calculatedHeight, maxHeight);
            }
            calculatedHeight = Mathf.Max(calculatedHeight, minHeight);

            if (!IsMeasuring)
            {
                EditorGUI.BeginChangeCheck();
                string newText = EditorGUILayout.TextArea(currentValue, EditorStyles.textArea, GUILayout.MinHeight(calculatedHeight));
                if (EditorGUI.EndChangeCheck())
                {
                    currentValue = newText;
                    onChanged?.Invoke(newText);
                }
            }

            if (string.IsNullOrWhiteSpace(currentValue))
                Diagnostics.WarnOnce("textarea.empty", "Text is empty.");

            AccumulateSize(new Vector2(contentWidth, calculatedHeight));
            return currentValue;
        }


        /// <summary>
        /// Draws (or measures) content inside a Box style container.
        /// </summary>
        /// <param name="inner">Action containing the drawing logic for the content inside the box.</param>
        protected void PartBox(Action inner)
        {
            var padding = GUI.skin.box.padding;

            PartSpace(padding.top);

            if (!IsMeasuring) GUILayout.BeginVertical(GUI.skin.box);
            inner?.Invoke();
            if (!IsMeasuring) GUILayout.EndVertical();

            PartSpace(padding.bottom);

            contentMaxWidth += padding.left + padding.right;
        }

        /// <summary>
        /// Draws (or measures) a dropdown/popup for selecting a background from a configuration.
        /// </summary>
        protected void PartBackgroundDropdown(BackgroundHolderData backgroundHolderData)
        {
            BackgroundsInfoConfig config = BackgroundsInfoConfig.Instance;

            if (config == null || config.Entries == null || config.Entries.Count == 0)
            {
                PartMiniLabel("No backgrounds configured.");
                return;
            }

            Guid currentId = backgroundHolderData.BackgroundId;

            string label = "Background";
            string emptyText = "—";

            int count = config.Entries.Count;
            string[] names = new string[count + 1];
            Guid[] ids = new Guid[count + 1];

            names[0] = emptyText;
            ids[0] = Guid.Empty;

            int selected = 0;
            for (int i = 0; i < count; i++)
            {
                var entry = config.Entries[i];
                Guid id = entry.Guid;
                string name = entry.Data != null ? entry.Data.Name : $"Background {i + 1}";

                names[i + 1] = string.IsNullOrEmpty(name) ? $"Background {i + 1}" : name;
                ids[i + 1] = id;

                if (id == currentId) selected = i + 1;
            }

            int newIndex = PartPopup(label, selected, names);
            Guid result = (newIndex >= 0 && newIndex < ids.Length) ? ids[newIndex] : currentId;

            backgroundHolderData.BackgroundId = result;
        }

        /// <summary>
        /// Draws (or measures) a standard Unity Editor dropdown/popup control.
        /// </summary>
        /// <param name="label">The label displayed before the dropdown.</param>
        /// <param name="selectedIndex">The current selected index.</param>
        /// <param name="options">The array of string options.</param>
        /// <param name="onChanged">Action called when the selection changes.</param>
        /// <returns>The newly selected index.</returns>
        protected int PartPopup(string label, int selectedIndex, string[] options, Action<int> onChanged = null)
        {
            string shown = (options != null && options.Length > 0 && selectedIndex >= 0 && selectedIndex < options.Length)
                ? options[selectedIndex] : "—";

            var labelContent = string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label);
            var valueContent = new GUIContent(shown);

            Vector2 labelSize = string.IsNullOrEmpty(label) ? Vector2.zero : EditorStyles.label.CalcSize(labelContent);
            Vector2 popupSize = EditorStyles.popup.CalcSize(valueContent);

            float rowHeight = DialogueNodeConstants.RowHeight(popupSize.y);
            float rowWidth = labelSize.x + DialogueNodeConstants.ControlGapX + Mathf.Max(DialogueNodeConstants.PopupMinWidth, popupSize.x);

            if (!IsMeasuring)
            {
                GUILayout.BeginHorizontal();
                if (!string.IsNullOrEmpty(label))
                    GUILayout.Label(labelContent, GUILayout.Height(rowHeight));

                EditorGUI.BeginChangeCheck();
                int newIndex = EditorGUILayout.Popup(selectedIndex, options ?? Array.Empty<string>(), GUILayout.Height(rowHeight));
                if (EditorGUI.EndChangeCheck())
                {
                    selectedIndex = newIndex;
                    onChanged?.Invoke(selectedIndex);
                }
                GUILayout.EndHorizontal();
            }

            AccumulateSize(new Vector2(rowWidth, rowHeight));
            return selectedIndex;
        }

        /// <summary>
        /// Draws (or measures) the list of actors associated with the node.
        /// Allows adding, removing actors, and changing their ID, alignment, and emotion.
        /// Also handles setting the speaker (first actor in the list).
        /// </summary>
        /// <param name="holder">The object holding the list of actor data.</param>
        protected void PartActorsList(ActorsHolderData holder)
        {
            if (!ValidateActorsHolder(holder))
                return;

            if (!ValidateActorsConfig(out ActorsInfoConfig config))
                return;

            PartTitle("Actors");

            List<DialogueActorData> list = holder.Actors ?? new List<DialogueActorData>();
            holder.Actors = list; // Important to return the reference/new list to the data holder

            for (int i = 0; i < list.Count; i++)
            {
                DrawSingleActor(i, list, config);
            }

            PartButton("+ Add actor", () => list.Add(new DialogueActorData()));

            holder.Actors = list;
        }

        private bool ValidateActorsHolder(ActorsHolderData holderData)
        {
            if (holderData == null)
            {
                PartMiniLabel("ActorsHolder is null");
                return false;
            }

            return true;
        }

        private bool ValidateActorsConfig(out ActorsInfoConfig config)
        {
            config = ActorsInfoConfig.Instance;

            if (config == null || config.Entries == null || config.Entries.Count == 0)
            {
                PartMiniLabel("No actors configured.");
                return false;
            }

            return true;
        }

        private void DrawSingleActor(int index, List<DialogueActorData> list, ActorsInfoConfig config)
        {
            DialogueActorData row = list[index];

            var entries = config.Entries;

            int selectedActorIndex = 0;
            for (int k = 1; k < entries.Count; k++)
            {
                if (entries[k].Guid == row.ActorDataId.Value)
                {
                    if (entries[k].Data.Sprites.Count == 0)
                    {
                        selectedActorIndex = 0;
                        break;
                    }

                    selectedActorIndex = k;
                    break;
                }
            }

            var removeContent = new GUIContent("✕");
            var speakerContent = new GUIContent("Speaker");

            Vector2 removeSize = GUI.skin.button.CalcSize(removeContent);
            Vector2 speakerSize = GUI.skin.button.CalcSize(speakerContent);

            string alignText = row.Alignment.ToString();
            string emotionText = row.ActorEmotion.ToString();
            string selectedActorName = entries[Mathf.Clamp(selectedActorIndex, 0, entries.Count - 1)].Data.Name;

            Vector2 actorPopupSize = new Vector2(Mathf.Max(DialogueNodeConstants.PopupMinWidth, EditorStyles.popup.CalcSize(new GUIContent(selectedActorName)).x), DialogueNodeConstants.LineHeight);
            Vector2 alignPopupSize = EditorStyles.popup.CalcSize(new GUIContent(alignText));
            Vector2 emotionPopupSize = EditorStyles.popup.CalcSize(new GUIContent(emotionText));

            float controlsMaxHeight = Mathf.Max(removeSize.y, speakerSize.y, actorPopupSize.y, alignPopupSize.y, emotionPopupSize.y);
            float rowHeight = DialogueNodeConstants.RowHeight(controlsMaxHeight);

            float rowWidth =
                removeSize.x
                + DialogueNodeConstants.ControlGapX
                + actorPopupSize.x
                + DialogueNodeConstants.ControlGapX
                + alignPopupSize.x
                + DialogueNodeConstants.ControlGapX
                + emotionPopupSize.x
                + DialogueNodeConstants.ControlGapX
                + speakerSize.x;

            if (!IsMeasuring)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(removeContent, GUILayout.Height(rowHeight)))
                {
                    list.RemoveAt(index);
                    index--;
                    GUILayout.EndHorizontal();
                    return;
                }

                var validEntries = entries.Where(entry => entry.Data.Sprites.Count != 0).ToList();

                EditorGUI.BeginChangeCheck();
                int newSellectedActor = EditorGUILayout.Popup(
                    selectedActorIndex, validEntries.Select(entry => entry.Data.Name).ToArray(),
                    GUILayout.Width(actorPopupSize.x), GUILayout.Height(rowHeight));

                if (EditorGUI.EndChangeCheck())
                    row.ActorDataId = (newSellectedActor >= 0 && newSellectedActor < entries.Count) ? entries[newSellectedActor].Guid : row.ActorDataId;

                var selectedEntries = entries[newSellectedActor];

                EditorGUI.BeginChangeCheck();
                var newAlign = (EActorAlignmentType)EditorGUILayout.EnumPopup(
                    row.Alignment, GUILayout.Width(alignPopupSize.x), GUILayout.Height(rowHeight));

                if (EditorGUI.EndChangeCheck())
                    row.Alignment = newAlign;

                List<EActorEmotionType> curentActorValidEmotions = selectedEntries.Data.Sprites.Select(spr => spr.Emotion).ToList();

                int newEmotionIndex = curentActorValidEmotions.IndexOf(row.ActorEmotion);

                if (newEmotionIndex == -1)
                    newEmotionIndex = 0;

                EditorGUI.BeginChangeCheck();
                newEmotionIndex = EditorGUILayout.Popup(
                    newEmotionIndex, curentActorValidEmotions.Select(emotion => emotion.ToString()).ToArray(),
                    GUILayout.Width(emotionPopupSize.x), GUILayout.Height(rowHeight));

                if (EditorGUI.EndChangeCheck())
                    row.ActorDataId = (newSellectedActor >= 0 && newSellectedActor < entries.Count) ? entries[newSellectedActor].Guid : row.ActorDataId;

                if (EditorGUI.EndChangeCheck())
                    row.ActorEmotion = curentActorValidEmotions[newEmotionIndex];

                bool isSpeaker = (index == 0);
                GUI.enabled = !isSpeaker;

                if (GUILayout.Button(speakerContent, GUILayout.Height(rowHeight)))
                    MakeSpeaker(list, index);

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            AccumulateSize(new Vector2(rowWidth, rowHeight));
            PartSpace(DialogueNodeConstants.ControlGapY);
        }


        // Function to move an actor to the speaker position (index 0)
        private void MakeSpeaker(List<DialogueActorData> list, int index)
        {
            if (index < 0 || index >= list.Count) return;
            var item = list[index];
            list.RemoveAt(index);
            list.Insert(0, item);
        }


        /// <summary>
        /// Draws the title within the "chrome" of the node window.
        /// </summary>
        /// <param name="text">The title text.</param>
        /// <param name="windowRect">The rect of the node window.</param>
        protected void ChromeTitle(string text, Rect windowRect)
        {
            var content = new GUIContent(text);
            float titleHeight = DialogueNodeConstants.TitleHeight(DialogueNodeConstants.TitleStyle.lineHeight);
            Rect titleRect = new Rect(0, 0, windowRect.width, titleHeight);
            GUI.Label(titleRect, content, DialogueNodeConstants.TitleStyle);
        }

        /// <summary>
        /// Draws and handles a click on a button placed on the left edge of the node.
        /// </summary>
        /// <param name="symbol">The symbol or text for the button.</param>
        /// <param name="windowRect">The rect of the node window.</param>
        /// <param name="onClick">Action to execute on click.</param>
        /// <returns>Returns true if the button was clicked.</returns>
        protected bool ChromeEdgeButtonLeft(string symbol, Rect windowRect, Action onClick = null)
        {
            float size = DialogueNodeConstants.EdgeButtonSize;
            Rect rect = new Rect(0, (windowRect.height - size) * 0.5f, size, size);
            bool clicked = GUI.Button(rect, symbol);
            if (clicked) onClick?.Invoke();
            return clicked;
        }

        /// <summary>
        /// Draws and handles a click on a button placed on the right edge of the node.
        /// </summary>
        /// <param name="symbol">The symbol or text for the button.</param>
        /// <param name="windowRect">The rect of the node window.</param>
        /// <param name="onClick">Action to execute on click.</param>
        /// <returns>Returns true if the button was clicked.</returns>
        protected bool ChromeEdgeButtonRight(string symbol, Rect windowRect, Action onClick = null)
        {
            float size = DialogueNodeConstants.EdgeButtonSize;
            Rect rect = new Rect(windowRect.width - size, (windowRect.height - size) * 0.5f, size, size);
            bool clicked = GUI.Button(rect, symbol);
            if (clicked) onClick?.Invoke();
            return clicked;
        }

        /// <summary>
        /// Draws and handles a click on a button placed in the top-right corner of the node.
        /// </summary>
        /// <param name="symbol">The symbol or text for the button.</param>
        /// <param name="windowRect">The rect of the node window.</param>
        /// <param name="onClick">Action to execute on click.</param>
        /// <returns>Returns true if the button was clicked.</returns>
        protected bool ChromeCornerButtonTopRight(string symbol, Rect windowRect, Action onClick = null)
        {
            float size = DialogueNodeConstants.EdgeButtonSize;
            Rect r = new Rect(windowRect.width - size, 0f, size, size);
            bool clicked = GUI.Button(r, symbol);
            if (clicked) onClick?.Invoke();
            return clicked;
        }

        /// <summary>
        /// Draws a visual connection line from this node to a target node.
        /// </summary>
        /// <param name="targetNode">The target node to connect to.</param>
        /// <param name="color">Optional color for the connection line.</param>
        /// <param name="thickness">Thickness of the connection line.</param>
        protected void DrawConnectionTo(BaseNodeEditorView targetNode, Color? color = null, float thickness = 3f)
        {
            if (targetNode == null)
            {
                Diagnostics.WarnOnce("connection.missing", "Connection target is not set.");
                return;
            }

            if (IsMeasuring)
                return;

            Rect fromRect = new Rect(Position, CachedSize);
            Rect toRect = new Rect(targetNode.Position, targetNode.CachedSize);

            Vector3 start = new Vector3(fromRect.xMax, fromRect.center.y, 0f);
            Vector3 end = new Vector3(toRect.xMin, toRect.center.y, 0f);

            ConnectionDrawer.DrawConnectionTo(start, end, color, thickness);
        }

        /// <summary>
        /// Virtual method for collecting node-specific diagnostic information (warnings, errors).
        /// To be implemented in derived classes.
        /// </summary>
        protected virtual void CollectDiagnostics() { }

        /// <summary>
        /// Draws (or measures) the diagnostics panel containing all collected warnings and errors.
        /// </summary>
        protected void PartDiagnosticsPanel()
        {
            if (Diagnostics.IsEmpty) return;

            float width = Mathf.Max(contentMaxWidth, DialogueNodeConstants.PopupMinWidth);
            for (int i = 0; i < Diagnostics.Items.Count; i++)
            {
                var it = Diagnostics.Items[i];
                var content = new GUIContent(it.msg);
                float height = EditorStyles.helpBox.CalcHeight(content, width);

                if (!IsMeasuring)
                {
                    var type = it.sev == NodeDiagnosticsController.Severity.Warning ? MessageType.Warning : MessageType.Error;
                    EditorGUILayout.HelpBox(it.msg, type);
                }
                AccumulateSize(new Vector2(width, height));
                PartSpace(DialogueNodeConstants.ControlGapY);
            }
        }
    }
}