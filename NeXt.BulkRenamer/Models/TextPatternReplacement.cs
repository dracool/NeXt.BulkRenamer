using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NeXt.BulkRenamer.Models
{
    internal class TextPatternReplacement : ITextReplacement
    {
        //syntax:
        //uppercase: <+VALUE> \+VALUE
        //lowercase: <-VALUE> \-VALUE
        //trim:      <?VALUE> \?VALUE
        //capitlize: <$VALUE> \$VALUE
        
        // regex explanation: 
        // Modifier Symbol:             [+-?$]     
        // Identiier Symbol:            [\p{L}\p{N}]
        // Modifier List:               [+-?$]*     
        // Identiier:                   [\p{L}\p{N}]+
        // identifier with modifier:    [+-?$]*[\p{L}\p{N}]+
        // <> type group:               <[+-?$]*[\p{L}\p{N}]+?>
        // \ type group:                \\[+-?$]*[\p{L}\p{N}]+
        // actual regex:                <> type or \ type
        
        /// <summary>
        /// gets matches for group value replacements
        /// </summary>
        private static readonly Regex FindGroups = new Regex("<[+-?$]*[\\p{L}\\p{N}]+?>|\\\\[+-?$]*[\\p{L}\\p{N}]+", RegexOptions.Compiled);
        
        private class Group
        {

            private enum GroupKind
            {
                Text,
                IndexedGroup,
                NamedGroup,
            }

            private enum GroupAction
            {
                Capitalize, // $
                Uppercase,  // +
                Lowecase,   // -
                Trim,       // ?
            }

            /// <summary>
            /// creates a replacement group that outputs static text
            /// </summary>
            public static Group FromText(string text)
            {
                return new Group(text, -1, GroupKind.Text, new GroupAction[0]);
            }

            public static Group Create(string text)
            {
                var list = new List<GroupAction>();

                var cindex = 0;
                while (cindex < text.Length)
                {
                    switch (text[cindex])
                    {
                        case '+':
                            list.Add(GroupAction.Uppercase);
                            break;
                        case '-':
                            list.Add(GroupAction.Lowecase);
                            break;
                        case '?':
                            list.Add(GroupAction.Trim);
                            break;
                        case '$':
                            list.Add(GroupAction.Capitalize);
                            break;
                        default: goto GetName;
                    }
                    cindex++;
                }
                GetName:
                if (cindex > text.Length) return null;
                var name = text.Substring(cindex);

                return int.TryParse(name, out var index) 
                    ? new Group(null, index, GroupKind.IndexedGroup, list.AsReadOnly()) 
                    : new Group(name, -1, GroupKind.NamedGroup, list.AsReadOnly());
            }

            private Group(string text, int index, GroupKind kind, IReadOnlyList<GroupAction> actions)
            {
                Text = text;
                Index = index;
                Kind = kind;
                this.actions = actions;
            }

            private int Index { get; }
            private string Text { get; }
            private GroupKind Kind { get; }

            private readonly IReadOnlyList<GroupAction> actions;

            public string Transform(GroupCollection groups)
            {
                string input;
                switch (Kind)
                {
                    case GroupKind.IndexedGroup:
                        input = groups[Index].Value;
                        break;
                    case GroupKind.NamedGroup:
                        input = groups[Text].Value;
                        break;
                    default: return Text;
                }

                foreach (var action in actions)
                {
                    switch (action)
                    {
                        case GroupAction.Capitalize:
                            if (input.Length > 0)
                            {
                                input = input[0].ToString().ToUpper() + input.Substring(1);
                            }
                            break;
                        case GroupAction.Uppercase:
                            input = input.ToUpper();
                            break;
                        case GroupAction.Lowecase:
                            input = input.ToLower();
                            break;
                        case GroupAction.Trim:
                            input = input.Trim();
                            break;
                    }
                }
                return input;
            }
        }

        public TextPatternReplacement(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                groups = new List<Group>().AsReadOnly();
                return;
            }

            var list = new List<Group>();

            var lastIndex = 0;

            foreach (var match in FindGroups.Matches(pattern).Cast<Match>())
            {
                var len = match.Index - lastIndex;
                if (len > 0)
                {
                    list.Add(Group.FromText(pattern.Substring(lastIndex, len)));
                }

                var text = match.Value;
                if(string.IsNullOrEmpty(text)) continue;

                text = text[0] == '\\' 
                    ? text.Substring(1) 
                    : text.Substring(1, text.Length - 2);

                list.Add(Group.Create(text));

                lastIndex = match.Index + match.Length;

            }

            var remainderLength = pattern.Length - lastIndex;
            if (remainderLength > 0)
            {
                list.Add(Group.FromText(pattern.Substring(lastIndex, remainderLength)));
            }
            
            groups = list.AsReadOnly();
        }
        
        private readonly IReadOnlyList<Group> groups;

        public string Apply(string input, Regex regex)
        {
            if (groups.Count == 0)
            {
                return input;
            }

            var match = regex.Match(input);

            var sb = new StringBuilder();

            foreach (var group in groups)
            {
                sb.Append(group.Transform(match.Groups));
            }

            return sb.ToString();
        }
    }
}