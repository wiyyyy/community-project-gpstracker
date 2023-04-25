using GpsTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsTracker.GrainInterfaces
{
    public interface IDataCollector : IGrainWithGuidKey
    {
        ValueTask Collect(DeviceMessage json);
    }
}
