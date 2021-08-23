using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public static class AnalyticsController
{
    public static void SendAnalyticResult(string name)
    {
        AnalyticsResult analyticsResult = Analytics.CustomEvent(name);
        Debug.Log($"analyticsResult: {analyticsResult}");
    }

    public static void SendAnalyticDictionary(string name, string keyName,object keyValue)
    {
        AnalyticsResult analyticsResult = Analytics.CustomEvent(
            name,
            new Dictionary<string, object>
            {
                {keyName,keyValue}
            }
            );
        Debug.Log($"analyticsDictionaryResult: {analyticsResult}");
    }
}
