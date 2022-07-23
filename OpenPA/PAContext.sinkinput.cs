using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OpenPA.Native.pa_operation;
using static OpenPA.Native.pa_context;

namespace OpenPA
{
    public unsafe partial class PAContext
    {

        #region SinkInput

        #region Callbacks
        /// <summary>
        /// Callback for the get_sink_input_info family of calls
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="info">sink_input_info data</param>
        /// <param name="eol">End of list marker</param>
        /// <param name="userdata">Mainloop pointer should be passed in here</param>
        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
        static unsafe void SinkInputCallback(pa_context* ctx, pa_sink_input_info* info, int eol, void* userdata)
        {
            // Test for the end of list
            if (eol == 0)
            {
                // Not the end of the list

                // Copy the pa_sink_input_info address
                *((pa_sink_input_info**)userdata) = info;

                // Signal the mainloop to continue
                MainLoop.Instance.Signal(1);
            }
            else
            {
                // Return null
                *((pa_sink_input_info**)userdata) = null;

                // Signal the mainloop
                MainLoop.Instance.Signal(0);
            }

        }
        
        #endregion

        /// <summary>
        /// Gets a SinkInputInfo object describing a sink input
        /// </summary>
        /// <param name="index">Idx of the sink input</param>
        /// <returns>SinkInputInfo object</returns>
        public Task<SinkInputInfo?> GetSinkInputInfoAsync(uint index) => Task.Run(() => GetSinkInputInfo(index));

        /// <summary>
        /// Gets a SinkInputInfo object describing a sink input
        /// </summary>
        /// <param name="index">Idx of the sink input</param>
        /// <returns>SinkInputInfo object</returns>
        public SinkInputInfo? GetSinkInputInfo(uint index)
        {
            // Returned object
            SinkInputInfo? info = default;

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Pointer to returned structure
            pa_sink_input_info* sink_input_info = null;

            // Call the native get_sink_input_info native function passing in the idx and callback
            pa_operation* op = pa_context.pa_context_get_sink_input_info(pa_Context, index, &SinkInputCallback, &sink_input_info);

            // Wait for the callback to signal
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && sink_input_info == null)
                MainLoop.Instance.Wait();


            // If the callback returned data
            if (sink_input_info != null)
            {
                // Copy the unmanaged sink_input_info structure into a SinkInputInfo object
                info = SinkInputInfo.Convert(*sink_input_info);

                // Allow PulseAudio to free the pa_sink_input_info structure
                MainLoop.Instance.Accept();

                // Wait for the operation to complete
                MainLoop.Instance.Wait();

            }

            // Dereference the pa_operation
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock the context
            Monitor.Exit(this);

            // Return the SinkInputInfo object
            return info;

        }

        /// <summary>
        /// Gets the list of sink inputs currently on the server
        /// </summary>        
        /// <returns>List of SinkInputInfo objects</returns>
        public Task<IReadOnlyList<SinkInputInfo?>> GetSinkInputInfoListAsync() => Task.Run(GetSinkInputInfoList);

        /// <summary>
        /// Gets the list of sink input currently on the server
        /// </summary>        
        /// <returns>List of SinkInputInfo objects</returns>
        public IReadOnlyList<SinkInputInfo?> GetSinkInputInfoList()
        {

            List<SinkInputInfo?> SinkInputs = new();

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            pa_sink_input_info* sink_input_info = null;

            // Call the native get_sink_input_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_sink_input_info_list(pa_Context, &SinkInputCallback, &sink_input_info);

            do
            {
                // Wait for the callback to signal
                while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && sink_input_info == null)
                    MainLoop.Instance.Wait();


                // If the callback returned data
                if (sink_input_info != null)
                {
                    // Copy the unmanaged sink_input_info structure into a SinkInputInfo object
                    // and add to the list
                    SinkInputs.Add(SinkInputInfo.Convert(*sink_input_info));

                    // Allow PulseAudio to free the sink_input_info
                    MainLoop.Instance.Accept();

                    sink_input_info = null;

                    continue;
                }
                else
                {
                    break;
                }


            } while (true);

            // Dereference the pa_operation
            pa_operation_unref(op);


            // Unlock the mainloop
            MainLoop.Instance.Unlock();


            // Unlock the context
            Monitor.Exit(this);

            // Return the SinkinputInfo list
            return SinkInputs;
        }

        #endregion


    }
}