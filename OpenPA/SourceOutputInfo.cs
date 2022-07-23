using OpenPA.Native;
using System;
using System.Runtime.InteropServices;

namespace OpenPA
{
    public class SourceOutputInfo
    {
        /// <summary>
        /// Name of source output
        /// </summary>
        public String? Name { get; init; }
        /// <summary>
        /// Index of the source output
        /// </summary>
        public uint Index { get; init; }
        /// <summary>
        /// Sample spec of this source output
        /// </summary>
        public SampleSpec? SampleSpec { get; init; }
        /// <summary>
        /// Channel map of this source output
        /// </summary>
        public ChannelMap? ChannelMap { get; init; }
        /// <summary>
        /// Owning module index, or PA_INVALID_INDEX
        /// </summary>
        public uint OwnerModule { get; init; }
        /// <summary>
        /// Volume of the source output
        /// </summary>
        public Volume? Volume { get; init; }
        /// <summary>
        /// Mute switch of the source output
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
        /// Formats used by this source output
        /// </summary>
        public FormatInfo? Format { get; init; }
        /// <summary>
        /// Index of the client this source output belongs to, or PA_INVALID_INDEX when it does not belong to any client.
        /// </summary>
        public uint Client { get; init; }
        /// <summary>
        /// Index of the connected source.
        /// </summary>
        public uint Source { get; init; }
        /// <summary>
        /// Latency due to buffering in the source output.
        /// </summary>
        public ulong BufferUsec { get; init; }
        /// <summary>
        /// Latency of the source device.
        /// </summary>
        public ulong SourceUsec { get; init; }
        /// <summary>
        /// The resampling method used by this source output.
        /// </summary>
        public String? ResampleMethod { get; init; }
        /// <summary>
        /// Corked switch of the source output
        /// </summary>
        public bool Corked { get; init; }
        /// <summary>
        /// Indicates if the source output has volume.
        /// </summary>
        public bool HasVolume { get; init; }
        /// <summary>
        /// Indicates if the source output has writable volume.
        /// </summary>
        public bool VolumeWritable { get; init; }

        internal unsafe static SourceOutputInfo Convert(pa_source_output_info source_output_Info)
        {
            SourceOutputInfo sourceOutputInfo = new()
            {
                Name = source_output_Info.name != IntPtr.Zero ? Marshal.PtrToStringUTF8(source_output_Info.name) : String.Empty,
                Index = source_output_Info.index,
                SampleSpec = SampleSpec.Convert(source_output_Info.sample_spec),
                ChannelMap = ChannelMap.Convert(source_output_Info.channel_map),
                OwnerModule = source_output_Info.owner_module,
                Volume = Volume.Convert(source_output_Info.volume),
                Mute = source_output_Info.mute != 0,
                Driver = source_output_Info.driver != IntPtr.Zero ? Marshal.PtrToStringUTF8(source_output_Info.driver) : String.Empty,
                PropList = PropList.Convert(source_output_Info.proplist),
                Format = FormatInfo.Convert(*source_output_Info.format),
                Client = source_output_Info.client,
                Source = source_output_Info.source,
                BufferUsec = source_output_Info.buffer_usec,
                SourceUsec = source_output_Info.source_usec,
                ResampleMethod = source_output_Info.resample_method != IntPtr.Zero ? Marshal.PtrToStringUTF8(source_output_Info.resample_method) : String.Empty,
                Corked = source_output_Info.corked != 0,
                HasVolume = source_output_Info.has_volume != 0,
                VolumeWritable = source_output_Info.volume_writable != 0
            };

            return sourceOutputInfo;
        }

        public override string ToString()
        {
            return string.Format("SourceOutput@{0}:{1} {2}::{3} source {4}",
                OwnerModule,
                Index,
                Driver,
                String.IsNullOrEmpty(Name) ? "Untitled" : Name,
                Source
            );
        }
    }
}