﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeZoneBot
{
    public static class TimeZoneConversion
    {
        public static Dictionary<string, string> shortToName = new Dictionary<string, string>
        {
            {"DST-12", "Dateline Standard Time"},
            {"UTC-11", "UTC-11"},
            {"AST-10", "Aleutian Standard Time"},
            {"HST-10", "Hawaiian Standard Time"},
            {"MST-9.5", "Marquesas Standard Time"},
            {"AST-9", "Alaskan Standard Time"},
            {"UTC-9", "UTC-09"},
            {"PSTM-8", "Pacific Standard Time (Mexico)"},
            {"UTC-8", "UTC-08"},
            {"PST-8", "Pacific Standard Time"},
            {"USMST-7", "US Mountain Standard Time"},
            {"MSTM-7", "Mountain Standard Time (Mexico)"},
            {"MST-7", "Mountain Standard Time"},
            {"CAST-6", "Central America Standard Time"},
            {"CST-6", "Central Standard Time"},
            {"EIST-6", "Easter Island Standard Time"},
            {"CSTM-6", "Central Standard Time (Mexico)"},
            {"CCST-6", "Canada Central Standard Time"},
            {"SAPST-5", "SA Pacific Standard Time"},
            {"ESTM-5", "Eastern Standard Time (Mexico)"},
            {"EST-5", "Eastern Standard Time"},
            {"HST-5", "Haiti Standard Time"},
            {"CST-5", "Cuba Standard Time"},
            {"USEST-5", "US Eastern Standard Time"},
            {"TCST-5", "Turks and Caicos Standard Time"},
            {"PST-4", "Paraguay Standard Time"},
            {"AST-4", "Atlantic Standard Time"},
            {"VST-4", "Venezuela Standard Time"},
            {"CBST-4", "Central Brazilian Standard Time"},
            {"SAWST-4", "SA Western Standard Time"},
            {"PSAST-4", "Pacific SA Standard Time"},
            {"NST-3.5", "Newfoundland Standard Time"},
            {"TST-3", "Tocantins Standard Time"},
            {"ESAST-3", "E. South America Standard Time"},
            {"SAEST-3", "SA Eastern Standard Time"},
            {"AST-3", "Argentina Standard Time"},
            {"GST-3", "Greenland Standard Time"},
            {"MoST-3", "Montevideo Standard Time"},
            {"MaST-3", "Magallanes Standard Time"},
            {"SPST-3", "Saint Pierre Standard Time"},
            {"BST-3", "Bahia Standard Time"},
            {"UTC-2", "UTC-02"},
            {"MAST-2", "Mid-Atlantic Standard Time"},
            {"AST-1", "Azores Standard Time"},
            {"CVST-1", "Cabo Verde Standard Time"},
            {"CUT+0", "Co-ordinated Universal Time"},
            {"GMTST+0", "GMT Standard Time"},
            {"GST+0", "Greenwich Standard Time"},
            {"STST+0", "Sao Tome Standard Time"},
            {"MST+0", "Morocco Standard Time"},
            {"WEST+1", "W. Europe Standard Time"},
            {"CEST+1", "Central Europe Standard Time"},
            {"RST+1", "Romance Standard Time"},
            {"CEnST+1", "Central European Standard Time"},
            {"WCAST+1", "W. Central Africa Standard Time"},
            {"JoST+2", "Jordan Standard Time"},
            {"GTBST+2", "GTB Standard Time"},
            {"MEST+2", "Middle East Standard Time"},
            {"EST+2", "Egypt Standard Time"},
            {"EEST+2", "E. Europe Standard Time"},
            {"SyST+2", "Syria Standard Time"},
            {"WBGST+2", "West Bank Gaza Standard Time"},
            {"SAST+2", "South Africa Standard Time"},
            {"FLEST+2", "FLE Standard Time"},
            {"JeST+2", "Jerusalem Standard Time"},
            {"RTZST+2", "Russia TZ 1 Standard Time"},
            {"SuST+2", "Sudan Standard Time"},
            {"LST+2", "Libya Standard Time"},
            {"NST+2", "Namibia Standard Time"},
            {"AcST+3", "Arabic Standard Time"},
            {"TST+3", "Turkey Standard Time"},
            {"AST+3", "Arab Standard Time"},
            {"BST+3", "Belarus Standard Time"},
            {"RTZST+3", "Russia TZ 2 Standard Time"},
            {"EAST+3", "E. Africa Standard Time"},
            {"IST+3.5", "Iran Standard Time"},
            {"ArST+4", "Arabian Standard Time"},
            {"AsST+4", "Astrakhan Standard Time"},
            {"AzST+4", "Azerbaijan Standard Time"},
            {"RTZST+4", "Russia TZ 3 Standard Time"},
            {"MST+4", "Mauritius Standard Time"},
            {"SST+4", "Saratov Standard Time"},
            {"GST+4", "Georgian Standard Time"},
            {"VST+4", "Volgograd Standard Time"},
            {"CST+4", "Caucasus Standard Time"},
            {"AfST+4.5", "Afghanistan Standard Time"},
            {"WAST+5", "West Asia Standard Time"},
            {"RTZST+5", "Russia TZ 4 Standard Time"},
            {"PST+5", "Pakistan Standard Time"},
            {"QST+5", "Qyzylorda Standard Time"},
            {"IST+5.5", "India Standard Time"},
            {"SLST+5.5", "Sri Lanka Standard Time"},
            {"NST+5.75", "Nepal Standard Time"},
            {"CAST+6", "Central Asia Standard Time"},
            {"BST+6", "Bangladesh Standard Time"},
            {"OST+6", "Omsk Standard Time"},
            {"MST+6.5", "Myanmar Standard Time"},
            {"SEAST+7", "SE Asia Standard Time"},
            {"AST+7", "Altai Standard Time"},
            {"WMST+7", "W. Mongolia Standard Time"},
            {"RTZST+7", "Russia TZ 6 Standard Time"},
            {"NST+7", "Novosibirsk Standard Time"},
            {"TST+7", "Tomsk Standard Time"},
            {"CST+8", "China Standard Time"},
            {"RTZST+8", "Russia TZ 7 Standard Time"},
            {"MPST+8", "Malay Peninsula Standard Time"},
            {"WAST+8", "W. Australia Standard Time"},
            {"TST+8", "Taipei Standard Time"},
            {"UST+8", "Ulaanbaatar Standard Time"},
            {"ACWST+8.75", "Aus Central W. Standard Time"},
            {"TrST+9", "Transbaikal Standard Time"},
            {"ToST+9", "Tokyo Standard Time"},
            {"NKST+9", "North Korea Standard Time"},
            {"KST+9", "Korea Standard Time"},
            {"RTZST+9", "Russia TZ 8 Standard Time"},
            {"CAST+9.5", "Cen. Australia Standard Time"},
            {"AUSCST+9.5", "AUS Central Standard Time"},
            {"EAST+10", "E. Australia Standard Time"},
            {"AUSEST+10", "AUS Eastern Standard Time"},
            {"WPST+10", "West Pacific Standard Time"},
            {"TST+10", "Tasmania Standard Time"},
            {"RTZST+10", "Russia TZ 9 Standard Time"},
            {"LHST+10.5", "Lord Howe Standard Time"},
            {"BST+11", "Bougainville Standard Time"},
            {"RTZST+11", "Russia TZ 10 Standard Time"},
            {"MST+11", "Magadan Standard Time"},
            {"NST+11", "Norfolk Standard Time"},
            {"SST+11", "Sakhalin Standard Time"},
            {"CPST+11", "Central Pacific Standard Time"},
            {"RTZST+12", "Russia TZ 11 Standard Time"},
            {"NZST+12", "New Zealand Standard Time"},
            {"UTC+12", "UTC+12"},
            {"FST+12", "Fiji Standard Time"},
            {"KST+12", "Kamchatka Standard Time"},
            {"CIST+12.75", "Chatham Islands Standard Time"},
            {"UTC+13", "UTC+13"},
            {"TST+13", "Tonga Standard Time"},
            {"SST+13", "Samoa Standard Time"},
            {"LIST+14", "Line Islands Standard Time"}
        };
    }
}