using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using GarminAnalyzer.FileHandling.Abstractions;
using GarminAnalyzer.Models;

namespace GarminAnalyzer.FileHandling
{
    public class OsmParser : IOsmParser
    {
        public Task<List<Way>> ParseOsm(string filename, int relationId)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException($"File {filename} does not exist");

            if (Path.GetExtension(filename)?.ToUpper() != ".OSM")
                throw new ArgumentException("Only files with extension .osm possible");
            var results = new List<Way>();

            var document = new XmlDocument();
            document.Load(filename);

            var references = new List<string>();

            if (document.DocumentElement?.ChildNodes == null) return Task.FromResult(results);

            foreach (XmlNode node in document.DocumentElement?.ChildNodes)
            {
                if (node.Name != "relation") continue;
                if (node.Attributes != null && node.Attributes["id"]?.InnerText != relationId.ToString())
                    continue;

                foreach (XmlNode relationNode in node.ChildNodes)
                    if (relationNode.Attributes != null)
                    {
                        if (relationNode.Attributes["type"]?.InnerText != "way") continue;
                        var reference = relationNode.Attributes["ref"]?.InnerText;
                        references.Add(reference);
                    }
            }

            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                if (node.Name != "way") continue;
                if (node.Attributes != null && !references.Contains(node.Attributes["id"]?.InnerText))
                    continue;


                var way = new Way();
                way.Id = int.Parse(node.Attributes?["id"]?.InnerText ?? "0");
                way.Nodes = new List<Position>();

                foreach (XmlNode childNode in node.ChildNodes)
                    if (childNode.Name == "nd")
                    {
                        var position = NodeLookup(childNode.Attributes?["ref"].InnerText,
                            document.DocumentElement);
                        if (position == null) continue;

                        var nodeObject = new Position
                        {
                            Latitude = position.Latitude,
                            Longitude = position.Longitude
                        };
                        way.Nodes.Add(nodeObject);
                    }
                    else if (childNode.Name == "tag")
                    {
                        var key = childNode.Attributes?["k"].InnerText;
                        if (key == null) continue;
                        var value = childNode.Attributes?["v"].InnerText;
                        switch (key)
                        {
                            case "piste:type":
                                way.Type = value;
                                break;
                            case "piste:difficulty":
                                way.Difficulty = value;
                                break;
                            case "piste:grooming":
                                way.Grooming = value;
                                break;
                            case "aerialway":
                                way.Type = value;
                                break;
                            case "aerialway:bubble":
                                way.Bubble = value;
                                break;
                            case "aerialway:occupancy":
                                way.Occupancy = int.Parse(value);
                                break;
                            case "aerialway:heating":
                                way.Heating = value;
                                break;
                            case "name":
                                way.Name = value;
                                break;
                            case "ref":
                                way.Ref = value;
                                break;
                        }
                    }

                results.Add(way);
            }


            return Task.FromResult(results);
        }

        private Position NodeLookup(string nodeId, XmlNode node)
        {
            if (nodeId == null) return null;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name != "node") continue;

                var id = childNode.Attributes?["id"].InnerText;
                if (id == null || id != nodeId) continue;

                var longitude = childNode.Attributes?["lon"].InnerText;
                var latitude = childNode.Attributes?["lat"].InnerText;

                var position = new Position
                {
                    Latitude = double.Parse(latitude, CultureInfo.InvariantCulture),
                    Longitude = double.Parse(longitude, CultureInfo.InvariantCulture)
                };
                return position;
            }

            return null;
        }
    }
}