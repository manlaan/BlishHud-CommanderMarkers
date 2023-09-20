using Manlaan.CommanderMarkers.Library.Models;
using Newtonsoft.Json;
using System;

namespace Manlaan.CommanderMarkers.Library.Services;

public class CommunityMarkerService
{
    protected const string FILE_URL = "https://bhm.blishhud.com/Manlaan.CommanderMarkers/Community/Markers.json";

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
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(FILE_URL);

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
        catch (Exception r) {
            return new CommunitySets();
        }

    }
}
