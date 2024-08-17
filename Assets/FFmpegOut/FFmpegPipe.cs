using System.Diagnostics;
using System.IO;
using System;

namespace FFmpegOut
{
    // A stream pipe class that invokes ffmpeg and connect to it.
    class FFmpegPipe
    {
        #region Public properties

        public enum Codec
        {
            ProRes, 
            H264, 
            VP8,
			H264_NVENC,
			HEVC_NVENC,
			H264_MAC_VIDEOTOOLBOX,
			HEVC_MAC_VIDEOTOOLBOX,				
            UDP_H264_NVENC, 
            UDP_HEVC_NVENC,
            UDP_H264_MAC_VIDEOTOOLBOX,
            UDP_HEVC_MAC_VIDEOTOOLBOX
        }

        public string Filename { get; private set; }
        public string Error { get; private set; }

        #endregion

        #region Public methods

        public FFmpegPipe(string name, int width, int height, int framerate, Codec codec)
        {
            if (IsUDPStreaming(codec))
            {
                Filename = "udp://127.0.0.1:5000" + GetSuffix(codec)";
            }
            else
            {
                name += DateTime.Now.ToString(" yyyy MMdd HHmmss");
                Filename = name.Replace(" ", "_") + GetSuffix(codec);   
            }

            var opt = "-y -f rawvideo -vcodec rawvideo -pixel_format rgb24";
            opt += " -video_size " + width + "x" + height;
            opt += " -framerate " + framerate;
            opt += " -loglevel warning -i - " + GetOptions(codec);
            opt += " " + Filename;

            var info = new ProcessStartInfo(FFmpegConfig.BinaryPath, opt);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            _subprocess = Process.Start(info);
            _stdin = new BinaryWriter(_subprocess.StandardInput.BaseStream);
        }

        public void Write(byte[] data)
        {
            if (_subprocess == null) return;

            _stdin.Write(data);
            _stdin.Flush();
        }

        public void Close()
        {
            if (_subprocess == null) return;

            _subprocess.StandardInput.Close();
            _subprocess.WaitForExit();

            var outputReader = _subprocess.StandardError;
            Error = outputReader.ReadToEnd();

            _subprocess.Close();
            _subprocess.Dispose();

            outputReader.Close();
            outputReader.Dispose();

            _subprocess = null;
            _stdin = null;
        }

        #endregion

        #region Private members

        Process _subprocess;
        BinaryWriter _stdin;

        static string [] _suffixes = {
            ".mov",
            ".mp4",
            ".webm",
            ".mp4",
            ".mp4",
            ".mp4",
            ".mp4",
            "?pkt_size=1316",
            "?pkt_size=1316",
            "?pkt_size=1316",
            "?pkt_size=1316"
        };

        static string [] _options = {
            "-c:v prores_ks -pix_fmt yuv422p10le",
            "-pix_fmt yuv420p",
            "-c:v libvpx",
			"-c:v h264_nvenc",
			"-c:v hevc_nvenc",
			"-c:v hevc_videotoolbox",
			"-c:v hevc_videotoolbox",
            "-c:v h264_nvenc -preset veryfast -tune zerolatency -f mpegts -b 500000k",
            "-c:v hevc_nvenc -preset veryfast -tune zerolatency -f mpegts -b 500000k",
            "-c:v h264_videotoolbox -preset veryfast -tune zerolatency -f mpegts -b 500000k",
            "-c:v hevc_videotoolbox -preset veryfast -tune zerolatency -f mpegts -b 500000k"
        };

        static string GetSuffix(Codec codec)
        {
            return _suffixes[(int)codec];
        }

        static string GetOptions(Codec codec)
        {
            return _options[(int)codec];
        }

        static bool IsUDPStreaming(Codec codec)
        {
            if (Codec.UDP_H264_NVENC == codec)
            {
                return true;
            }
            
            if (Codec.UDP_H264_MAC_VIDEOTOOLBOX == codec)
            {
                return true;
            }
            
            if (Codec.UDP_HEVC_NVENC == codec)
            {
                return true;
            }
            
            if (Codec.UDP_H264_HEVC_VIDEOTOOLBOX == codec)
            {
                return true;
            }
            

            return false;
        }
        #endregion
    }
}
