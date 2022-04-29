using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace TaslerComputing.EditorOptions
{
	[Export(typeof(IWpfTextViewCreationListener))]
	[ContentType("text")]
	[TextViewRole(PredefinedTextViewRoles.Structured)]
	class OutliningUndoViewCreationListener : IWpfTextViewCreationListener
	{
		public void TextViewCreated(IWpfTextView textView)
		{
			textView.Options.SetOptionValue(DefaultTextViewOptions.OutliningUndoOptionId, false);
		}
	}
}
