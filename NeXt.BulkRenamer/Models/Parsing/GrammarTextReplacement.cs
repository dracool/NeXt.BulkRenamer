using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NeXt.BulkRenamer.Models.Background;
using Sprache;

namespace NeXt.BulkRenamer.Models.Parsing
{
    internal class GrammarTextReplacement : IReplacement
    {
        private static IEnumerable<DefaultTagFormat> TokenizeTagFormats(IEnumerable<char> values)
        {
            return values.Select(val =>
            {
                switch (val)
                {
                    case '+': return DefaultTagFormat.UpperCase;
                    case '-': return DefaultTagFormat.LowerCase;
                    case '$': return DefaultTagFormat.Capitalize;
                    case '?': return DefaultTagFormat.Trim;
                    default: throw new InvalidOperationException("Invalid tag format value");
                }
            });
        }

        private const char ShortTagOpen = '\\';
        private const char LongTagOpen = '<';
        private const char LongTagFormatDelimiter = ':';
        private const char LongTagSpecialIdentifierIndicator = '%';
        private const char LongTagClose = '>';

        private static readonly char[] ShortFormats = { '+', '-', '$', '?' };
        private static readonly string AnyFormatTerminatingCharacter = new string(new[] { ShortTagOpen, LongTagOpen, LongTagClose });
        private static readonly string AnyOpenCharacter = new string(new[] { ShortTagOpen, LongTagOpen });
        
        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private static readonly Parser<string> TagIndexedIdentifier;
        private static readonly Parser<string> TagNamedIdentifier;
        private static readonly Parser<string> TagIdentifier;
        private static readonly Parser<string> SpecialIdentifier;
                 
        private static readonly Parser<string> LongSpecialFormat;
                 
        private static readonly Parser<IResultPart> ShortTag;
        private static readonly Parser<IResultPart> LongDefaultTag;
        private static readonly Parser<IResultPart> LongSpecialTag;
        private static readonly Parser<IResultPart> Constant;
                 
        private static readonly Parser<IEnumerable<IResultPart>> Complete;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

        static GrammarTextReplacement()
        {
            TagIndexedIdentifier = from digits in Parse.Digit.AtLeastOnce().Text()
                                   select digits;

            TagNamedIdentifier = from first in Parse.Letter.Once()
                                 from rest in Parse.LetterOrDigit.Many()
                                 select new string(first.Concat(rest).ToArray());

            TagIdentifier = TagIndexedIdentifier.Or(TagNamedIdentifier);
            
            SpecialIdentifier = from indicator in Parse.Char(LongTagSpecialIdentifierIndicator)
                                from identifier in TagNamedIdentifier
                                select identifier;
            
            ShortTag = from open in Parse.Char(ShortTagOpen)
                       from identifier in TagIdentifier
                       from format in Parse.Chars(ShortFormats).Many()
                       select Results.CreateDefaultTag(identifier, TokenizeTagFormats(format));
            
            LongDefaultTag = from open in Parse.Char(LongTagOpen)
                             from identifier in TagIdentifier
                             from format in Parse.Chars(ShortFormats).Many()
                             from close in Parse.Char(LongTagClose)
                             select Results.CreateDefaultTag(identifier, TokenizeTagFormats(format));

            LongSpecialFormat = from delimiter in Parse.Char(LongTagFormatDelimiter)
                                from format in Parse.CharExcept(AnyFormatTerminatingCharacter).Many().Text()
                                select format;

            LongSpecialTag = from open in Parse.Char(LongTagOpen)
                             from identifier in SpecialIdentifier
                             from format in LongSpecialFormat.Optional()
                             from close in Parse.Char(LongTagClose)
                             select Results.CreateSpecialTag(identifier, format.GetOrDefault());

            Constant = from text in Parse.CharExcept(AnyOpenCharacter).AtLeastOnce().Text()
                       select (IResultPart)new ConstantResultPart(text);

            Complete = ShortTag
                .XOr(LongSpecialTag)
                .Or(LongDefaultTag)
                .Or(Constant)
                .XAtLeastOnce()
                .End();
        }

        private readonly IReadOnlyList<IResultPart> parts;

        public GrammarTextReplacement(string pattern)
        {
            parts = Complete.Parse(pattern).ToArray();
        }
        
        public string Apply(Regex regex, string name, FileInfo file)
        {
            var m = regex.Match(name);

            var sb = new StringBuilder();

            foreach (var part in parts)
            {
                sb.Append(part.Process(m.Groups, file));
            }

            return sb.ToString();
        }
    }
}
