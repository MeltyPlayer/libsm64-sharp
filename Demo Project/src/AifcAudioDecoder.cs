using demo.common.audio;
using demo.common.audio.impl.al;

using libsm64sharp;


namespace demo {
  public static class AifcAudioDecoder {
    public static IReadOnlyList<short> itable = new short[] {
        0, 1, 2, 3, 4, 5, 6, 7,
        -8, -7, -6, -5, -4, -3, -2, -1,
    };


    public static void memset<T>(T[] dst,
                                 int dstIndex,
                                 T value,
                                 int n)
        where T : unmanaged {
      for (var i = 0; i < n; ++i) {
        dst[dstIndex + i] = value;
      }
    }

    public static void memcpy<T>(T[] dst,
                                 int dstIndex,
                                 T[] src,
                                 int srcIndex,
                                 int n)
        where T : unmanaged {
      for (var i = 0; i < n; ++i) {
        dst[dstIndex + i] = src[srcIndex + i];
      }
    }

    public static short SignExtend(
        int b, // number of bits representing the number in x
        int x // sign extend this b-bit number to r
    ) {
      int m = 1 << (int) (b - 1); // mask can be pre-computed if b is fixed

      x = x & ((1 << (int) b) -
               1); // (Skip this if bits in x above position b are already zero.)
      return (short) ((x ^ m) - m);
    }

    private static short[] lastsmp = new short[8];

    public static IAudioBuffer<short> Decode(
        IAudioManager<short> audioManager,
        ISm64AudioBankSound sound) {
      var sample = sound.Sample;
      var fullSize = sample.Loop.End;

      var shortSamples = new short[fullSize];
      var book = sample.Book;

      AifcAudioDecoder.Decode(
          sample.Samples,
          shortSamples,
          sample.Samples.Length,
          book,
          false);

      var audioBuffer = audioManager.CreateMutableBuffer();
      audioBuffer.SetMonoPcm(shortSamples);
      audioBuffer.Frequency = (int)(22050 * sound.Tuning);

      return audioBuffer;
    }

    public static void Decode(
        byte[] inData,
        short[] outData,
        long len,
        ISm64AdpcmBook book,
        bool decode8Only) {
      for (int x = 0; x < 8; x++)
        lastsmp[x] = 0;

      var inDataIndex = 0;
      var outDataIndex = 0;

      int index;
      int pred;

      int samples = 0;

      // flip the predictors
      var preds = new short[32 * book.NPredictors];
      for (int p = 0; p < (8 * book.Order * book.NPredictors); p++) {
        preds[p] = book.Predictors[p];
      }

      if (!decode8Only) {
        int _len =
            (int) ((len / 9) *
                   9); //make sure length was actually a multiple of 9

        while (_len > 0) {
          index = (inData[inDataIndex] >> 4) & 0xf;
          pred = (inData[inDataIndex] & 0xf);

          // to not make zelda crash but doesn't fix it
          pred %= (book.NPredictors);

          _len--;

          var pred1Index = pred * 16;
          Decode8(inData, ++inDataIndex, outData, outDataIndex, index, preds,
                  pred1Index, lastsmp);
          inDataIndex += 4;
          _len -= 4;
          outDataIndex += 8;

          Decode8(inData, inDataIndex, outData, outDataIndex, index, preds,
                  pred1Index,
                  lastsmp);
          inDataIndex += 4;
          _len -= 4;
          outDataIndex += 8;

          samples += 16;
        }
      } else {
        int _len =
            (int) ((len / 5) *
                   5); //make sure length was actually a multiple of 5

        while (_len > 0) {
          index = (inData[inDataIndex] >> 4) & 0xf;
          pred = (inData[inDataIndex] & 0xf);

          // to not make zelda crash but doesn't fix it
          pred = pred % (book.NPredictors);

          _len--;

          var pred1Index = pred * 16;
          Decode8Half(inData, ++inDataIndex, outData, outDataIndex, index,
                      preds, pred1Index, lastsmp);
          inDataIndex += 2;
          _len -= 2;
          outDataIndex += 8;

          Decode8Half(inData, inDataIndex, outData, outDataIndex, index,
                      preds, pred1Index, lastsmp);
          inDataIndex += 2;
          _len -= 2;
          outDataIndex += 8;
        }

        samples += 16;
      }
    }

