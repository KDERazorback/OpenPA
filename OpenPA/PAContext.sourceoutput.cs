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


        #region SourceOutput

        #region Callbacks
        /// <summary>
        /// Callback for the get_source_output_info family of calls
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="info">source_output_info data</param>
        /// <param name="eol">End of list marker</param>
        /// <param name="userdata">Mainloop pointer should be passed in here</param>
        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
        static unsafe void SourceOutputCallback(pa_context* ctx, pa_source_output_info* info, int eol, void* userdata)
        {
            // Test for the end of list
            if (eol == 0)
            {
                // Not the end of the list

                // Copy the pa_source_output_info address
                *((pa_source_output_info**)userdata) = info;

                // Signal the mainloop to continue
                MainLoop.Instance.Signal(1);
            }
            else
            {
                // Return null
                *((pa_source_output_info**)userdata) = null;

                // Signal the mainloop
                MainLoop.Instance.Signal(0);
            }

        }
        
        #endregion

        /// <summary>
        /// Gets a SourceOutputInfo object describing a source output
        /// </summary>
        /// <param name="index">Idx of the source output</param>
        /// <returns>SourceOutputInfo object</returns>
        public Task<SourceOutputInfo?> GetSourceOutputInfoAsync(uint index) => Task.Run(() => GetSourceOutputInfo(index));

        /// <summary>
        /// Gets a SourceOutputInfo object describing a source output
        /// </summary>
        /// <param name="index">Idx of the source output</param>
        /// <returns>SourceOutputInfo object</returns>
        public SourceOutputInfo? GetSourceOutputInfo(uint index)
        {
            // Returned object
            SourceOutputInfo? info = default;

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Pointer to returned structure
            pa_source_output_info* source_output_info = null;

            // Call the native get_source_output_info native function passing in the idx and callback
            pa_operation* op = pa_context.pa_context_get_source_output_info(pa_Context, index, &SourceOutputCallback, &source_output_info);

            // Wait for the callback to signal
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && source_output_info == null)
                MainLoop.Instance.Wait();


            // If the callback returned data
            if (source_output_info != null)
            {
                // Copy the unmanaged source_output_info structure into a SourceOutputInfo object
                info = SourceOutputInfo.Convert(*source_output_info);

                // Allow PulseAudio to free the pa_source_output_info structure
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

            // Return the SourceOutputInfo object
            return info;

        }

        /// <summary>
        /// Gets the list of source outputs currently on the server
        /// </summary>        
        /// <returns>List of SourceOutputInfo objects</returns>
        public Task<IReadOnlyList<SourceOutputInfo?>> GetSourceOutputInfoListAsync() => Task.Run(GetSourceOutputInfoList);

        /// <summary>
        /// Gets the list of source output currently on the server
        /// </summary>        
        /// <returns>List of SourceOutputInfo objects</returns>
        public IReadOnlyList<SourceOutputInfo?> GetSourceOutputInfoList()
        {

            List<SourceOutputInfo?> sourceOutputs = new();

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            pa_source_output_info* source_output_info = null;

            // Call the native get_source_output_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_source_output_info_list(pa_Context, &SourceOutputCallback, &source_output_info);

            do
            {
                // Wait for the callback to signal
                while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && source_output_info == null)
                    MainLoop.Instance.Wait();


                // If the callback returned data
                if (source_output_info != null)
                {
                    // Copy the unmanaged source_output_info structure into a SourceOutputInfo object
                    // and add to the list
                    sourceOutputs.Add(SourceOutputInfo.Convert(*source_output_info));

                    // Allow PulseAudio to free the source_output_info
                    MainLoop.Instance.Accept();

                    source_output_info = null;

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

            // Return the SinkOutputInfo list
            return sourceOutputs;
        }

        #endregion


    }
}
