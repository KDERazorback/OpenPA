using OpenPA.Native;
using System;
using System.Runtime.InteropServices;

namespace OpenPA
{
    public class SinkInputInfo
    {
        /// <summary>
        /// Name of sink input
        /// </summary>
        public String? Name { get; init; }
        /// <summary>
        /// Index of the sink input
        /// </summary>
        public uint Index { get; init; }
        /// <summary>
        /// Sample spec of this sink input
        /// </summary>
        public SampleSpec? SampleSpec { get; init; }
        /// <summary>
        /// Channel map of this sink input
        /// </summary>
        public ChannelMap? ChannelMap { get; init; }
        /// <summary>
        /// Owning module index, or PA_INVALID_INDEX
        /// </summary>
        public uint OwnerModule { get; init; }
        /// <summary>
        /// Volume of the sink input
        /// </summary>
        public Volume? Volume { get; init; }
        /// <summary>
        /// Mute switch of the sink input
        /// </summary>
        public bool Mute { get; init; }
        /// <summary>
        /// Driver name
        /// </summary>
        public String? Driver { get; init; }
        /// <summary>
        /// Property list
        /// </summary>
        public PropList? PropList { get; init; }
        /// <summary>
        /// Formats used by this sink input
        /// </summary>
        public FormatInfo? Format { get; init; }
        /// <summary>
        /// Index of the client this sink input belongs to, or PA_INVALID_INDEX when it does not belong to any client.
        /// </summary>
        public uint Client { get; init; }
        /// <summary>
        /// Index of the connected sink.
        /// </summary>
        public uint Sink { get; init; }
        /// <summary>
        /// Latency due to buffering in the sink input.
        /// </summary>
        public ulong BufferUsec { get; init; }
        /// <summary>
        /// Latency of the sink device.
        /// </summary>
        public ulong SinkUsec { get; init; }
        /// <summary>
        /// The resampling method used by this sink input.
        /// </summary>
        public String? ResampleMethod { get; init; }
        /// <summary>
        /// Corked switch of the sink input
        /// </summary>
        public bool Corked { get; init; }
        /// <summary>
        /// Indicates if the sink input has volume.
        /// </summary>
        public bool HasVolume { get; init; }
        /// <summary>
        /// Indicates if the sink input has writable volume.
        /// </summary>
        public bool VolumeWritable { get; init; }

        internal unsafe static SinkInputInfo Convert(pa_sink_input_info sink_input_Info)
        {
            SinkInputInfo sinkInputInfo = new()
            {
                Name = sink_input_Info.name != IntPtr.Zero ? Marshal.PtrToStringUTF8(sink_input_Info.name) : String.Empty,
                Index = sink_input_Info.index,
                SampleSpec = SampleSpec.Convert(sink_input_Info.sample_spec),
                ChannelMap = ChannelMap.Convert(sink_input_Info.channel_map),
                OwnerModule = sink_input_Info.owner_module,
                Volume = Volume.Convert(sink_input_Info.volume),
                Mute = sink_input_Info.mute != 0,
                Driver = sink_input_Info.driver != IntPtr.Zero ? Marshal.PtrToStringUTF8(sink_input_Info.driver) : String.Empty,
                PropList = PropList.Convert(sink_input_Info.proplist),
                Format = FormatInfo.Convert(*sink_input_Info.format),
                Client = sink_input_Info.client,
                Sink = sink_input_Info.sink,
                BufferUsec = sink_input_Info.buffer_usec,
                SinkUsec = sink_input_Info.sink_usec,
                ResampleMethod = sink_input_Info.resample_method != IntPtr.Zero ? Marshal.PtrToStringUTF8(sink_input_Info.resample_method) : String.Empty,
                Corked = sink_input_Info.corked != 0,
                HasVolume = sink_input_Info.has_volume != 0,
                VolumeWritable = sink_input_Info.volume_writable != 0
            };

            return sinkInputInfo;
        }

        public override string ToString()
        {
            return string.Format("SinkInput@{0}:{1} {2}::{3} sink {4}",
                OwnerModule,
                Index,
                Driver,
                String.IsNullOrEmpty(Name) ? "Untitled" : Name,
                Sink
            );
        }
    }
}