using System.Collections.Generic;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Controller for collecting and managing temporary, frame-specific diagnostic issues 
    /// for a single node in the Dialogue Graph Editor.
    /// It enforces a "once per frame" rule to prevent duplicate error messages.
    /// </summary>
    public class NodeDiagnosticsController
    {
        /// <summary>
        /// Defines the severity level for node-level diagnostics (only Warning and Error are typically used).
        /// </summary>
        public enum Severity { Warning, Error }

        // Tuple list to store the actual issues
        private readonly List<(Severity sev, string code, string msg)> items = new();

        // Hash set to track codes and ensure "once per frame"
        private readonly HashSet<string> codes = new();

        /// <summary>
        /// Gets a read-only list of the collected issues for the current frame.
        /// </summary>
        public IReadOnlyList<(Severity sev, string code, string msg)> Items => items;

        /// <summary>
        /// Checks if there are any issues collected for the current frame.
        /// </summary>
        public bool IsEmpty => items.Count == 0;

        /// <summary>
        /// Prepares the controller for a new frame's drawing cycle by clearing all previous issues.
        /// This should be called at the beginning of the node's <c>DrawNode</c> method.
        /// </summary>
        public void BeginFrame()
        {
            items.Clear();
            codes.Clear();
        }

        /// <summary>
        /// Adds a warning issue to the list. If an issue with the same unique code 
        /// has already been added in the current frame, it is ignored.
        /// </summary>
        /// <param name="code">A unique identifier for this specific warning (e.g., "choice.nooption").</param>
        /// <param name="message">The warning message to display.</param>
        public void WarnOnce(string code, string message)
        {
            if (codes.Add(code))
                items.Add((Severity.Warning, code, message));
        }

        /// <summary>
        /// Adds an error issue to the list. If an issue with the same unique code 
        /// has already been added in the current frame, it is ignored.
        /// </summary>
        /// <param name="code">A unique identifier for this specific error (e.g., "startnode.noconnection").</param>
        /// <param name="message">The error message to display.</param>
        public void ErrorOnce(string code, string message)
        {
            if (codes.Add(code))
                items.Add((Severity.Error, code, message));
        }
    }
}