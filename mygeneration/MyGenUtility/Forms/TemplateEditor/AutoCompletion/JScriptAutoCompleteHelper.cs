using System;
using System.Collections.Generic;
using System.Text;

namespace MyGeneration.AutoCompletion
{
    class JScriptAutoCompleteHelper : AutoCompleteHelper
    {
        public override bool IsCommentStyle(bool isTagged, int style)
        {
            return (style == (isTagged ? 58 : 2));
        }

        public override bool IsCodeStyle(bool isTagged, int style)
        {
            return (style == (isTagged ? 61 : 11));
        }
    }
}
