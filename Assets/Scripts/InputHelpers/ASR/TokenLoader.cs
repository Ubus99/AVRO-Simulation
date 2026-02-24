using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace InputHelpers.ASR
{
    public static class VocabLoader
    {
        /// <summary>
        ///     Load vocab.json (token -> id) from a TextAsset and return an id->token map.
        ///     vocab.json format: { " the": 503, "hello": 15496, ... }
        /// </summary>
        public static Dictionary<int, string> LoadIdToTokenMap(TextAsset vocabJson)
        {
            if (!vocabJson) throw new ArgumentNullException(nameof(vocabJson));

            // Deserialize token->id map (keys are token strings, values are ints)
            Dictionary<string, int> tokenToId;
            try
            {
                tokenToId = JsonConvert.DeserializeObject<Dictionary<string, int>>(vocabJson.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse vocab.json: {e.Message}");
                return new Dictionary<int, string>();
            }

            // Invert to id->token (int -> string)
            var idToToken = new Dictionary<int, string>(tokenToId.Count);
            foreach (var kv in tokenToId)
            {
                // token string -> id int
                // invert; if duplicate ids exist (should not), last one wins
                idToToken[kv.Value] = kv.Key;
            }

            return idToToken;
        }

        /// <summary>
        ///     Helper: convert a sequence of token ids into a plain string using the id->token map.
        ///     This does not apply byte-level BPE merging/postprocessing — you should apply any
        ///     Whisper-specific postprocessing (e.g., strip special tokens, merge bytes) afterwards.
        /// </summary>
        public static string TokensToString(IEnumerable<int> tokenIds, Dictionary<int, string> idToToken)
        {
            if (idToToken == null || idToToken.Count == 0 || tokenIds == null) return string.Empty;

            // naive concatenation of token strings in order
            // for Whisper you will typically need to post-process byte-level tokens into UTF-8 text.
            var parts = tokenIds.Select(id => idToToken.TryGetValue(id, out var tok)
                ? tok
                : $"[#{id}]").ToList();
            return string.Concat(parts);
        }
    }
}
