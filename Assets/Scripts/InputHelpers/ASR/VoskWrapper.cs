using UnityEngine;
using Vosk;

namespace InputHelpers
{
    public class VoskWrapper
    {
        readonly VoskRecognizer _recognizer;

        public VoskWrapper(string modelPath, int maxAlternatives = 1)
        {
            var model = new Model(modelPath);
            _recognizer = new VoskRecognizer(model, 16000.0f);

            _recognizer.SetWords(true);
            _recognizer.SetMaxAlternatives(maxAlternatives);
        }

        public string RecognizeAudio(AudioClip audioClip)
        {
            var data = new float[audioClip.samples];
            audioClip.GetData(data, 0);
            return _recognizer.AcceptWaveform(data, audioClip.samples)
                ? _recognizer.Result()
                : _recognizer.PartialResult();
        }
    }
}
