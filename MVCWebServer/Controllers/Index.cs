using BeardedManStudios.Forge.Networking;
using System;

namespace BeardedManStudios.Forge.MVCWebServer.Controllers
{
    public class Index : PageController
    {
        public Index(NetWorker socket, ForgeWebServer webserver)
            : base(socket, webserver)
        {
            variables.Add("runTime", GetSimulatedTime);
            variables.Add("startTime", GetStartTime);
            variables.Add("bandwidthIn", GetBandwidthIn);
            variables.Add("bandwidthOut", GetBandwidthOut);
        }

        private string GetName()
        {
            return "Brent Farris";
        }

        private string GetSimulatedTime()
        {
            return Networker.Time.Timestep.ToString();
        }

        private string GetStartTime()
        {
            return (DateTime.Now - new TimeSpan(0, 0, 0, 0, (int)(Networker.Time.Timestep * 1000.0f))).ToString();
        }

        private string GetBandwidthIn()
        {
            return GetBandwidth(Networker.BandwidthIn).ToString();
        }

        private string GetBandwidthOut()
        {
            return GetBandwidth(Networker.BandwidthOut).ToString();
        }

        private string GetBandwidth(ulong bandwidth)
        {
            string bandwidthIn = bandwidth.ToString() + " B";

            if (bandwidth > 549755813888)
                bandwidthIn = (bandwidth / 1099511627776.0) + " TB";
            else if (bandwidth > 536870912)
                bandwidthIn = (bandwidth / 1073741824) + " GB";
            else if (bandwidth > 524288)
                bandwidthIn = (bandwidth / 1048576.0) + " MB";
            else if (bandwidth > 500)
                bandwidthIn = (bandwidth / 1024.0) + " KB";

            return bandwidthIn;
        }

        public override string Render()
        {
            return PresentHTML("index");
        }

        public string Players(params object[] args)
        {
            return PresentHTML("index");
        }
    }
}