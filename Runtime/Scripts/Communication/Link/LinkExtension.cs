using UnityEngine;

namespace OC.Communication
{
    public static class LinkExtension
    {
        public static string GetName(this Link link)
        {
            var name = link.Component.gameObject.name;
            var parent = link.Parent != null ?  link.Parent.transform : link.Component.transform.parent; 
            
            while (parent != null)
            {
                if (parent.TryGetComponent<Hierarchy>(out var hierarchy))
                {
                    if (hierarchy.IsNameSampler)
                    {
                        name = hierarchy.Name + "_" + name;
                        parent = hierarchy.GetParent();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    parent = parent.parent;
                }
            }

            return name;
        }

        public static string GetPath(this Link link, bool original = false)
        {
            var path = link.Component.gameObject.name;
            Transform parent;

            if (!original && link.Parent != null)
            {
                parent = link.Parent.transform;
            }
            else
            {
                parent = link.Component.transform.parent;
            }

            while (parent != null)
            {
                if (parent.TryGetComponent<Client>(out var client))
                {
                    return client.RootName + "." + path;
                }
                
                if (parent.TryGetComponent<Hierarchy>(out var hierarchy))
                {
                    path = hierarchy.IsNameSampler ? hierarchy.Name + "_" + path : hierarchy.Name + "." + path;
                    parent = original ? hierarchy.transform.parent : hierarchy.GetParent();
                    continue;
                }
                
                parent = parent.parent;
            }

            return path;
        }

        public static bool IsPathOriginal(this Link link)
        {
            if (link.Parent != null) return false;
            var parent = link.Component.transform.parent; 
            while (parent != null)
            {
                if (parent.TryGetComponent<Client>(out _))
                {
                    return true;
                }

                if (parent.TryGetComponent<Hierarchy>(out var hierarchy))
                {
                    if (hierarchy.Parent != null)
                    {
                        return false;
                    }
                    
                    parent = hierarchy.transform.parent;
                }
                else
                {
                    parent = parent.parent;
                }
            }

            return true;
        }
        
        public static Client GetClient(this Link link)
        {
            var parent = link.Parent != null ?  link.Parent.transform : link.Component.transform.parent;
            
            while (parent != null)
            {
                if (parent.TryGetComponent<Client>(out var client))
                {
                    return client;
                }
                
                parent = parent.parent;
            }

            return null;
        }
    }
}