    public static void Decode8(
        byte[] inData,
        int inDataIndex,
        short[] outData,
        int outDataIndex,
        int index,
        short[] predictors,
        int pred1Index,
        short[] lastsmp) {
      int i;
      var tmp = new short[8];
      long total = 0;
      short sample = 0;
      memset<short>(outData, outDataIndex, 0, 8);

      int pred2Index = pred1Index + 8;

      //printf("pred2[] = %x\n" , pred2[0]);
      for (i = 0; i < 8; i++) {
        var tmpIndex =
            (i & 1) != 0
                ? (inData[inDataIndex++] & 0xf)
                : ((inData[inDataIndex] >> 4) & 0xf);
        var shortVal = (short) (itable[tmpIndex] << index);

        tmp[i] = shortVal;
        tmp[i] = SignExtend(index + 4, tmp[i]);
      }

      for (i = 0; i < 8; i++) {
        var pred1Val = predictors[pred1Index + i];
        var pred2Val = predictors[pred2Index + i];

        total = (pred1Val * lastsmp[6]);
        total += (pred2Val * lastsmp[7]);

        if (i > 0) {
          for (int x = i - 1; x > -1; x--) {
            total += (tmp[((i - 1) - x)] * predictors[pred2Index + x]);
            //printf("sample: %x - pred: %x - _smp: %x\n" , ((i-1)-x) , pred2[x] , tmp[((i-1)-x)]);
          }
        }

//printf("pred = %x | total = %x\n" , pred2[0] , total);
        float result = ((tmp[i] << 0xb) + total) >> 0xb;
        if (result > 32767)
          sample = 32767;
        else if (result < -32768)
          sample = -32768;
        else
          sample = (short) result;
        outData[outDataIndex + i] = sample;
      }
      // update the last sample set for subsequent iterations
      memcpy(lastsmp, 0, outData, outDataIndex, 8);
    }

    public static void Decode8Half(
        byte[] inData,
        int inDataIndex,
        short[] outData,
        int outDataIndex,
        int index,
        short[] predictors,
        int pred1Index,
        short[] lastsmp
    ) {
      int i;
      var tmp = new short[8];
      long total = 0;
      short sample = 0;
      memset<short>(outData, outDataIndex, 0, 8);

      int pred2Index = (pred1Index + 8);

      //printf("pred2[] = %x\n" , pred2[0]);

      tmp[0] =
          (short) ((((((inData[inDataIndex]) & 0xC0) >> 6) & 0x3)) << (index));
      tmp[0] = SignExtend(index + 2, tmp[0]);
      tmp[1] =
          (short) ((((((inData[inDataIndex]) & 0x30) >> 4) & 0x3)) << (index));
      tmp[1] = SignExtend(index + 2, tmp[1]);
      tmp[2] =
          (short) ((((((inData[inDataIndex]) & 0x0C) >> 2) & 0x3)) << (index));
      tmp[2] = SignExtend(index + 2, tmp[2]);
      tmp[3] = (short) (((((inData[inDataIndex++]) & 0x03) & 0x3)) << (index));
      tmp[3] = SignExtend(index + 2, tmp[3]);
      tmp[4] =
          (short) ((((((inData[inDataIndex]) & 0xC0) >> 6) & 0x3)) << (index));
      tmp[4] = SignExtend(index + 2, tmp[4]);
      tmp[5] =
          (short) ((((((inData[inDataIndex]) & 0x30) >> 4) & 0x3)) << (index));
      tmp[5] = SignExtend(index + 2, tmp[5]);
      tmp[6] =
          (short) ((((((inData[inDataIndex]) & 0x0C) >> 2) & 0x3)) << (index));
      tmp[6] = SignExtend(index + 2, tmp[6]);
      tmp[7] = (short) (((((inData[inDataIndex++]) & 0x03) & 0x3)) << (index));
      tmp[7] = SignExtend(index + 2, tmp[7]);

      for (i = 0; i < 8; i++) {
        total = (predictors[pred1Index + i] * lastsmp[6]);
        total += (predictors[pred2Index + i] * lastsmp[7]);

        if (i > 0) {
          for (int x = i - 1; x > -1; x--) {
            total += (tmp[((i - 1) - x)] * predictors[pred2Index + x]);
            //printf("sample: %x - pred: %x - _smp: %x\n" , ((i-1)-x) , pred2[x] , tmp[((i-1)-x)]);
          }
        }

        //printf("pred = %x | total = %x\n" , pred2[0] , total);
        float result = ((tmp[i] << 0xb) + total) >> 0xb;
        if (result > 32767)
          sample = 32767;
        else if (result < -32768)
          sample = -32768;
        else
          sample = (short) result;
        outData[outDataIndex + i] = sample;
      }
// update the last sample set for subsequent iterations
      memcpy(lastsmp, 0, outData, outDataIndex, 8);
    }
  }
}