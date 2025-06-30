using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using OC.Communication;
using OC.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OC
{
    public static class ProjectTreeFactory
    {
        public static void CreateAndSave(Component root)
        {
            var path = FileBrowser.SaveFilePanel("Create Client Configuration", Data.Utils.GetStreamingAssetsPath(), GetFileName(root.name), "xml");
            if (string.IsNullOrEmpty(path)) return;
            
            if (File.Exists(path)) File.Delete(path);
            
            var config = Create(root);
            using (var xmlWriter = XmlWriter.Create(path, new XmlWriterSettings{ OmitXmlDeclaration = true, Indent = true }))
            {
                config.Save(xmlWriter);
            }
            
            Logging.Logger.Log(LogType.Log, $"Project Tree created: {path}");
        }
        
        public static XElement Create(Component root)
        {
            var links = GetDevices(root);
            var element = new XElement("Main");
            foreach (var link in links)
            {
                CreateDevice(element, link);
            }

            return element;
        }
        
        private static string GetFileName(string root)
        {
            var sceneName = SceneManager.GetActiveScene().name.Replace(" ", "_").Replace(".", "_");
            return $"{sceneName}_{root}_Tree";
        }

        private static List<Link> GetDevices(Component root)
        {
            var devices = root.GetComponentsInChildren<IDevice>();
            var links = new List<Link>();
            
            foreach (var device in devices)
            {
                var link = device.Link;
                if (!link.Enable) continue;
                link.Initialize(device.Component);
                links.Add(link);
            }

            links = links.OrderBy(x => x.ScenePath).ToList();
            return links;
        }
        
        private static void CreateDevice(XElement root, Link link)
        {
            var groups = link.ScenePath.Split('.');
            var localRoot = root;

            for (var i = 1; i < groups.Length - 1; i++)
            {
                var node = FindNodeByName(localRoot, groups[i]);

                if (node == null)
                {
                    var newGroup = CreateGroup(groups[i]);
                    localRoot.Add(newGroup);
                    localRoot = newGroup;
                }
                else
                {
                    localRoot = node;
                }
            }

            localRoot.Add(CreateDevice(link));
        }
        
        private static XElement CreateDevice(Link link)
        {
            var device = new XElement("Device");
            device.Add(new XAttribute("Name", link.Name));
            device.Add(new XAttribute("Type", link.Type));

            foreach (var attribute in link.Attributes)
            {
                if (string.IsNullOrEmpty(attribute.Key))
                {
                    Logging.Logger.LogWarning($"Device: {link.ScenePath} {link.Type}: Attribute Key is empty");
                    continue;
                }
                device.Add(new XElement(attribute.Key, attribute.Value));
            }
            
            if (!link.IsPathOriginal())
            {
                var path = link.GetHierarchyPath(true);
                device.Add(new XElement("OriginalPath", path));
            }

            return device;
        }

        private static XElement CreateGroup(string name)
        {
            return new XElement("Group",
                new XAttribute("Name", name));
        }
        
        private static XElement FindNodeByName(XElement root, string name)
        {
            return !root.HasElements ? null : root.Elements().FirstOrDefault(item => item.Attribute("Name")?.Value == name);
        }
    }
}