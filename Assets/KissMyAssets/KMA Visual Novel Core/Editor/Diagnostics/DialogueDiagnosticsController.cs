using KissMyAssets.VisualNovelCore.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Controller responsible for managing and displaying a comprehensive list of diagnostic issues 
    /// found across multiple dialogue scene assets (e.g., in the main Dialogue Creator window).
    /// </summary>
    public sealed class DialogueDiagnosticsController
    {
        /// <summary>
        /// Defines the severity level of a diagnostic issue.
        /// </summary>
        public enum Severity { Info, Warning, Error }

        /// <summary>
        /// Represents a single diagnostic issue with optional fix actions and related assets.
        /// </summary>
        public sealed class Issue
        {
            public Severity Severity;
            public string Message;
            public string FixLabel;                // Label for the fix button (optional)
            public Action Fix;                     // Action to execute for the fix (optional)
            public Object[] Related;               // Related assets (e.g., ScriptableObjects)
        }

        /// <summary>
        /// Structure to hold the configuration asset and its path for diagnostic processing.
        /// </summary>
        public readonly struct ConfigEntry
        {
            public readonly DialogueSceneConfig Config;
            public readonly string Path;
            public ConfigEntry(DialogueSceneConfig config, string path)
            { Config = config; Path = path; }
        }

        // Public counters for the summary
        /// <summary> Gets the total count of error issues. </summary>
        public int ErrorCount { get; private set; }
        /// <summary> Gets the total count of warning issues. </summary>
        public int WarningCount { get; private set; }
        /// <summary> Gets the total count of informational issues. </summary>
        public int InfoCount { get; private set; }

        private readonly List<Issue> _issues = new();
        private readonly GUIStyle _wrapLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogueDiagnosticsController"/> class.
        /// </summary>
        public DialogueDiagnosticsController()
        {
            _wrapLabel = new GUIStyle(EditorStyles.label) { wordWrap = true };
        }

        /// <summary>
        /// Clears all existing issues and resets counters before a new validation cycle.
        /// </summary>
        public void BeginCollect()
        {
            _issues.Clear();
            ErrorCount = WarningCount = InfoCount = 0;
        }

        /// <summary>
        /// Adds a new informational message to the list of issues.
        /// </summary>
        /// <param name="message">The informational text.</param>
        public void AddInfo(string message) =>
            AddIssue(Severity.Info, message, null, null, null);

        /// <summary>
        /// Adds a new warning message to the list of issues.
        /// </summary>
        /// <param name="message">The warning text.</param>
        public void AddWarning(string message) =>
            AddIssue(Severity.Warning, message, null, null, null);

        /// <summary>
        /// Adds a new error message to the list of issues.
        /// </summary>
        /// <param name="message">The error text.</param>
        public void AddError(string message) =>
            AddIssue(Severity.Error, message, null, null, null);

        /// <summary>
        /// Adds a new diagnostic issue with full customization.
        /// </summary>
        /// <param name="severity">The severity level of the issue.</param>
        /// <param name="message">The descriptive message.</param>
        /// <param name="fixLabel">Optional label for a fix button.</param>
        /// <param name="fixAction">Optional action to execute when the fix button is pressed.</param>
        /// <param name="relatedObjects">Optional array of related assets to select/ping.</param>
        public void AddIssue(Severity severity, string message, string fixLabel, Action fixAction, Object[] relatedObjects)
        {
            var issue = new Issue
            {
                Severity = severity,
                Message = message,
                FixLabel = fixLabel,
                Fix = fixAction,
                Related = relatedObjects
            };

            _issues.Add(issue);

            // Update counts
            switch (severity)
            {
                case Severity.Error: ErrorCount++; break;
                case Severity.Warning: WarningCount++; break;
                case Severity.Info: InfoCount++; break;
            }
        }

        /// <summary>
        /// Draws the entire diagnostic panel, iterating through all collected issues.
        /// </summary>
        public void DrawPanel()
        {
            if (!_issues.Any())
            {
                EditorGUILayout.HelpBox("No issues found.", MessageType.Info);
                return;
            }

            foreach (var issue in _issues)
            {
                DrawIssueBlock(issue);
                EditorGUILayout.Space(2);
            }
        }

        /// <summary>
        /// Draws a single diagnostic issue block, including message, severity box, related asset button, and fix button.
        /// </summary>
        /// <param name="issue">The issue to draw.</param>
        private void DrawIssueBlock(Issue issue)
        {
            var msgType = issue.Severity switch
            {
                Severity.Error => MessageType.Error,
                Severity.Warning => MessageType.Warning,
                _ => MessageType.Info
            };

            // Use a vertical layout to stack HelpBox and control buttons
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                // Message block (uses HelpBox to show severity icon)
                EditorGUILayout.HelpBox(issue.Message, msgType);

                // Related asset and fix buttons
                using (new EditorGUILayout.HorizontalScope())
                {
                    // Select Related Button
                    if (issue.Related != null && issue.Related.Length > 0)
                    {
                        if (GUILayout.Button("Select related", GUILayout.Width(120)))
                        {
                            // Select all related objects in the Project window
                            Selection.objects = issue.Related;
                            // Ping the first one to highlight it
                            if (issue.Related.Length == 1) EditorGUIUtility.PingObject(issue.Related[0]);
                        }
                    }

                    GUILayout.FlexibleSpace(); // Pushes the fix button to the right

                    // Fix Button (if available)
                    if (!string.IsNullOrEmpty(issue.FixLabel) && issue.Fix != null)
                    {
                        if (GUILayout.Button(issue.FixLabel, GUILayout.Width(120)))
                            issue.Fix.Invoke();
                    }
                }
            }
        }
    }
}