// <auto-generated />
#nullable enable

using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Meziantou.Framework.Http;

partial class HstsDomainPolicyCollection
{
    private void LoadPreloadDomains()
    {
        // HSTS preload data source: https://raw.githubusercontent.com/chromium/chromium/e988468e33e03f8a4eec34ad06ce7f4918cdcdbf/net/http/transport_security_state_static.json
        // Commit date: 2025-03-28T20:37:45.0000000+00:00
        CollectionsMarshal.SetCount(_policies, 5);

        var dict1 = new ConcurrentDictionary<string, HstsDomainPolicy>(concurrencyLevel: -1, capacity: 61, comparer: StringComparer.OrdinalIgnoreCase);
        _policies[0] = dict1;
        Load(dict1, 51, "preload_1.bin");

        var dict2 = new ConcurrentDictionary<string, HstsDomainPolicy>(concurrencyLevel: -1, capacity: 153277, comparer: StringComparer.OrdinalIgnoreCase);
        _policies[1] = dict2;
        Load(dict2, 153267, "preload_2.bin");

        var dict3 = new ConcurrentDictionary<string, HstsDomainPolicy>(concurrencyLevel: -1, capacity: 11956, comparer: StringComparer.OrdinalIgnoreCase);
        _policies[2] = dict3;
        Load(dict3, 11946, "preload_3.bin");

        var dict4 = new ConcurrentDictionary<string, HstsDomainPolicy>(concurrencyLevel: -1, capacity: 196, comparer: StringComparer.OrdinalIgnoreCase);
        _policies[3] = dict4;
        Load(dict4, 186, "preload_4.bin");

        var dict5 = new ConcurrentDictionary<string, HstsDomainPolicy>(concurrencyLevel: -1, capacity: 11, comparer: StringComparer.OrdinalIgnoreCase);
        _policies[4] = dict5;
        dict5.TryAdd("wnc-frontend-alb-1765173526.ap-northeast-2.elb.amazonaws.com", new HstsDomainPolicy("wnc-frontend-alb-1765173526.ap-northeast-2.elb.amazonaws.com", _expires1year, true));
    }
}