/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Diagnostics;

namespace Thinktecture.IdentityModel.Utility
{
    internal static class Tracing
    {
        public const string SOURCE = "Thinktecture.IdentityModel";

        public static void Warning(string message)
        {
            Trace(message, TraceEventType.Warning);
        }

        public static void Error(string message)
        {
            Trace(message, TraceEventType.Error);
        }

        public static void Info(string message)
        {
            Trace(message, TraceEventType.Information);
        }

        private static void Trace(string message, TraceEventType type)
        {
            TraceSource ts = new TraceSource(SOURCE);

            if (System.Diagnostics.Trace.CorrelationManager.ActivityId == Guid.Empty)
            {
                System.Diagnostics.Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            }

            ts.TraceEvent(type, 0, message);
        }
    }
}