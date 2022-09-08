using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collo.Cloud.Services.Libraries.Shared.Helpers
{
    public class BucketHelper
    {
        /// <summary>
        /// Get Dictionary of Hardcoded InstrumentId Strings (Temporary Solution)
        /// </summary>
        /// <returns>Dictionary of Hardcoded InstrumentId Strings</returns>
        public static Dictionary<int, string> GetInstruments()
        {
            return new() //InstrumentIds, Temp Data
            {
                {0, "yoqHAIO" },
                {1, "VmIVw3Z" },
                {2, "h0LcLsm" }
            };
        }

        /// <summary>
        /// Get Dictionary of Hardcoded Feature Strings (Temporary Solution)
        /// </summary>
        /// <returns>Dictionary of Hardcoded Feature Strings</returns>
        public static Dictionary<int, string> GetFeatures()
        {
            return new() //Features, Temp Data
            {
                { 0, "collo/features/civ/calibrated" },
                { 1, "collo/features/civ/calibrated/accepted" },
                { 2, "collo/features/civ/compensated" },
                { 3, "collo/features/civ/compensated/accepted" },
                { 4, "collo/features/civ/raw" },
                { 5, "collo/features/civ/raw/accepted" },
                { 6, "collo/features/cp/calibrated" },
                { 7, "collo/features/cp/calibrated/accepted" },
                { 8, "collo/features/cp/compensated" },
                { 9, "collo/features/cp/compensated/accepted" },
                { 10, "collo/features/cp/raw" },
                { 11, "collo/features/cp/raw/accepted" },
                { 12, "collo/features/t1/raw" },
                { 13, "collo/features/t1/raw/accepted" },
                { 14, "collo/features/t2/raw" },
                { 15, "collo/features/t2/raw/accepted" },
                { 16, "collo/features/t3/raw" },
                { 17, "collo/features/t3/raw/accepted" },
                { 18, "collo/features/temperature/external/raw/ohm" },
                { 19, "collo/features/temperature/internal/raw/ohm" },
                { 20, "collo/features/z1/raw" },
                { 21, "collo/features/z1/raw/accepted" },
                { 22, "collo/features/z2/raw" },
                { 23, "collo/features/z2/raw/accepted" },
                { 24, "collo/features/z3/raw" },
                { 25, "collo/features/z3/raw/accepted" }
            };
        }

        /// <summary>
        /// Get TimeBoxLength in Milliseconds
        /// </summary>
        /// <returns>TimeBoxLength in Milliseconds</returns>
        public static int GetTimeBoxLength()
        {
            return 120000;
        }

        /// <summary>
        /// Get Resolution in Milliseconds
        /// </summary>
        /// <returns>Resolution in Milliseconds</returns>
        public static int GetResolution()
        {
            return 1000;
        }

        /// <summary>
        /// Get Bucket PreCreationPeriod in Milliseconds
        /// </summary>
        /// <returns>PreCreationPeriod in Milliseconds</returns>
        public static int GetPreCreationPeriod()
        {
            return 600000; //10min
        }

        /// <summary>
        /// Get Bucket Item Count (Times and Values)
        /// </summary>
        /// <returns>Bucket Item Count</returns>
        public static int GetBucketItemCount()
        {
            var timeBoxLength = GetTimeBoxLength();
            var resolution = GetResolution();

            return timeBoxLength / resolution;
        }

        public static int GetNoOfBuckets()
        {
            var preCreationPeriod = GetPreCreationPeriod();
            var timeBoxLength = GetTimeBoxLength();

            return preCreationPeriod / timeBoxLength;
        }
    }
}
