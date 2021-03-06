using System;
using System.IO;
using System.Linq;
using SharpAvi;
using SharpAvi.Output;

namespace Aaru.VideoNow
{
    public static class Color
    {
        /// <summary>
        ///     This is some kind of header. Every 10 bytes there's an audio byte. Here it is without reordering from little
        ///     endian, so the first appearence is at 9th byte.
        /// </summary>
        public static readonly byte[] FrameMarker =
        {
            0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81,
            0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7,
            0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3,
            0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81,
            0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7,
            0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81,
            0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7,
            0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3,
            0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81,
            0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7,
            0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81,
            0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7,
            0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0xE3, 0x81, 0xC7, 0xE3,
            0x81, 0xC7, 0xE3, 0x81, 0x00, 0xC7, 0x00, 0x00, 0x01, 0x02, 0x02, 0x04, 0x03, 0x06, 0x00, 0xFF, 0x04, 0x08,
            0x05, 0x0A, 0x06, 0x0C, 0x07, 0x0E, 0x00, 0xFF, 0x08, 0x11, 0x09, 0x13, 0x0A, 0x15, 0x0B, 0x17, 0x00, 0xFF,
            0x0C, 0x19, 0x0D, 0x1B, 0x0E, 0x1D, 0x0F, 0x1F, 0x00, 0xFF, 0x28, 0x00, 0x29, 0x02, 0x2A, 0x04, 0x2B, 0x06,
            0x00, 0xFF, 0x2C, 0x08, 0x2D, 0x0A, 0x2E, 0x0C, 0x2F, 0x0E, 0x00, 0xFF, 0x30, 0x11, 0x31, 0x13, 0x32, 0x15,
            0x33, 0x17, 0x00, 0xFF, 0x34, 0x19, 0x35, 0x1B, 0x36, 0x1D, 0x37, 0x1F, 0x00, 0xFF, 0x38, 0x00, 0x39, 0x02,
            0x3A, 0x04, 0x3B, 0x06, 0x00, 0xFF, 0x3C, 0x08, 0x3D, 0x0A, 0x3E, 0x0C, 0x3F, 0x0E, 0x00, 0xFF, 0x40, 0x11,
            0x41, 0x13, 0x42, 0x15, 0x43, 0x17, 0x00, 0xFF, 0x44, 0x19, 0x45, 0x1B, 0x46, 0x1D, 0x47, 0x1F, 0x00, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0x00, 0xFF
        };

        public static readonly byte[] SwappedFrameMarker =
        {
            0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3,
            0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81,
            0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7,
            0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3,
            0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00,
            0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3,
            0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81,
            0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7,
            0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3,
            0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00,
            0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3,
            0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81,
            0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7, 0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x81, 0xE3, 0xE3, 0xC7,
            0xC7, 0x81, 0x81, 0xE3, 0xC7, 0x00, 0x00, 0x00, 0x02, 0x01, 0x04, 0x02, 0x06, 0x03, 0xFF, 0x00, 0x08, 0x04,
            0x0A, 0x05, 0x0C, 0x06, 0x0E, 0x07, 0xFF, 0x00, 0x11, 0x08, 0x13, 0x09, 0x15, 0x0A, 0x17, 0x0B, 0xFF, 0x00,
            0x19, 0x0C, 0x1B, 0x0D, 0x1D, 0x0E, 0x1F, 0x0F, 0xFF, 0x00, 0x00, 0x28, 0x02, 0x29, 0x04, 0x2A, 0x06, 0x2B,
            0xFF, 0x00, 0x08, 0x2C, 0x0A, 0x2D, 0x0C, 0x2E, 0x0E, 0x2F, 0xFF, 0x00, 0x11, 0x30, 0x13, 0x31, 0x15, 0x32,
            0x17, 0x33, 0xFF, 0x00, 0x19, 0x34, 0x1B, 0x35, 0x1D, 0x36, 0x1F, 0x37, 0xFF, 0x00, 0x00, 0x38, 0x02, 0x39,
            0x04, 0x3A, 0x06, 0x3B, 0xFF, 0x00, 0x08, 0x3C, 0x0A, 0x3D, 0x0C, 0x3E, 0x0E, 0x3F, 0xFF, 0x00, 0x11, 0x40,
            0x13, 0x41, 0x15, 0x42, 0x17, 0x43, 0xFF, 0x00, 0x19, 0x44, 0x1B, 0x45, 0x1D, 0x46, 0x1F, 0x47, 0xFF, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0x00
        };

