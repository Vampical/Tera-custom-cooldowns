﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using TeraDataLite;

namespace TCC.TeraCommon.Game.Services
{
    //TODO: make this the same as other DBs
    public class ServerDatabase
    {
        private const string DefaultOverride = "###########################################################\n# Add additional servers in this file (needed only for\n# VPN/Proxy that is not supported out-of-box)\n#\n# Format must follow the format IP Region ServerName\n#\n# Example:\n# 111.22.33.44 NA VPN Server 1\n#\n# Current possible regions: EU, NA, RU, KR, TW, JP\n#\n# Lines starting with '#' are ignored\n# Place servers below the next line\n###########################################################";

        private readonly List<Server> _servers;
        private List<Server> _serverlist;

        public ServerDatabase(string folder)
        {
            _serverlist = File.ReadAllLines(Path.Combine(folder, "servers.txt"))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Split(new[] { ' ' }, 4))
                .Select(
                    parts =>
                        new Server(parts[3], parts[1], parts[0],
                            !string.IsNullOrEmpty(parts[2]) ? uint.Parse(parts[2]) : uint.MaxValue)).ToList();
            _servers = _serverlist.Where(x => x.ServerId != uint.MaxValue).ToList();
            _serverlist.Add(new Server("VPN", "Unknown", "127.0.0.1"));

            var serversOverridePath = Path.Combine(App.ResourcesPath, "config/server-overrides.txt");
            if (!File.Exists(serversOverridePath))//create the default file if it doesn't exist
                File.WriteAllText(serversOverridePath, DefaultOverride);
            var overriddenServers = GetServers(serversOverridePath).ToList();
            AddOverrides(overriddenServers);

        }

        public string Region { get; set; }
        private LangEnum _language;
        public LangEnum Language
        {
            get => _language;
            set
            {
                _language = value;
                switch (_language)
                {
                    case LangEnum.EN:
                    case LangEnum.GER:
                    case LangEnum.FR:
                        Region = "EU"; break;
                    case LangEnum.THA:
                    case LangEnum.SE:
                        Region = "THA"; break;
                    default:
                        Region = _language.ToString(); break;
                }
            }
        }

        public string StringLanguage
        {
            get
            {
                var ret = _language.ToString();
                switch (_language)
                {
                    case LangEnum.GER:
                    case LangEnum.FR:
                    case LangEnum.EN:
                        ret = "EU-" + ret;
                        break;
                }

                return ret;
            }
        }

        private static IEnumerable<Server> GetServers(string filename)
        {
            return File.ReadAllLines(filename)
                       .Where(s => !s.StartsWith("#") && !string.IsNullOrWhiteSpace(s))
                       .Select(s => s.Split(new[] { ' ' }, 3))
                       .Select(parts => new Server(parts[2], parts[1], parts[0]));
        }
        public Dictionary<string, Server> GetServersByIp()
        {
            return _serverlist.GroupBy(x => x.Ip).ToDictionary(x => x.Key, x => x.First());
        }
        public Server GetServer(uint serverId, Server oldServer = null)
        {
            var servers = _servers.Where(x => x.ServerId == serverId).ToList();
            if (!servers.Any()) return oldServer;
            return servers.FirstOrDefault(x => x.Region == Region) ?? servers.First();
        }
        public string GetServerName(uint serverId, Server oldServer = null)
        {
            var servers = _servers.Where(x => x.ServerId == serverId).ToList();
            if (!servers.Any()) return oldServer?.Name ?? $"{serverId}";
            return servers.FirstOrDefault(x => x.Region == Region)?.Name ?? servers.First().Name;
        }

        private void AddOverrides(IEnumerable<Server> newServers)
        {
            _serverlist = _serverlist.Concat(newServers.Where(sl => _serverlist.All(os => os.Ip != sl.Ip))).ToList();
        }

    }
}