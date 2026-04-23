using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Defines various constant values and utility properties used for sizing and spacing 
    /// of individual nodes and their controls within the Dialogue Graph Editor.
    /// </summary>
    public static class DialogueNodeConstants
    {
        /// <summary>
        /// The minimum width for a node window in editor pixels.
        /// </summary>
        public static readonly float MinWindowWidth = 160f;

        /// <summary>
        /// The minimum height for a node window in editor pixels.
        /// </summary>
        public static readonly float MinWindowHeight = 70f;

        /// <summary>
        /// Horizontal padding applied inside the node body.
        /// </summary>
        public static readonly float HorizontalPadding = 25f;

        /// <summary>
        /// Vertical padding applied inside the node body.
        /// </summary>
        public static readonly float VerticalPadding = 25f;

        /// <summary>
        /// Gets the standard height of a single line of control (from <see cref="EditorGUIUtility"/>).
        /// </summary>
        public static float LineHeight => EditorGUIUtility.singleLineHeight;

        /// <summary>
        /// Gets the standard vertical spacing between controls (from <see cref="EditorGUIUtility"/>).
        /// </summary>
        public static float VerticalSpacing => EditorGUIUtility.standardVerticalSpacing;

        /// <summary>
        /// Extra padding added to the standard line height to calculate the size of edge buttons.
        /// </summary>
        public static readonly float EdgeButtonPadding = 6f;

        /// <summary>
        /// Calculated size for the square edge buttons (e.g., connection ports).
        /// </summary>
        public static float EdgeButtonSize => Mathf.Round(LineHeight + EdgeButtonPadding);

        /// <summary>
        /// Standard horizontal gap between controls within the node body.
        /// </summary>
        public static readonly float ControlGapX = 8f;

        /// <summary>
        /// Standard vertical gap between controls within the node body.
        /// </summary>
        public static readonly float ControlGapY = 6f;

        /// <summary>
        /// Minimum width for dropdown menus and popup controls.
        /// </summary>
        public static readonly float PopupMinWidth = 180f;

        /// <summary>
        /// Minimum height for standard buttons.
        /// </summary>
        public static readonly float ButtonMinHeight = 20f;

        /// <summary>
        /// Minimum width for text area controls.
        /// </summary>
        public static readonly float TextAreaMinWidth = 240f;

        /// <summary>
        /// Calculates the minimum height for a row, ensuring it meets minimum button/line height requirements.
        /// </summary>
        /// <param name="measured">The calculated height of the row's content.</param>
        public static float RowHeight(float measured) =>
            Mathf.Max(measured, Mathf.Max(ButtonMinHeight, LineHeight));

        /// <summary>
        /// Calculates the height for the node's title area, based on the style.
        /// </summary>
        /// <param name="measuredStyleLineHeight">The line height as measured by the title style.</param>
        public static float TitleHeight(float measuredStyleLineHeight) =>
            Mathf.Max(LineHeight, measuredStyleLineHeight > 0 ? measuredStyleLineHeight : LineHeight);


        private static GUIStyle titleStyle;
        /// <summary>
        /// Custom <see cref="GUIStyle"/> for drawing the node title in the chrome area.
        /// </summary>
        public static GUIStyle TitleStyle
        {
            get
            {
                if (titleStyle != null) return titleStyle;
                titleStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.LowerCenter,
                    clipping = TextClipping.Clip,
                    wordWrap = false
                };
                return titleStyle;
            }
        }
    }
}