        public static void Decode(string filename, Stream fs, bool swapped, long framePosition)
        {
            char progress = ' ';

            var aviWriter = new AviWriter(filename + ".avi")
            {
                EmitIndex1 = true, FramesPerSecond = 18
            };

            IAviVideoStream videoStream = aviWriter.AddVideoStream(144, 80, BitsPerPixel.Bpp24);
            videoStream.Codec = KnownFourCCs.Codecs.Uncompressed;
            IAviAudioStream audioStream = aviWriter.AddAudioStream(2, 17640, 8);

            fs.Position = framePosition;
            byte[] frameBuffer = new byte[19600];
            fs.Read(frameBuffer, 0, frameBuffer.Length);

            int    audioStart       = swapped ? 9 : 8;
            byte[] frameMarkerToUse = swapped ? SwappedFrameMarker : FrameMarker;

            if(!swapped)
                frameBuffer = Swapping.SwapBuffer(frameBuffer);

            var outFs = new MemoryStream();

            for(int i = 9; i <= frameBuffer.Length; i += 10)
            {
                switch((i / 10) % 4)
                {
                    case 0:
                        progress = '-';

                        break;
                    case 1:
                        progress = '\\';

                        break;
                    case 2:
                        progress = '|';

                        break;
                    case 3:
                        progress = '/';

                        break;
                }

                Console.Write($"\r{Localization.ExtractingAudio}", progress);
                outFs.WriteByte(frameBuffer[i]);
            }

            byte[] videoFrame = DecodeFrame(frameBuffer);
            videoStream.WriteFrame(true, videoFrame, 0, videoFrame.Length);
            audioStream.WriteBlock(outFs.ToArray(), 0, (int)outFs.Length);

            int totalFrames = 1;
            framePosition += 19600;
            byte[] buffer = new byte[frameMarkerToUse.Length];

            while(framePosition + 19600 < fs.Length)
            {
                switch(totalFrames % 4)
                {
                    case 0:
                        progress = '-';

                        break;
                    case 1:
                        progress = '\\';

                        break;
                    case 2:
                        progress = '|';

                        break;
                    case 3:
                        progress = '/';

                        break;
                }

                Console.Write($"\r{Localization.LookingForMoreFrames}", progress);

                for(int ab = audioStart; ab < buffer.Length; ab += 10)
                    buffer[ab] = 0;

                if(!buffer.SequenceEqual(frameMarkerToUse))
                {
                    Console.Write("\r                                      \r");
                    Console.WriteLine(Localization.FrameAndNextAreNotAligned, totalFrames);
                    long expectedFramePosition = framePosition;

                    while(framePosition < fs.Length)
                    {
                        fs.Position = framePosition;
                        fs.Read(buffer, 0, buffer.Length);

                        for(int ab = audioStart; ab < buffer.Length; ab += 10)
                            buffer[ab] = 0;

                        if(buffer.SequenceEqual(frameMarkerToUse))
                        {
                            Console.Write("\r                                      \r");

                            fs.Position = framePosition;
                            frameBuffer = new byte[19600];
                            fs.Read(frameBuffer, 0, frameBuffer.Length);

                            if(!swapped)
                                frameBuffer = Swapping.SwapBuffer(frameBuffer);

                            outFs = new MemoryStream();

                            for(int i = 9; i <= frameBuffer.Length; i += 10)
                            {
                                switch((i / 10) % 4)
                                {
                                    case 0:
                                        progress = '-';

                                        break;
                                    case 1:
                                        progress = '\\';

                                        break;
                                    case 2:
                                        progress = '|';

                                        break;
                                    case 3:
                                        progress = '/';

                                        break;
                                }

                                Console.Write($"\r{Localization.ExtractingAudio}", progress);
                                outFs.WriteByte(frameBuffer[i]);
                            }

                            videoFrame = DecodeFrame(frameBuffer);
                            videoStream.WriteFrame(true, videoFrame, 0, videoFrame.Length);
                            audioStream.WriteBlock(outFs.ToArray(), 0, (int)outFs.Length);

                            totalFrames++;
                            Console.Write("\r                                      \r");

                            Console.WriteLine(Localization.FrameFoundAtPosition, framePosition, totalFrames,
                                              framePosition - expectedFramePosition);

                            Console.
                                WriteLine(framePosition % 2352 == 0 ? Localization.FrameIsAtSectorBoundary : Localization.FrameIsNotAtSectorBoundary,
                                          totalFrames);

                            framePosition += 19600;

                            break;
                        }

                        framePosition++;
                    }

                    continue;
                }

                if(framePosition % 2352 == 0)
                {
                    Console.Write("\r                                      \r");
                    Console.WriteLine(Localization.FrameIsAtSectorBoundary, totalFrames);
                }

                Console.Write("\r                                      \r");
                fs.Position = framePosition;
                frameBuffer = new byte[19600];
                fs.Read(frameBuffer, 0, frameBuffer.Length);

                if(!swapped)
                    frameBuffer = Swapping.SwapBuffer(frameBuffer);

                outFs = new MemoryStream();

                for(int i = 9; i <= frameBuffer.Length; i += 10)
                {
                    switch((i / 10) % 4)
                    {
                        case 0:
                            progress = '-';

                            break;
                        case 1:
                            progress = '\\';

                            break;
                        case 2:
                            progress = '|';

                            break;
                        case 3:
                            progress = '/';

                            break;
                    }

                    Console.Write($"\r{Localization.ExtractingAudio}", progress);
                    outFs.WriteByte(frameBuffer[i]);
                }

                videoFrame = DecodeFrame(frameBuffer);
                videoStream.WriteFrame(true, videoFrame, 0, videoFrame.Length);
                audioStream.WriteBlock(outFs.ToArray(), 0, (int)outFs.Length);

                totalFrames++;
                fs.Position = framePosition;
                fs.Read(buffer, 0, buffer.Length);
                framePosition += 19600;
            }

            Console.Write("\r                                      \r");
            Console.WriteLine(Localization.FramesFound, totalFrames);

            outFs.Close();
            aviWriter.Close();
        }

