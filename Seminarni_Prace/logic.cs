using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Appka
{
    internal class Logic
    {
        public static FrequencyData[] DoTransform(string filename)
        {   
            using (FileStream stream = File.OpenRead(filename))
            {
                BinaryReader reader = new BinaryReader(stream);
                SampleArray sampleArray = InspectWav(reader);
                FrequencyData[] fourie = DFT(sampleArray.samples, sampleArray.sampleRate);
                return fourie;
            }
        }
        public static FrequencyData[] DFT(float[] samples, int sampleRate)
        {
            int numFrequencies = samples.Length / 2 + 1;
            FrequencyData[] spectrum = new FrequencyData[numFrequencies];
            float frequencyStep = sampleRate / (float)samples.Length; 
            Parallel.For(0, spectrum.Length, freqIndex =>
            {
                Vector2 sampleSum = Vector2.Zero;
                for (int i = 0; i < samples.Length; i++)
                {
                    float angle = i / (float)(samples.Length) * (float)(2 * Math.PI) * freqIndex;
                    Vector2 testPoint = new((float)Math.Cos(angle), (float)Math.Sin(angle));
                    sampleSum += testPoint * samples[i];
                }

                Vector2 sampleCentre = sampleSum / samples.Length;

                bool is0Hz = freqIndex == 0;
                bool isNyquistFreq = freqIndex == spectrum.Length - 1 && samples.Length % 2 == 0;
                float amplitudeScale = is0Hz || isNyquistFreq ? 1 : 2;
                float amplitude = (float)(sampleCentre.Length() * amplitudeScale);
                float frequency = freqIndex * frequencyStep;
                float phase = -(float)Math.Atan2(sampleCentre.Y, sampleCentre.X);
                spectrum[freqIndex] = new FrequencyData(frequency, amplitude, phase);
            });

            return spectrum;
        }

        public struct FrequencyData
        {
            public float Frequency;
            public float Amplitude;
            public float Phase;

            public FrequencyData(float frequency, float amplitude, float offset)
            {
                Frequency = frequency;
                Amplitude = amplitude;
                Phase = offset;
            }
        }



        //FFT 

        public static Vector<Complex> FFT()
        {

        }

        public static (float[], float[]) EvenOddArray(float[] fullArray, int len)
        {
            float[] Aodd = new float[len / 2];
            float[] Aeven = new float[len / 2]; ;
            for (int i = 0; i < len; i++)
            {
                if (i % 2 == 1)
                {
                    Aodd[(i - 1) / 2] = fullArray[i];
                }
                else
                {
                    Aeven[i / 2] = fullArray[i]; 
                }
            }
            return (Aodd, Aeven);
        }

        static Dictionary<string, long> WavChunkLookUp(BinaryReader reader) //Metoda vracející dictionary<string, long> 
        {
        string RIFFID = string.Join("", reader.ReadChars(4)); //přečte první 4 chary souboru a uloží je jako RIFFId, potom posune polohu readeru o 4 dál
        reader.ReadInt32(); // velikost souboru, opět nás posune dopředu
        string WaveId = string.Join("", reader.ReadChars(4));
        if (RIFFID != "RIFF" || WaveId != "WAVE") { Console.WriteLine("Invalid file"); }
        //projdeme soubor a zjistíme, jestli je.wav
        Dictionary<string, long> keyValues = new Dictionary<string, long>();
        while (reader.BaseStream.Position < reader.BaseStream.Length) //projde celou soubor a zjistí důležité informace: fmt, LIST, data
        {
            long ChunkPosition = reader.BaseStream.Position; //pozice ve složce
            string chunkID = string.Join("", reader.ReadChars(4));
            keyValues.Add(chunkID, ChunkPosition);
            int chunkSize = reader.ReadInt32();
            reader.BaseStream.Position += chunkSize;
        }
        return keyValues;
        }
        public static SampleArray InspectWav(BinaryReader reader)
        {
            Dictionary<string, long> keyValues = WavChunkLookUp(reader);
            reader.BaseStream.Position = keyValues["fmt "] + 8;
            int format = reader.ReadUInt16(); //pcm (Pulse code modulation), IEEE_FLOAT, ALAW, MULAW, EXTENSIBLE
            int nChannels = reader.ReadInt16(); //mono nebo stereo
            int sampleRate = reader.ReadInt32(); //samples za sekundu
            reader.ReadChars(6); //přeskočí 6 nepotřebných (pro nás) bytů. Mohli bychom stejného docílit i za pomocí: reader.ReadInt32();reader.ReadInt16();
            int bitsPerSample = reader.ReadInt16(); //kolik bitů máme za jeden sample. Je to informace, která je potřeba k budoucím výpočtům
            int bytesPerSample = bitsPerSample / 8; // byte = 8 * bit => num of bits / 8 is num of bytes
            if (format == 0xFFFE)
            {
                reader.ReadChars(8);
                format = reader.ReadUInt16();
            }
            reader.BaseStream.Position = keyValues["data"] + 4;
            int ckSize = reader.ReadInt32(); //výsledkem bude, kolik bytů je v jednom chunku
            byte[] data = reader.ReadBytes(ckSize);
            float[] samples = new float[ckSize / (nChannels * bytesPerSample)]; //array floatů s velikostí n samples
            float normFactor = 1f / ((2 ^ (bitsPerSample - 1)) - 1); //1f je jedna, ale je to float number -- float/integer = float a my chceme float. každý formát má jiný počet bitů na sample, a proto by bylo dobré mít nějaký normovací faktor, kterým můžeme potom normovat array floatů na místě „i“ samples[i]
            if (format == 0xFFFE) { normFactor = 1; }
            for (int i = 0; i < samples.Length; i++)
            {
                int offset = i * bytesPerSample * nChannels;
                ReadOnlySpan<byte> sampleBytes = data.AsSpan(offset, bytesPerSample);
                samples[i] = BytesToFloat(sampleBytes, format) * normFactor;
            }
            return new SampleArray(samples, sampleRate);
        }
        static float BytesToFloat(ReadOnlySpan<byte> sampleBytes, int wavFormat) // metoda konvertující byty na floaty, které potom využijeme jako vektory k fourierově transformac
        {
            if (wavFormat == 0x0001) //jestli je format souboru PCM 
            {
                return sampleBytes.Length switch
                {
                    1 => sampleBytes[0] - 128,
                    2 => BitConverter.ToInt16(sampleBytes),
                    3 => (sampleBytes[2] >> 7) * (0xFF << 24) | sampleBytes[2] << 16 | sampleBytes[1] << 8 | sampleBytes[0], // 24 bit
                    4 => BitConverter.ToInt32(sampleBytes),
                    _ => throw new Exception($"Unsupported byte count: {sampleBytes.Length}")
                };
            }
            else if (wavFormat == 0x0003) // IEEE_FLOAT
            {
                return BitConverter.ToSingle(sampleBytes);
            }
            throw new Exception("You are using a format I don't want you to use");
            //neřešíme tu vůbec alaw a mulaw lol
            // TODO : alaw, mulaw a ty se mi dělat nechtějí
        }
    }
    public struct SampleArray
    {
        public float[] samples;
        public int sampleRate;
        public SampleArray(float[] samples, int sampleRate)
        {
            this.samples = samples;
            this.sampleRate = sampleRate;
        }
    }
}