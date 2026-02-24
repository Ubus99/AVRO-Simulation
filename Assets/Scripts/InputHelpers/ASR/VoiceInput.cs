using System;
using System.Collections.Generic;
using System.Linq;
using Unity.InferenceEngine;
using UnityEngine;

namespace InputHelpers.ASR
{
    /// <summary>
    ///     Minimal Whisper-Tiny pipeline reference for local inference using the Unity Inference Engine.
    ///     - Assign the four ONNX->ModelAsset files from the HuggingFace package:
    ///     LogMel (logmel_spectrogram), Encoder (encoder_model), Decoder (decoder_model),
    ///     DecoderWithPast (decoder_with_past_model) and the vocab.json as a TextAsset.
    ///     - This example shows the call sequence only; replace DecodeOutput(...) with your tokenizer/CTC decoder.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class VoiceInput : MonoBehaviour
    {
        [Header("Model assets (from huggingface: unity/inference-engine-whisper-tiny)")]
        public ModelAsset logMelModel;

        public ModelAsset encoderModel;
        public ModelAsset decoderModel;
        public ModelAsset decoderWithPastModel;
        public TextAsset vocabJson; // drag vocab.json here (used by your decoder)

        [Header("Backend / Mic")]
        public BackendType backend = BackendType.CPU;

        public string micDevice = "";
        public int sampleRate = 16000;
        public int micBufferSeconds = 2;

        [Header("VAD")]
        [Range(0f, 1f)]
        public float triggerRms = 0.02f;

        public float minSecondsBetweenTriggers = 0.8f;
        public float captureSeconds = 1.0f;
        Worker decoderWithPastWorker;
        Worker decoderWorker;
        Worker encoderWorker;
        int lastReadPos;
        float lastTriggerTime = -10f;

        // workers
        Worker logMelWorker;

        // mic runtime
        AudioClip micClip;
        int micClipSamples;
        Dictionary<int, string> tokenMap;

        void Start()
        {
            tokenMap = VocabLoader.LoadIdToTokenMap(vocabJson);

            if (!InitWorkers())
            {
                enabled = false;
                return;
            }
            if (!StartMicrophone())
            {
                enabled = false;
            }
        }

        void Update()
        {
            var block = ReadLatestBlock(Math.Min(sampleRate, micClipSamples));
            if (block == null || block.Length == 0) return;

            var rms = ComputeRMS(block);
            if (!ShouldTrigger(rms)) return;

            lastTriggerTime = Time.time;
            var capture = CaptureRecentSamples(captureSeconds);
            if (capture is not { Length: > 0 }) return;

            // Run full pipeline on captured audio (synchronously for simplicity)
            var transcript = RunWhisperPipeline(capture);
            OnTranscription(transcript);
        }

        void OnDisable()
        {
            if (Microphone.IsRecording(micDevice)) Microphone.End(micDevice);
            logMelWorker?.Dispose();
            encoderWorker?.Dispose();
            decoderWorker?.Dispose();
            decoderWithPastWorker?.Dispose();
        }

        bool InitWorkers()
        {
            try
            {
                if (!logMelModel || !encoderModel || !decoderModel || !decoderWithPastModel)
                {
                    Debug.LogError("Assign all Whisper model ModelAssets in inspector.");
                    return false;
                }

                // Load Model and create Worker for each stage. Keep as simple blocking initialization.
                var mLogMel = ModelLoader.Load(logMelModel);
                var mEnc = ModelLoader.Load(encoderModel);
                var mDec = ModelLoader.Load(decoderModel);
                var mDecPast = ModelLoader.Load(decoderWithPastModel);

                logMelWorker = new Worker(mLogMel, backend);
                encoderWorker = new Worker(mEnc, backend);
                decoderWorker = new Worker(mDec, backend);
                decoderWithPastWorker = new Worker(mDecPast, backend);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize inference workers: {e.Message}");
                return false;
            }
        }

        /// <summary>
        ///     Starts the microphone and returns true on success.
        /// </summary>
        /// <returns></returns>
        bool StartMicrophone()
        {
            micClip = Microphone.Start(micDevice, true, micBufferSeconds, sampleRate);
            if (micClip == null)
            {
                Debug.LogError("Microphone start failed.");
                return false;
            }
            while (Microphone.GetPosition(micDevice) <= 0)
            {
            } // wait until mic warms
            micClipSamples = micClip.samples;
            lastReadPos = Microphone.GetPosition(micDevice);
            return true;
        }

        float[] ReadLatestBlock(int maxSamples)
        {
            var pos = Microphone.GetPosition(micDevice);
            if (pos < 0 || micClipSamples == 0) return Array.Empty<float>();

            var newSamples = pos - lastReadPos;
            if (newSamples < 0) newSamples += micClipSamples;
            if (newSamples == 0) return Array.Empty<float>();

            var toRead = Math.Min(newSamples, maxSamples);
            var buffer = new float[toRead];
            var start = lastReadPos;

            if (start + toRead <= micClipSamples)
            {
                micClip.GetData(buffer, start);
            }
            else
            {
                var first = micClipSamples - start;
                var tmp = new float[first];
                micClip.GetData(tmp, start);
                Array.Copy(tmp, 0, buffer, 0, first);
                var rem = toRead - first;
                var tmp2 = new float[rem];
                micClip.GetData(tmp2, 0);
                Array.Copy(tmp2, 0, buffer, first, rem);
            }

            lastReadPos = (lastReadPos + toRead) % micClipSamples;
            return buffer;
        }

        static float ComputeRMS(float[] samples)
        {
            var s = samples.Aggregate<float, double>(0, (current, t) => current + t * t);
            return samples.Length > 0 ? Mathf.Sqrt((float)(s / samples.Length)) : 0f;
        }

        bool ShouldTrigger(float rms)
        {
            return rms >= triggerRms && Time.time - lastTriggerTime >= minSecondsBetweenTriggers;
        }

        float[] CaptureRecentSamples(float seconds)
        {
            var captureSamples = Mathf.Min(Mathf.CeilToInt(seconds * sampleRate), micClipSamples);
            var capture = new float[captureSamples];

            var pos = Microphone.GetPosition(micDevice);
            var start = pos - captureSamples;
            if (start < 0) start += micClipSamples;

            if (start + captureSamples <= micClipSamples)
            {
                micClip.GetData(capture, start);
            }
            else
            {
                var first = micClipSamples - start;
                var tmp = new float[first];
                micClip.GetData(tmp, start);
                Array.Copy(tmp, 0, capture, 0, first);
                var rem = captureSamples - first;
                var tmp2 = new float[rem];
                micClip.GetData(tmp2, 0);
                Array.Copy(tmp2, 0, capture, first, rem);
            }

            return capture;
        }

        /// <summary>
        ///     Runs the core Whisper-Tiny pipeline:
        ///     1) Run log-mel spectrogram model to produce features.
        ///     2) Run encoder on features -> encoded features.
        ///     3) Run decoder (and decoder-with-past) to generate logits/tokens.
        ///     NOTE: The exact input/output tensor names & shapes depend on the ONNX models exported for Unity;
        ///     inspect your Model assets (Model.inputs / Model.outputs) and adapt tensor shapes accordingly.
        /// </summary>
        string RunWhisperPipeline(float[] audioSamples)
        {
            // 1) Build input tensor for log-mel model.
            //    Many Whisper repos precompute log-mel on Python side; here we assume the provided logMelModel
            //    accepts raw PCM float input shaped like [1, N] or a specific shape — adjust as needed.
            using var inputLogMel = new Tensor<float>(new TensorShape(1, audioSamples.Length), audioSamples);
            logMelWorker.Schedule(inputLogMel);
            using var melOut = logMelWorker.PeekOutput() as Tensor<float>;
            if (melOut == null) return "[error: logmel produced no output]";

            // 2) Encoder: take melOut array as encoder input (shape mapping depends on model)
            var melArr = melOut.DownloadToArray();
            using var inputEnc = new Tensor<float>(new TensorShape(1, melArr.Length), melArr);
            encoderWorker.Schedule(inputEnc);
            using var encOut = encoderWorker.PeekOutput() as Tensor<float>;
            if (encOut == null) return "[error: encoder produced no output]";

            // 3) Decoder: run a single-step decode to get logits (this is illustrative).
            var encArr = encOut.DownloadToArray();
            using var inputDec = new Tensor<float>(new TensorShape(1, encArr.Length), encArr);
            decoderWorker.Schedule(inputDec);
            using var decOut = decoderWorker.PeekOutput() as Tensor<float>;
            if (decOut == null) return "[error: decoder produced no output]";

            var logits = decOut.DownloadToArray();

            // Placeholder decoding: map logits -> a single token id. Replace with full autoregressive decoding,
            // use decoderWithPastWorker for multi-step decoding, and use vocabJson to map token ids -> strings.
            return DecodeOutput(logits);
        }

        // Simple placeholder decoder. Replace with tokenizer + CTC/beam decode using vocabJson.
        static string DecodeOutput(float[] logits)
        {
            if (logits == null || logits.Length == 0) return string.Empty;
            var best = 0;
            var max = logits[0];
            for (var i = 1; i < logits.Length; i++)
                if (logits[i] > max)
                {
                    max = logits[i];
                    best = i;
                }
            return $"[token_{best}]";
        }

        static void OnTranscription(string text)
        {
            Debug.Log($"WhisperTiny -> {text}");
        }
    }
}
