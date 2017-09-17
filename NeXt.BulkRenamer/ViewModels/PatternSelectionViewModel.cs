using System;
using System.Text.RegularExpressions;
using Caliburn.Micro;
using NeXt.BulkRenamer.Models;

namespace NeXt.BulkRenamer.ViewModels
{
    internal class PatternSelectionViewModel : PropertyChangedBase
    {
        private readonly IBackgroundTextReplacement replacer;
        private bool ignoreCase;
        private string regexText;
        private string patternText;
        private bool matchExtension;
        private bool allowWhitespace;
        private bool rightToLeft;

        public PatternSelectionViewModel(IBackgroundTextReplacement replacer)
        {
            this.replacer = replacer;
        }

        public bool MatchExtension
        {
            get => matchExtension;
            set
            {
                matchExtension = value;
                replacer.SetMatchExtension(matchExtension);
            }
        }

        public bool RightToLeft
        {
            get => rightToLeft;
            set
            {
                rightToLeft = value;
                UpdateRegex();
            }
        }

        public bool AllowWhitespace
        {
            get => allowWhitespace;
            set
            {
                allowWhitespace = value;
                UpdateRegex();
            }
        }

        public bool IgnoreCase
        {
            get => ignoreCase;
            set
            {
                ignoreCase = value; 
                UpdateRegex();
            }
        }

        public string RegexText
        {
            get => regexText;
            set
            {
                regexText = value;
                UpdateRegex();
            }
        }

        public string PatternText
        {
            get => patternText;
            set
            {
                patternText = value; 
                UpdatePattern();
            }
        }

        public bool HintVisible { get; private set; }

        public void SetHint(bool show)
        {
            HintVisible = show;
        }

        private void UpdatePattern()
        {
            replacer.SetReplacement(new TextPatternReplacement(PatternText));
        }

        private void UpdateRegex()
        {
            var options = RegexOptions.Compiled;

            if (IgnoreCase) options |= RegexOptions.IgnoreCase;
            if (AllowWhitespace) options |= RegexOptions.IgnorePatternWhitespace;
            if (RightToLeft) options |= RegexOptions.RightToLeft;

            try
            {
                var regex = new Regex(RegexText, options);
                replacer.SetRegex(regex);
            }
            catch (ArgumentException) { }

        }
    }
}
