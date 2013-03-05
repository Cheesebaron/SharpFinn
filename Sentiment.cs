﻿#region Copyright
// <copyright file="Sentiment.cs">
// This program uses the AFINN sentiment lexicon to calculate sentiment
// for a sentence. More information about AFINN can be found here:
// http://neuro.imm.dtu.dk/wiki/AFINN
//
// Copyright (C) 2013  Tomasz Cielecki
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// 
// Project Lead - Tomasz Cielecki. http://ostebaronen.dk
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SharpFinn
{
    public class Sentiment
    {
        #region Fields
        private static volatile Sentiment _instance;
        private static readonly object SyncRoot = new Object();
        private readonly IDictionary<string, int> _words;
        #endregion

        #region Properties
        public IDictionary<string, int> Words { get { return _words; } }
        public int WordsCount { get { return _words.Count; } }
        #endregion

        public static Sentiment Instance
        {
            get 
            {
                if (_instance == null) 
                {
                    lock (SyncRoot) 
                    {
                        if (_instance == null)
                            _instance = new Sentiment();
                    }
                }

                return _instance;
            }
        }

        #region ctor
        private Sentiment()
        {
            _words = new Dictionary<string, int>();

            using (var file = new StreamReader("db/AFINN-111.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var bits = line.Split('\t');
                    _words.Add(bits[0], int.Parse(bits[1]));
                }
            }
        }
        #endregion

        /// <summary>
        /// Tokenizes a string. This method first removes non-alpha characters,
        /// removes multiple spaces, and lowercases every word. Then splits the
        /// string into an array of words.
        /// </summary>
        /// <param name="input">String to be tokenized</param>
        /// <returns>Array of words (tokens)</returns>
        private static IEnumerable<string> Tokenize(string input)
        {
            input = Regex.Replace(input, "[^a-zA-Z ]+", "");
            input = Regex.Replace(input, @"\s+", " ");
            input = input.ToLower();
            return input.Split(' ');
        }

        /// <summary>
        /// Calculates sentiment score of a sentence
        /// </summary>
        /// <param name="input">Sentence</param>
        /// <returns>Score object</returns>
        public Score GetScore(string input)
        {
            var score = new Score {Tokens = Tokenize(input)};

            foreach (var token in score.Tokens)
            {
                if (!_words.ContainsKey(token)) continue;

                var item = _words[token];
                score.Words.Add(token);

                if (item > 0) score.Positive.Add(token);
                if (item < 0) score.Negative.Add(token);

                score.Sentiment += item;
            }

            return score;
        }

        /// <summary>
        /// Add extra words for scoring
        /// </summary>
        /// <param name="words">Dictionary of string keys and int values</param>
        public void InjectWords(IDictionary<string, int> words)
        {
            foreach (var word in words.Where(word => !_words.ContainsKey(word.Key)))
            {
                _words.Add(word.Key, word.Value);
            }
        }

        #region Inner Score Class
        public class Score
        {
            /// <summary>
            /// Tokens which were scored
            /// </summary>
            public IEnumerable<string> Tokens { get; set; }
            /// <summary>
            /// Total sentiment score of the tokens
            /// </summary>
            public int Sentiment { get; set; }
            /// <summary>
            /// Average sentiment score Sentiment/Tokens.Count
            /// </summary>
            public double AverageSentimentTokens { get { return (double)Sentiment/Tokens.Count(); } }
            /// <summary>
            /// Average sentiment score Sentiment/Words.Count
            /// </summary>
            public double AverageSentimentWords { get { return (double)Sentiment / Words.Count(); } }
            /// <summary>
            /// Words that were used from AFINN
            /// </summary>
            public IList<string> Words { get; set; }
            /// <summary>
            /// Words that had negative sentiment
            /// </summary>
            public IList<string> Negative { get; set; }
            /// <summary>
            /// Words that had positive sentiment
            /// </summary>
            public IList<string> Positive { get; set; }

            public Score()
            {
                Words = new List<string>();
                Negative = new List<string>();
                Positive = new List<string>();
            }
        }
        #endregion
    }
}
