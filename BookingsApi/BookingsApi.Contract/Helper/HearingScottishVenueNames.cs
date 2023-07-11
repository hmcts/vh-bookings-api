using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BookingsApi.Contract.Helper;

[SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed")]
public static class HearingScottishVenueNames
{
    public const string Aberdeen = "Aberdeen Tribunal Hearing Centre";
    public const string Dundee = "Dundee Tribunal Hearing Centre";
    public const string Edinburgh = "Edinburgh Employment Tribunal";
    public const string Glasgow = "Glasgow Tribunals Centre";
    public const string Inverness = "Inverness Employment Tribunal";
    public const string Ayr = "Ayr";
    public const string HamiltonBrandonGate = "Hamilton Brandon Gate";
    public const string StirlingWallaceHouse = "Stirling Wallace House";
    public const string EdinburghEmploymentAppealTribunal = "Edinburgh Employment Appeal Tribunal";
    public const string InvernessJusticeCentre = "Inverness Justice Centre";
    public const string EdinburghSocialSecurityAndChildSupportTribunal = "Edinburgh Social Security and Child Support Tribunal";
    public const string EdinburghUpperTribunal = "Edinburgh Upper Tribunal (Administrative Appeals Chamber)";
    
    public const string Benbecula = "Benbecula";
    public const string Campbeltown = "Campbeltown";
    public const string Galashiels = "Galashiels";
    public const string AtlanticQuayGlasgow = "Atlantic Quay Glasgow";
    public const string Greenock = "Greenock";
    public const string HamiltonSocialSecurityAndChildSupportTribunal = "Hamilton Social Security and Child Support Tribunal";
    public const string InvernessSocialSecurityAndChildSupportTribunal = "Inverness Social Security and Child Support Tribunal";
    public const string Kilmarnock = "Kilmarnock";
    public const string Kirkcaldy = "Kirkcaldy";
    public const string Kirkwall = "Kirkwall";
    public const string Lerwick = "Lerwick";
    public const string Oban = "Oban";
    public const string Stranraer = "Stranraer";

    [Obsolete("Use the IsScottish property on the HearingVenue object instead of this list")]
    public static readonly IReadOnlyCollection<string> ScottishHearingVenuesList = new List<string> {
        Aberdeen,
        Ayr,
        Dundee,
        Edinburgh,
        Glasgow,
        HamiltonBrandonGate,
        Inverness,
        StirlingWallaceHouse,
        EdinburghEmploymentAppealTribunal,
        InvernessJusticeCentre,
        EdinburghSocialSecurityAndChildSupportTribunal,
        EdinburghUpperTribunal,
        Benbecula,
        Campbeltown,
        Galashiels, 
        AtlanticQuayGlasgow,
        Greenock,
        HamiltonSocialSecurityAndChildSupportTribunal,
        InvernessSocialSecurityAndChildSupportTribunal,
        Kilmarnock, 
        Kirkcaldy,
        Kirkwall, 
        Lerwick,
        Oban,
        Stranraer,
    };
}

