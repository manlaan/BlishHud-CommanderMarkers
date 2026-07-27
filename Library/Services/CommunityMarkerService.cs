using Manlaan.CommanderMarkers.Library;
using Manlaan.CommanderMarkers.Library.Models;
using Newtonsoft.Json;
using System;

namespace Manlaan.CommanderMarkers.Library.Services;

public class CommunityMarkerService
{
    protected static string FileUrl => DevEndpoints.LegacyCommunityMarkersUrl;

    protected CommunitySets? _communitySets;

    public CommunitySets CommunitySets { get
        {
            if (_communitySets == null)
            {
                return FetchListing();
            }
            else
            {
                return _communitySets;
            }
        }  
    }

    public event EventHandler<CommunitySets>? CommunitySetsUpdated;

    public CommunityMarkerService()
    {

    }

    public CommunitySets FetchListing()
    {
        try
        {
            using (var webClient = ModuleHttp.CreateClient())
            {
                var json = webClient.DownloadString(FileUrl);

                CommunitySets? sets = JsonConvert.DeserializeObject<CommunitySets>(json);

                if (sets == null)
                {
                    return new CommunitySets();
                }


                _communitySets = sets;
                CommunitySetsUpdated?.Invoke(this,_communitySets);
                return _communitySets ;
            }
        }
        catch (Exception)
        {
            return new CommunitySets();
        }

    }
}