        public static byte[] DecodeFrame(byte[] frameBuffer)
        {
            char progress = ' ';
            var  videoFs  = new MemoryStream();
            Array.Reverse(frameBuffer);
            byte r, g, b;

            int index = 1;
            int indexBlock2;

            for(int i = 0; i < 19200; i += 240)
            {
                for(int k = 0; k < 120; k += 10)
                {
                    for(int j = 1; j < 9; j += 3)
                    {
                        indexBlock2 = index + 120;

                        switch((index / 10) % 4)
                        {
                            case 0:
                                progress = '-';

                                break;
                            case 1:
                                progress = '\\';

                                break;
                            case 2:
                                progress = '|';

                                break;
                            case 3:
                                progress = '/';

                                break;
                        }

                        Console.Write($"\r{Localization.ExtractingVideo}", progress);
                        r = (byte)((frameBuffer[index]       & 0xF0) + ((frameBuffer[index]       & 0xF0) >> 4));
                        b = (byte)((frameBuffer[indexBlock2] & 0xF0) + ((frameBuffer[indexBlock2] & 0xF0) >> 4));
                        g = (byte)((frameBuffer[indexBlock2] & 0x0F) + ((frameBuffer[indexBlock2] & 0x0F) << 4));
                        videoFs.WriteByte(r);
                        videoFs.WriteByte(g);
                        videoFs.WriteByte(b);

                        r = (byte)((frameBuffer[indexBlock2 + 1] & 0xF0) +
                                   ((frameBuffer[indexBlock2 + 1] & 0xF0) >> 4));

                        b = (byte)((frameBuffer[index]     & 0x0F) + ((frameBuffer[index]     & 0x0F) << 4));
                        g = (byte)((frameBuffer[index + 1] & 0xF0) + ((frameBuffer[index + 1] & 0xF0) >> 4));
                        videoFs.WriteByte(r);
                        videoFs.WriteByte(g);
                        videoFs.WriteByte(b);

                        r = (byte)((frameBuffer[index + 1] & 0x0F) + ((frameBuffer[index + 1] & 0x0F) << 4));

                        b = (byte)((frameBuffer[indexBlock2 + 1] & 0x0F) +
                                   ((frameBuffer[indexBlock2 + 1] & 0x0F) << 4));

                        g = (byte)((frameBuffer[indexBlock2 + 2] & 0xF0) +
                                   ((frameBuffer[indexBlock2 + 2] & 0xF0) >> 4));

                        videoFs.WriteByte(r);
                        videoFs.WriteByte(g);
                        videoFs.WriteByte(b);

                        r = (byte)((frameBuffer[index + 120 + 2] & 0x0F) +
                                   ((frameBuffer[index + 120 + 2] & 0x0F) << 4));

                        b = (byte)((frameBuffer[index + 2] & 0xF0) + ((frameBuffer[index + 2] & 0xF0) >> 4));
                        g = (byte)((frameBuffer[index + 2] & 0x0F) + ((frameBuffer[index + 2] & 0x0F) << 4));
                        videoFs.WriteByte(r);
                        videoFs.WriteByte(g);
                        videoFs.WriteByte(b);

                        index += 3;
                    }

                    index += 1;
                }

                index += 120;
            }

            frameBuffer = new byte[videoFs.Length];
            byte[] frameBuffer2 = videoFs.ToArray();

            for(int row = 0; row < 80; row++)
            {
                for(int p = 0; p < 432; p++)
                    frameBuffer[(row * 432) + p] = frameBuffer2[(row * 432) + (431 - p)];
            }

            return frameBuffer;
        }
    }
}