using System;
using System.Collections.Generic;
using System.Text;

namespace MyGeneration.AutoCompletion
{
    class VBScriptAutoCompleteHelper : AutoCompleteHelper
    {
        public override bool IsCommentStyle(bool isTagged, int style)
        {
            return (style == (isTagged ? 82 : 1));
        }

        public override bool IsCodeStyle(bool isTagged, int style)
        {
            return (style == (isTagged ? 86 : 7));
        }
    }
}
